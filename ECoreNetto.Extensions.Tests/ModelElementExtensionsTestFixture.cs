// -------------------------------------------------------------------------------------------------
// <copyright file="ModelElementExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
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
        private EPackage rootPackage;
        private ILoggerFactory loggerFactory; 
        
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
            EModelElement eModelElement = null;

            Assert.That(() => ModelElementExtensions.QueryDocumentation(eModelElement), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryRawDocumentation_throws_Exception_when_argument_null()
        {
            EModelElement eModelElement = null;

            Assert.That(() => ModelElementExtensions.QueryRawDocumentation(eModelElement), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_RemoveUnwantedHtmlTags_throws_Exception_when_argument_null()
        {
            string html = null;

            Assert.That(() => ModelElementExtensions.RemoveUnwantedHtmlTags(html, new List<string>()), Throws.ArgumentException);
        }
    }
}
