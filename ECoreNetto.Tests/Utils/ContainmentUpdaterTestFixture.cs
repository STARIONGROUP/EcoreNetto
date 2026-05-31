// -------------------------------------------------------------------------------------------------
// <copyright file="ContainmentUpdaterTestFixture.cs" company="Starion Group S.A.">
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

namespace ECoreNetto.Tests.Utils
{
    using ECoreNetto.Resource;
    using ECoreNetto.Utils;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ContainmentUpdater"/> class.
    /// </summary>
    [TestFixture]
    public class ContainmentUpdaterTestFixture
    {
        /// <summary>
        /// The <see cref="Resource"/> used to instantiate the <see cref="EObject"/>s under test
        /// </summary>
        private Resource resource;

        [SetUp]
        public void SetUp()
        {
            this.resource = new Resource();
        }

        [Test]
        public void Verify_that_RemoveFromContainer_throws_when_object_is_null()
        {
            EObject @object = null;

            Assert.That(() => @object.RemoveFromContainer(), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_RemoveFromContainer_is_a_noop_when_object_has_no_container()
        {
            var eClass = new EClass(this.resource) { Name = "Orphan" };

            Assert.Multiple(() =>
            {
                Assert.That(eClass.EContainer, Is.Null);
                Assert.That(() => eClass.RemoveFromContainer(), Throws.Nothing);
            });
        }

        [Test]
        public void Verify_that_an_EAnnotation_is_removed_from_its_container()
        {
            var eClass = new EClass(this.resource) { Name = "WithAnnotation" };
            var annotation = new EAnnotation(this.resource);
            eClass.EAnnotations.Add(annotation);

            annotation.RemoveFromContainer();

            Assert.That(eClass.EAnnotations, Does.Not.Contain(annotation));
        }

        [Test]
        public void Verify_that_a_sub_EPackage_is_removed_from_its_container()
        {
            var parent = new EPackage(this.resource) { Name = "Parent" };
            var child = new EPackage(this.resource) { Name = "Child" };
            parent.ESubPackages.Add(child);

            child.RemoveFromContainer();

            Assert.That(parent.ESubPackages, Does.Not.Contain(child));
        }

        [Test]
        public void Verify_that_an_EClassifier_is_removed_from_its_container()
        {
            var package = new EPackage(this.resource) { Name = "Pkg" };
            var eClass = new EClass(this.resource) { Name = "SomeClass" };
            package.EClassifiers.Add(eClass);

            eClass.RemoveFromContainer();

            Assert.That(package.EClassifiers, Does.Not.Contain(eClass));
        }

        [Test]
        public void Verify_that_an_EParameter_is_removed_from_its_container()
        {
            var operation = new EOperation(this.resource) { Name = "DoWork" };
            var parameter = new EParameter(this.resource) { Name = "arg" };
            operation.EParameters.Add(parameter);

            parameter.RemoveFromContainer();

            Assert.That(operation.EParameters, Does.Not.Contain(parameter));
        }

        [Test]
        public void Verify_that_an_EOperation_is_removed_from_its_container()
        {
            var eClass = new EClass(this.resource) { Name = "Owner" };
            var operation = new EOperation(this.resource) { Name = "DoWork" };
            eClass.EOperations.Add(operation);

            operation.RemoveFromContainer();

            Assert.That(eClass.EOperations, Does.Not.Contain(operation));
        }

        [Test]
        public void Verify_that_an_EStructuralFeature_is_removed_from_its_container()
        {
            var eClass = new EClass(this.resource) { Name = "Owner" };
            var attribute = new EAttribute(this.resource) { Name = "name" };
            eClass.EStructuralFeatures.Add(attribute);

            attribute.RemoveFromContainer();

            Assert.That(eClass.EStructuralFeatures, Does.Not.Contain(attribute));
        }

        [Test]
        public void Verify_that_an_EEnumLiteral_is_removed_from_its_container()
        {
            var eEnum = new EEnum(this.resource) { Name = "Color" };
            var literal = new EEnumLiteral(this.resource) { Name = "Red" };
            eEnum.ELiterals.Add(literal);

            literal.RemoveFromContainer();

            Assert.That(eEnum.ELiterals, Does.Not.Contain(literal));
        }

        [Test]
        public void Verify_that_adding_to_a_container_moves_the_object_from_its_previous_container()
        {
            var packageA = new EPackage(this.resource) { Name = "A" };
            var packageB = new EPackage(this.resource) { Name = "B" };
            var eClass = new EClass(this.resource) { Name = "Movable" };

            packageA.EClassifiers.Add(eClass);
            packageB.EClassifiers.Add(eClass);

            Assert.Multiple(() =>
            {
                Assert.That(packageA.EClassifiers, Does.Not.Contain(eClass));
                Assert.That(packageB.EClassifiers, Does.Contain(eClass));
                Assert.That(eClass.EContainer, Is.SameAs(packageB));
            });
        }

        [Test]
        public void Verify_that_an_EEnumLiteral_can_be_reparented_between_enums()
        {
            var enumA = new EEnum(this.resource) { Name = "EnumA" };
            var enumB = new EEnum(this.resource) { Name = "EnumB" };
            var literal = new EEnumLiteral(this.resource) { Name = "Literal" };

            enumA.ELiterals.Add(literal);

            // re-parenting triggers ContainmentUpdater.RemoveFromContainer for an EEnumLiteral
            // that already has a container; this must not throw (regression guard).
            Assert.That(() => enumB.ELiterals.Add(literal), Throws.Nothing);

            Assert.Multiple(() =>
            {
                Assert.That(enumA.ELiterals, Does.Not.Contain(literal));
                Assert.That(enumB.ELiterals, Does.Contain(literal));
                Assert.That(literal.EContainer, Is.SameAs(enumB));
            });
        }

        [Test]
        public void Verify_that_RemoveFromContainer_throws_for_an_unsupported_type()
        {
            var package = new EPackage(this.resource) { Name = "Pkg" };

            // EFactory derives from EModelElement and is not handled by RemoveFromContainer
            var factory = new EFactory(this.resource) { EContainer = package };

            Assert.That(
                () => factory.RemoveFromContainer(),
                Throws.ArgumentException.With.Message.Contains(typeof(EFactory).ToString()));
        }
    }
}
