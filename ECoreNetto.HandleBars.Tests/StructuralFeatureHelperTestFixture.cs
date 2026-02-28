// -------------------------------------------------------------------------------------------------
// <copyright file="StructuralFeatureHelperTestFixture.cs" company="Starion Group S.A.">
// 
//   Copyright 2017-2025 Starion Group S.A.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
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
        private IHandlebars handlebarsContenxt;

        private EPackage root;

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
        public void Verify_that_StructuralFeature_IsEnumerable_block_helper_writes_content_only_when_true()
        {
            var template = "{{#StructuralFeature.IsEnumerable this}}Enumerable{{/StructuralFeature.IsEnumerable}}";
            var action = this.handlebarsContenxt.Compile(template);

            var recipeClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");
            var enumerableFeature = recipeClass.EStructuralFeatures.Single(x => x.Name == "ingredients");

            var timeTriggerClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger");
            var nonEnumerableFeature = timeTriggerClass.EStructuralFeatures.Single(x => x.Name == "minutes");

            Assert.Multiple(() =>
            {
                Assert.That(action(enumerableFeature), Is.EqualTo("Enumerable"));
                Assert.That(action(nonEnumerableFeature), Is.EqualTo(string.Empty));
            });
        }

        [Test]
        public void Verify_that_StructuralFeature_QueryTypeName_returns_expected_result()
        {
            var template = "{{#with this}}{{StructuralFeature.QueryTypeName}}{{/with}}";
            var action = this.handlebarsContenxt.Compile(template);

            var recipeClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");
            var eStructuralFeature = recipeClass.EStructuralFeatures.Single(x => x.Name == "name");

            var result = action(eStructuralFeature);

            Assert.That(result, Is.EqualTo("string"));
        }

        [Test]
        public void Verify_that_StructuralFeature_QueryTypeName_throws_when_context_is_not_a_structural_feature()
        {
            var template = "{{StructuralFeature.QueryTypeName}}";
            var action = this.handlebarsContenxt.Compile(template);

            Assert.That(() => action(new object()), Throws.TypeOf<ArgumentException>());
        }
    }
}
