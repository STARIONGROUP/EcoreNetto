// ------------------------------------------------------------------------------------------------
// <copyright file="AnchorHelperTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars.Tests
{
    using System.IO;
    using System.Linq;

    using ECoreNetto;

    using HandlebarsDotNet;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="AnchorHelper"/> class
    /// </summary>
    [TestFixture]
    public class AnchorHelperTestFixture
    {
        private IHandlebars handlebarsContext = null!;

        private EPackage root = null!;

        [SetUp]
        public void Setup()
        {
            this.handlebarsContext = Handlebars.Create();

            AnchorHelper.RegisterAnchorHelper(this.handlebarsContext);

            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "report-features.ecore");
            this.root = ModelLoader.Load(path);
        }

        [Test]
        public void Verify_that_Anchor_writes_the_sanitized_slug()
        {
            var template = "{{ Anchor this }}";
            var action = this.handlebarsContext.Compile(template);

            var person = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Person");

            var result = action(person);

            Assert.Multiple(() =>
            {
                Assert.That(result, Does.Match("^[A-Za-z0-9-]+$"));
                Assert.That(result, Does.EndWith("Person"));
            });
        }

        [Test]
        public void Verify_that_Anchor_throws_when_not_exactly_one_argument()
        {
            var template = "{{ Anchor this that }}";
            var action = this.handlebarsContext.Compile(template);

            var person = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Person");

            Assert.Throws<HandlebarsException>(() => action(new { this_ = person, that = person }));
        }
    }
}
