// ------------------------------------------------------------------------------------------------
// <copyright file="ModelElementExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ECoreNetto.Extensions;
    using ECoreNetto.Resource;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="ModelElementExtensions"/> class
    /// </summary>
    [TestFixture]
    public class ModelElementExtensionsTestFixture
    {
        private EPackage rootPackage = null!;
        private ILoggerFactory loggerFactory = null!;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
        }

        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");
            var filePath = Path.GetFullPath(path);
            var uri = new System.Uri(filePath);

            var resourceSet = new ResourceSet(this.loggerFactory);
            var resource = resourceSet.CreateResource(uri);

            this.rootPackage = resource.Load(null);
        }

        [Test]
        public void Verify_that_QueryDocumentation_returns_the_expected_result()
        {
            var documentedClass = this.rootPackage.EClassifiers.Single(x => x.Name == "Ingredient");

            var documentation = documentedClass.QueryDocumentation();

            Assert.That(documentation.First(), Is.EqualTo("Any of the foods or substances that are combined to make a particular dish."));
            
            var undocumentedClass = this.rootPackage.EClassifiers.Single(x => x.Name == "Recipe");

            documentation = undocumentedClass.QueryDocumentation();

            Assert.That(documentation, Is.Empty);
        }

        [Test]
        public void Verify_that_QueryDocumentation_throws_Exception_when_argument_null()
        {
            EModelElement? eModelElement = null;

            Assert.That(() => ModelElementExtensions.QueryDocumentation(eModelElement!), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryRawDocumentation_throws_Exception_when_argument_null()
        {
            EModelElement? eModelElement = null;

            Assert.That(() => ModelElementExtensions.QueryRawDocumentation(eModelElement!), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_RemoveUnwantedHtmlTags_throws_Exception_when_argument_null()
        {
            string? html = null;

            Assert.That(() => ModelElementExtensions.RemoveUnwantedHtmlTags(html!, new List<string>()), Throws.ArgumentException);
        }

        [Test]
        public void Verify_that_QueryRawDocumentation_returns_expected_results()
        {
            var documentedClass = this.rootPackage.EClassifiers.Single(x => x.Name == "Ingredient");

            var documentation = documentedClass.QueryRawDocumentation();

            Assert.That(documentation, Is.EqualTo("Any of the foods or substances that are combined to make a particular dish."));

            var undocumentedClass = this.rootPackage.EClassifiers.Single(x => x.Name == "Recipe");

            documentation = undocumentedClass.QueryRawDocumentation();

            Assert.That(documentation, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Verify_that_RemoveUnwantedHtmlTags_returns_expected_results()
        {
            const string html = "<p>Hello <em>World</em></p>";
            var cleaned = ModelElementExtensions.RemoveUnwantedHtmlTags(html, new List<string> { "p", "em" });
            Assert.That(cleaned, Is.EqualTo("Hello World"));

            var unchanged = ModelElementExtensions.RemoveUnwantedHtmlTags(html, new List<string>());
            Assert.That(unchanged, Is.EqualTo(html));
        }

        [Test]
        public void Verify_that_RemoveUnwantedHtmlTags_returns_input_when_there_are_no_element_or_text_nodes()
        {
            // a comment-only fragment yields no element/text nodes, so the input is returned unchanged
            const string html = "<!-- just a comment -->";

            var result = ModelElementExtensions.RemoveUnwantedHtmlTags(html, new List<string> { "p" });

            Assert.That(result, Is.EqualTo(html));
        }
    }
}
