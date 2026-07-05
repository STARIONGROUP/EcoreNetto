// ------------------------------------------------------------------------------------------------
// <copyright file="StructuralFeatureHelperTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars.Tests
{
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using ECoreNetto;

    using HandlebarsDotNet;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="GeneralizationHelper"/> class
    /// </summary>
    [TestFixture]
    public class StructuralFeatureHelperTestFixture
    {
        private IHandlebars handlebarsContenxt = null!;

        private EPackage root = null!;

        [SetUp]
        public void Setup()
        {
            this.handlebarsContenxt = Handlebars.Create();
            this.handlebarsContenxt.Configuration.FormatProvider = CultureInfo.InvariantCulture;

            StructuralFeatureHelper.RegisterStructuralFeatureHelper(this.handlebarsContenxt);

            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");

            this.root = ModelLoader.Load(path);
        }

        [Test]
        public void Verify_that_StructuralFeature_QueryIsEnumerable_returns_expected_result()
        {
            var template = "{{ #StructuralFeature.QueryIsEnumerable this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");
            var eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "ingredients");

            var result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("True"));
        }

        [Test]
        public void Verify_that_StructuralFeature_QueryIsAttribute_returns_expected_result()
        {
            var template = "{{ #StructuralFeature.QueryIsAttribute this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");
            var eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "ingredients");

            var result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("False"));

            eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger");
            eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "minutes");

            result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("True"));
        }

        [Test]
        public void Verify_that_StructuralFeature_QueryIsReference_returns_expected_result()
        {
            var template = "{{ #StructuralFeature.QueryIsReference this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");
            var eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "ingredients");

            var result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("True"));

            eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger");
            eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "minutes");

            result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("False"));
        }

        [Test]
        public void Verify_that_StructuralFeature_QueryIsEnum_returns_expected_result()
        {
            var template = "{{ #StructuralFeature.QueryIsEnum this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");
            var eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "ingredients");

            var result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("False"));
            
            eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Amount");
            eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "unit");

            result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("True"));
        }

        [Test]
        public void Verify_that_StructuralFeature_QueryHasDefaultValue_returns_expected_result()
        {
            var template = "{{ #StructuralFeature.QueryHasDefaultValue this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");
            var eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "ingredients");

            var result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("False"));

            eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Amount");
            eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "unit");

            result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("True"));
        }

        [Test]
        public void Verify_that_StructuralFeature_QueryIsContainment_returns_expected_result()
        {
            var template = "{{ #StructuralFeature.QueryIsContainment this }}";

            var action = this.handlebarsContenxt.Compile(template);
            
            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Ingredient");
            var eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "amount");

            var result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("True"));

            eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger");
            eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "minutes");

            result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("False"));
        }

        [Test]
        public void Verify_that_StructuralFeature_NameEqualsEnclosingType_renders_when_names_match()
        {
            var template = "{{#StructuralFeature.NameEqualsEnclosingType feature eClass}}MATCH{{/StructuralFeature.NameEqualsEnclosingType}}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Amount");
            var eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "amount");

            var result = action(new { feature = eStructuralFeature, eClass });

            Assert.That(result, Is.EqualTo("MATCH"));
        }

        [Test]
        public void Verify_that_StructuralFeature_NameEqualsEnclosingType_does_not_render_when_names_differ()
        {
            var template = "{{#StructuralFeature.NameEqualsEnclosingType feature eClass}}MATCH{{/StructuralFeature.NameEqualsEnclosingType}}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Amount");
            var eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "unit");

            var result = action(new { feature = eStructuralFeature, eClass });

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Verify_that_StructuralFeature_NameEqualsEnclosingType_throws_when_not_exactly_two_arguments()
        {
            var template = "{{#StructuralFeature.NameEqualsEnclosingType feature}}MATCH{{/StructuralFeature.NameEqualsEnclosingType}}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Amount");
            var eStructuralFeature = eClass.EStructuralFeatures.Single(x => x.Name == "amount");

            Assert.Throws<HandlebarsException>(() => action(new { feature = eStructuralFeature }));
        }

        [Test]
        public void Verify_that_StructuralFeature_IsEnumerable_block_renders_only_for_enumerable_feature()
        {
            var template = "{{#StructuralFeature.IsEnumerable this}}ENUMERABLE{{/StructuralFeature.IsEnumerable}}";
            var action = this.handlebarsContenxt.Compile(template);

            var ingredients = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe")
                .EStructuralFeatures.Single(x => x.Name == "ingredients");
            Assert.That(action(ingredients), Is.EqualTo("ENUMERABLE"));

            var minutes = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger")
                .EStructuralFeatures.Single(x => x.Name == "minutes");
            Assert.That(action(minutes), Is.Empty);
        }

        [Test]
        public void Verify_that_StructuralFeature_IsAttribute_block_renders_only_for_attribute()
        {
            var template = "{{#StructuralFeature.IsAttribute this}}ATTRIBUTE{{/StructuralFeature.IsAttribute}}";
            var action = this.handlebarsContenxt.Compile(template);

            var minutes = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger")
                .EStructuralFeatures.Single(x => x.Name == "minutes");
            Assert.That(action(minutes), Is.EqualTo("ATTRIBUTE"));

            var ingredients = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe")
                .EStructuralFeatures.Single(x => x.Name == "ingredients");
            Assert.That(action(ingredients), Is.Empty);
        }

        [Test]
        public void Verify_that_StructuralFeature_IsReference_block_renders_only_for_reference()
        {
            var template = "{{#StructuralFeature.IsReference this}}REFERENCE{{/StructuralFeature.IsReference}}";
            var action = this.handlebarsContenxt.Compile(template);

            var ingredients = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe")
                .EStructuralFeatures.Single(x => x.Name == "ingredients");
            Assert.That(action(ingredients), Is.EqualTo("REFERENCE"));

            var minutes = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger")
                .EStructuralFeatures.Single(x => x.Name == "minutes");
            Assert.That(action(minutes), Is.Empty);
        }

        [Test]
        public void Verify_that_StructuralFeature_IsEnum_block_renders_only_for_enum_attribute()
        {
            var template = "{{#StructuralFeature.IsEnum this}}ENUM{{/StructuralFeature.IsEnum}}";
            var action = this.handlebarsContenxt.Compile(template);

            var unit = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Amount")
                .EStructuralFeatures.Single(x => x.Name == "unit");
            Assert.That(action(unit), Is.EqualTo("ENUM"));

            var ingredients = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe")
                .EStructuralFeatures.Single(x => x.Name == "ingredients");
            Assert.That(action(ingredients), Is.Empty);
        }

        [Test]
        public void Verify_that_StructuralFeature_QueryStructuralFeatureNameEqualsEnclosingType_returns_expected_result()
        {
            var template = "{{ #StructuralFeature.QueryStructuralFeatureNameEqualsEnclosingType feature eClass }}";
            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Amount");

            var amount = eClass.EStructuralFeatures.Single(x => x.Name == "amount");
            Assert.That(action(new { feature = amount, eClass }), Is.EqualTo("True"));

            var unit = eClass.EStructuralFeatures.Single(x => x.Name == "unit");
            Assert.That(action(new { feature = unit, eClass }), Is.EqualTo("False"));
        }

        [Test]
        public void Verify_that_StructuralFeature_QueryTypeName_returns_expected_result()
        {
            var template = "{{ #StructuralFeature.QueryTypeName this }}";
            var action = this.handlebarsContenxt.Compile(template);

            var ingredients = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe")
                .EStructuralFeatures.Single(x => x.Name == "ingredients");

            Assert.That(action(ingredients), Is.EqualTo("Ingredient"));
        }

        [Test]
        public void Verify_that_StructuralFeature_QueryTypeName_throws_when_context_is_not_a_structural_feature()
        {
            var template = "{{ #StructuralFeature.QueryTypeName this }}";
            var action = this.handlebarsContenxt.Compile(template);

            Assert.That(() => action("not-a-structural-feature"), Throws.ArgumentException);
        }

        [TestCase("StructuralFeature.QueryIsEnumerable", false)]
        [TestCase("StructuralFeature.QueryIsAttribute", false)]
        [TestCase("StructuralFeature.QueryIsReference", false)]
        [TestCase("StructuralFeature.QueryIsEnum", false)]
        [TestCase("StructuralFeature.QueryHasDefaultValue", false)]
        [TestCase("StructuralFeature.QueryIsContainment", false)]
        [TestCase("StructuralFeature.IsEnumerable", true)]
        [TestCase("StructuralFeature.IsAttribute", true)]
        [TestCase("StructuralFeature.IsReference", true)]
        [TestCase("StructuralFeature.IsEnum", true)]
        public void Verify_that_single_argument_helpers_throw_when_not_exactly_one_argument(string helper, bool isBlock)
        {
            var template = isBlock
                ? "{{#" + helper + " this that}}X{{/" + helper + "}}"
                : "{{ #" + helper + " this that }}";

            var action = this.handlebarsContenxt.Compile(template);

            var ingredients = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe")
                .EStructuralFeatures.Single(x => x.Name == "ingredients");

            Assert.Throws<HandlebarsException>(() => action(ingredients));
        }
    }
}
