// ------------------------------------------------------------------------------------------------
// <copyright file="StructuralFeatureHelperFeaturesTestFixture.cs" company="Starion Group S.A.">
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
    using HandlebarsDotNet.Helpers;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the identity and opposite helpers on <see cref="StructuralFeatureHelper"/>, exercised
    /// against the feature-rich synthetic model.
    /// </summary>
    [TestFixture]
    public class StructuralFeatureHelperFeaturesTestFixture
    {
        private IHandlebars handlebarsContext = null!;

        private EPackage root = null!;

        [SetUp]
        public void Setup()
        {
            this.handlebarsContext = Handlebars.Create();
            HandlebarsHelpers.Register(this.handlebarsContext);

            StructuralFeatureHelper.RegisterStructuralFeatureHelper(this.handlebarsContext);

            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "report-features.ecore");
            this.root = ModelLoader.Load(path);
        }

        private EStructuralFeature Feature(string className, string featureName)
        {
            return this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == className)
                .EStructuralFeatures.Single(x => x.Name == featureName);
        }

        [Test]
        public void Verify_that_QueryIsId_returns_true_for_an_id_attribute()
        {
            var template = "{{ #StructuralFeature.QueryIsId this }}";
            var action = this.handlebarsContext.Compile(template);

            Assert.Multiple(() =>
            {
                Assert.That(action(this.Feature("Person", "identifier")), Is.EqualTo("True"));
                Assert.That(action(this.Feature("Person", "tags")), Is.EqualTo("False"));
                Assert.That(action(this.Feature("Person", "employer")), Is.EqualTo("False"));
            });
        }

        [Test]
        public void Verify_that_QueryIsId_throws_when_not_exactly_one_argument()
        {
            var template = "{{ #StructuralFeature.QueryIsId this that }}";
            var action = this.handlebarsContext.Compile(template);

            Assert.Throws<HandlebarsException>(() => action(new { that = this.Feature("Person", "identifier") }));
        }

        [Test]
        public void Verify_that_WriteOpposite_renders_the_opposite_reference()
        {
            var template = "{{ StructuralFeature.WriteOpposite this }}";
            var action = this.handlebarsContext.Compile(template);

            var result = action(this.Feature("Person", "employer"));

            Assert.Multiple(() =>
            {
                Assert.That(result, Does.Contain("{opposite:"));
                Assert.That(result, Does.Contain("employees"));
            });
        }

        [Test]
        public void Verify_that_WriteOpposite_renders_nothing_for_a_reference_without_an_opposite()
        {
            var template = "{{ StructuralFeature.WriteOpposite this }}";
            var action = this.handlebarsContext.Compile(template);

            var result = action(this.Feature("Person", "addresses"));

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Verify_that_WriteOpposite_renders_nothing_for_an_attribute()
        {
            var template = "{{ StructuralFeature.WriteOpposite this }}";
            var action = this.handlebarsContext.Compile(template);

            var result = action(this.Feature("Person", "tags"));

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Verify_that_WriteOpposite_throws_when_not_exactly_one_argument()
        {
            var template = "{{ StructuralFeature.WriteOpposite this that }}";
            var action = this.handlebarsContext.Compile(template);

            Assert.Throws<HandlebarsException>(() => action(new { that = this.Feature("Person", "employer") }));
        }
    }
}
