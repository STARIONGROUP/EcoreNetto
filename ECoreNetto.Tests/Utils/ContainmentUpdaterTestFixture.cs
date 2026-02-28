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
    using System;

    using ECoreNetto.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class ContainmentUpdaterTestFixture
    {
        [Test]
        public void Verify_that_RemoveFromContainer_throws_for_null_subject()
        {
            Assert.That(() => ContainmentUpdater.RemoveFromContainer(null), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_RemoveFromContainer_returns_when_subject_has_no_container()
        {
            var resource = new Resource.Resource();
            var subject = new EClass(resource);

            Assert.That(() => subject.RemoveFromContainer(), Throws.Nothing);
        }

        [Test]
        public void Verify_that_RemoveFromContainer_removes_supported_types_from_their_parent_collections()
        {
            var resource = new Resource.Resource();
            var parentPackage = new EPackage(resource)
            {
                Name = "Parent"
            };

            var subPackage = new EPackage(resource)
            {
                Name = "Child"
            };

            var classifier = new EClass(resource)
            {
                Name = "Classifier"
            };

            var annotation = new EAnnotation(resource);
            var operation = new EOperation(resource)
            {
                Name = "Operation"
            };

            var parameter = new EParameter(resource)
            {
                Name = "Parameter"
            };

            var attribute = new EAttribute(resource)
            {
                Name = "Attribute"
            };

            var eEnum = new EEnum(resource)
            {
                Name = "Enum"
            };

            var literal = new EEnumLiteral(resource)
            {
                Name = "Literal"
            };

            parentPackage.ESubPackages.Add(subPackage);
            parentPackage.EClassifiers.Add(classifier);
            classifier.EAnnotations.Add(annotation);
            classifier.EOperations.Add(operation);
            operation.EParameters.Add(parameter);
            classifier.EStructuralFeatures.Add(attribute);
            eEnum.ELiterals.Add(literal);

            subPackage.RemoveFromContainer();
            classifier.RemoveFromContainer();
            annotation.RemoveFromContainer();
            operation.RemoveFromContainer();
            parameter.RemoveFromContainer();
            attribute.RemoveFromContainer();
            literal.RemoveFromContainer();

            Assert.Multiple(() =>
            {
                Assert.That(parentPackage.ESubPackages, Is.Empty);
                Assert.That(parentPackage.EClassifiers, Is.Empty);
                Assert.That(((EClass)classifier).EAnnotations, Is.Empty);
                Assert.That(((EClass)classifier).EOperations, Is.Empty);
                Assert.That(operation.EParameters, Is.Empty);
                Assert.That(((EClass)classifier).EStructuralFeatures, Is.Empty);
                Assert.That(eEnum.ELiterals, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_RemoveFromContainer_throws_for_unsupported_type()
        {
            var resource = new Resource.Resource();
            var unsupported = new TestEObject(resource)
            {
                EContainer = new EPackage(resource)
            };

            Assert.That(() => unsupported.RemoveFromContainer(), Throws.TypeOf<ArgumentException>());
        }

        private class TestEObject : EObject
        {
            public TestEObject(Resource.Resource resource)
                : base(resource)
            {
            }
        }
    }
}
