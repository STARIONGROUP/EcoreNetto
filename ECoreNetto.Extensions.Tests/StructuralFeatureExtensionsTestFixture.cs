// ------------------------------------------------------------------------------------------------
// <copyright file="StructuralFeatureExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions.Tests
{
    using System.IO;
    using System.Linq;

    using ECoreNetto.Extensions;
    using ECoreNetto.Resource;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="StructuralFeatureExtensionsTestFixture"/> class
    /// </summary>
    [TestFixture]
    public class StructuralFeatureExtensionsTestFixture
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

            Assert.That(capacityClass!.Name, Is.EqualTo("Amount"));

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

        [Test]
        public void Verify_that_QueryTypeName_returns_expected_results()
        {
            var ingredientClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Ingredient");
            var amountStructuralFeature = ingredientClass.EStructuralFeatures.Single(x => x.Name == "amount");
            Assert.That(amountStructuralFeature.QueryTypeName(), Is.EqualTo("Amount"));

            var timeTriggerClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "TimeTrigger");
            var minutesStructuralFeature = timeTriggerClass.EStructuralFeatures.Single(x => x.Name == "minutes");
            Assert.That(minutesStructuralFeature.QueryTypeName(), Is.EqualTo("EInt"));
        }

        [Test]
        public void Verify_that_QueryIsNullable_returns_expected_results()
        {
            var relationClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "ContainmentRelation");
            var containerFeature = relationClass.EStructuralFeatures.Single(x => x.Name == "container");
            Assert.That(containerFeature.QueryIsNullable(), Is.False);

            var standardActionClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "StandardAction");
            var toolFeature = standardActionClass.EStructuralFeatures.Single(x => x.Name == "tool");
            Assert.That(toolFeature.QueryIsNullable(), Is.True);

            var recipeClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");
            var ingredientsFeature = recipeClass.EStructuralFeatures.Single(x => x.Name == "ingredients");
            Assert.That(ingredientsFeature.QueryIsNullable(), Is.False);
        }

        [Test]
        public void Verify_that_QueryStructuralFeatureNameEqualsEnclosingType_returns_expected_results()
        {
            var amountClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Amount");

            var amountFeature = amountClass.EStructuralFeatures.Single(x => x.Name == "amount");
            Assert.That(amountFeature.QueryStructuralFeatureNameEqualsEnclosingType(amountClass), Is.True);

            var unitFeature = amountClass.EStructuralFeatures.Single(x => x.Name == "unit");
            Assert.That(unitFeature.QueryStructuralFeatureNameEqualsEnclosingType(amountClass), Is.False);
        }

        [Test]
        public void Verify_that_QueryStructuralFeatureNameEqualsEnclosingType_throws_when_class_is_null()
        {
            var amountClass = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Amount");
            var amountFeature = amountClass.EStructuralFeatures.Single(x => x.Name == "amount");

            EClass nullClass = null!;

            Assert.That(() => amountFeature.QueryStructuralFeatureNameEqualsEnclosingType(nullClass),
                Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_the_extension_methods_throw_when_the_structural_feature_is_null()
        {
            EStructuralFeature feature = null!;

            var eClass = this.rootPackage.EClassifiers.OfType<EClass>().First();

            Assert.Multiple(() =>
            {
                Assert.That(() => feature.QueryIsEnum(), Throws.ArgumentNullException);
                Assert.That(() => feature.QueryClass(), Throws.ArgumentNullException);
                Assert.That(() => feature.QueryIsEnumerable(), Throws.ArgumentNullException);
                Assert.That(() => feature.QueryIsAttribute(), Throws.ArgumentNullException);
                Assert.That(() => feature.QueryIsReference(), Throws.ArgumentNullException);
                Assert.That(() => feature.QueryIsContainment(), Throws.ArgumentNullException);
                Assert.That(() => feature.QueryHasDefaultValue(), Throws.ArgumentNullException);
                Assert.That(() => feature.QueryTypeName(), Throws.ArgumentNullException);
                Assert.That(() => feature.QueryIsNullable(), Throws.ArgumentNullException);
                Assert.That(() => feature.QueryStructuralFeatureNameEqualsEnclosingType(eClass), Throws.ArgumentNullException);
            });
        }
    }
}
