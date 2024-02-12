// -------------------------------------------------------------------------------------------------
// <copyright file="StructuralFeatureExtensionsTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2024 RHEA System S.A.
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
    using System.IO;
    using System.Linq;

    using ECoreNetto.Extensions;
    using ECoreNetto.Resource;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="StructuralFeatureExtensionsTestFixture"/> class
    /// </summary>
    [TestFixture]
    public class StructuralFeatureExtensionsTestFixture
    {
        private EPackage rootPackage;

        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");
            var filePath = Path.GetFullPath(path);
            var uri = new System.Uri(filePath);

            var resourceSet = new ResourceSet();
            var resource = resourceSet.CreateResource(uri);

            this.rootPackage = resource.Load(null);
        }

        [Test]
        public void Verify_that_QueryIsEnum_returns_expected_results()
        {
            var amountClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Amount");

            var amountStructuralFeature = amountClass.EStructuralFeatures.Single(x => x.Name == "amount");

            Assert.That(amountStructuralFeature.QueryIsEnum(), Is.False);

            var unitStructuralFeature = amountClass.EStructuralFeatures.Single(x => x.Name == "unit");

            Assert.That(unitStructuralFeature.QueryIsEnum(), Is.True);

            var recipeClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");

            var ingredientsStructuralFeature = recipeClass.EStructuralFeatures.Single(x => x.Name == "ingredients");

            Assert.That(ingredientsStructuralFeature.QueryIsEnum(), Is.False);
        }

        [Test]
        public void Verify_that_QueryIsEnumerable_returns_expected_results()
        {
            var recipeClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");

            var ingredientsStructuralFeature = recipeClass.EStructuralFeatures.Single(x => x.Name == "ingredients");

            Assert.That(ingredientsStructuralFeature.QueryIsEnumerable, Is.True);

            var ingredientClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Ingredient");

            var amountStructuralFeature = ingredientClass.EStructuralFeatures.Single(x => x.Name == "amount");

            Assert.That(amountStructuralFeature.QueryIsEnumerable, Is.False);
        }

        [Test]
        public void Verify_that_QueryIsAttribute_returns_expected_results()
        {
            var recipeClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");

            var ingredientsStructuralFeature = recipeClass.EStructuralFeatures.Single(x => x.Name == "ingredients");

            Assert.That(ingredientsStructuralFeature.QueryIsAttribute, Is.False);

            var timeTriggerClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger");

            var minutesStructuralFeature = timeTriggerClass.EStructuralFeatures.Single(x => x.Name == "minutes");

            Assert.That(minutesStructuralFeature.QueryIsAttribute, Is.True);
        }

        [Test]
        public void Verify_that_QueryIsReference_returns_expected_results()
        {
            var recipeClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");

            var ingredientsStructuralFeature = recipeClass.EStructuralFeatures.Single(x => x.Name == "ingredients");

            Assert.That(ingredientsStructuralFeature.QueryIsReference, Is.True);

            var timeTriggerClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger");

            var minutesStructuralFeature = timeTriggerClass.EStructuralFeatures.Single(x => x.Name == "minutes");

            Assert.That(minutesStructuralFeature.QueryIsReference, Is.False);
        }

        [Test]
        public void Verify_that_QueryHasDefaultValue_returns_expected_results()
        {
            var amountClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Amount");

            var unitStructuralFeature = amountClass.EStructuralFeatures.Single(x => x.Name == "unit");

            Assert.That(unitStructuralFeature.QueryHasDefaultValue, Is.True);
        }

        [Test]
        public void Verify_that_QueryClass_returns_expected_result()
        {
            var containerClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Container");

            var capacityStructuralFeature = containerClass.EStructuralFeatures.Single(x => x.Name == "capacity");

            var capacityClass = capacityStructuralFeature.QueryClass();

            Assert.That(capacityClass.Name, Is.EqualTo("Amount"));

            var timeTriggerClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger");

            var minutesStructuralFeature = timeTriggerClass.EStructuralFeatures.Single(x => x.Name == "minutes");
            
            Assert.That(minutesStructuralFeature.QueryClass(), Is.Null);
        }

        [Test]
        public void Verify_that_QueryIsContainment_returns_expected_result()
        {
            var ingredientClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Ingredient");

            var amountStructuralFeature = ingredientClass.EStructuralFeatures.Single(x => x.Name == "amount");

            Assert.That(amountStructuralFeature.QueryIsContainment, Is.True);

            var timeTriggerClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger");

            var minutesStructuralFeature = timeTriggerClass.EStructuralFeatures.Single(x => x.Name == "minutes");

            Assert.That(minutesStructuralFeature.QueryIsContainment, Is.False);
        }
    }
}
