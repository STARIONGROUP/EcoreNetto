// -------------------------------------------------------------------------------------------------
// <copyright file="ContainerListTestFixture.cs" company="Starion Group S.A.">
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

namespace ECoreNetto.Tests.ModelElement
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class ContainerListTestFixture
    {
        [Test]
        public void Verify_that_Add_sets_container_and_reparents_element()
        {
            var resource = new Resource.Resource();
            var sourceContainer = new EPackage(resource);
            var targetContainer = new EPackage(resource);
            var subject = new EPackage(resource)
            {
                Name = "MovedPackage"
            };

            sourceContainer.ESubPackages.Add(subject);

            targetContainer.ESubPackages.Add(subject);

            Assert.Multiple(() =>
            {
                Assert.That(sourceContainer.ESubPackages, Has.Count.EqualTo(0));
                Assert.That(targetContainer.ESubPackages, Has.Count.EqualTo(1));
                Assert.That(targetContainer.ESubPackages[0], Is.SameAs(subject));
                Assert.That(subject.EContainer, Is.SameAs(targetContainer));
            });
        }

        [Test]
        public void Verify_that_Add_throws_when_item_is_null_or_duplicate()
        {
            var resource = new Resource.Resource();
            var container = new EPackage(resource);
            var subject = new EPackage(resource)
            {
                Name = "SubPackage"
            };

            Assert.That(() => container.ESubPackages.Add(null), Throws.ArgumentNullException);

            container.ESubPackages.Add(subject);

            Assert.That(() => container.ESubPackages.Add(subject), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Verify_that_AddRange_throws_for_null_and_assigns_container_to_all_items()
        {
            var resource = new Resource.Resource();
            var container = new EPackage(resource);

            Assert.That(() => container.EClassifiers.AddRange(null), Throws.ArgumentNullException);

            var classA = new EClass(resource)
            {
                Name = "A"
            };

            var classB = new EClass(resource)
            {
                Name = "B"
            };

            container.EClassifiers.AddRange(new[] { classA, classB });

            Assert.Multiple(() =>
            {
                Assert.That(container.EClassifiers, Has.Count.EqualTo(2));
                Assert.That(classA.EContainer, Is.SameAs(container));
                Assert.That(classB.EContainer, Is.SameAs(container));
            });
        }

        [Test]
        public void Verify_that_indexer_setter_validates_input_and_reparents_item()
        {
            var resource = new Resource.Resource();
            var sourceContainer = new EPackage(resource);
            var targetContainer = new EPackage(resource);

            var first = new EClass(resource)
            {
                Name = "First"
            };

            var second = new EClass(resource)
            {
                Name = "Second"
            };

            var replacement = new EClass(resource)
            {
                Name = "Replacement"
            };

            sourceContainer.EClassifiers.Add(replacement);
            targetContainer.EClassifiers.AddRange(new[] { first, second });

            Assert.That(() => targetContainer.EClassifiers[-1], Throws.ArgumentOutOfRangeException);
            Assert.That(() => targetContainer.EClassifiers[2], Throws.ArgumentOutOfRangeException);
            Assert.That(() => targetContainer.EClassifiers[0] = null, Throws.ArgumentNullException);
            Assert.That(() => targetContainer.EClassifiers[0] = second, Throws.TypeOf<InvalidOperationException>());

            targetContainer.EClassifiers[0] = replacement;

            Assert.Multiple(() =>
            {
                Assert.That(sourceContainer.EClassifiers, Has.Count.EqualTo(0));
                Assert.That(targetContainer.EClassifiers[0], Is.SameAs(replacement));
                Assert.That(replacement.EContainer, Is.SameAs(targetContainer));
            });
        }
    }
}
