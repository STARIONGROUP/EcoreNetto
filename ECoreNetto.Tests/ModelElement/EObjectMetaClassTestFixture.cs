// -------------------------------------------------------------------------------------------------
// <copyright file="EObjectMetaClassTestFixture.cs" company="Starion Group S.A.">
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
    using System.IO;
    using System.Linq;

    using ECoreNetto.Resource;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests that verify <see cref="EObject.EClass"/> returns the Ecore meta class that
    /// describes the runtime type of the model element (see issue #10).
    /// </summary>
    [TestFixture]
    public class EObjectMetaClassTestFixture
    {
        private Resource resource = null!;

        private EPackage rootPackage = null!;

        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");
            var uri = new Uri(Path.GetFullPath(path));

            var resourceSet = new ResourceSet();
            this.resource = resourceSet.CreateResource(uri);
            this.rootPackage = this.resource.Load(null);
        }

        [Test]
        public void Verify_that_EClass_returns_the_expected_meta_class_for_each_element_kind()
        {
            var eClass = this.rootPackage.EClassifiers.OfType<EClass>().First();
            var attribute = this.rootPackage.EClassifiers.OfType<EClass>()
                .SelectMany(c => c.EStructuralFeatures).OfType<EAttribute>().First();
            var reference = this.rootPackage.EClassifiers.OfType<EClass>()
                .SelectMany(c => c.EStructuralFeatures).OfType<EReference>().First();
            var eEnum = this.rootPackage.EClassifiers.OfType<EEnum>().First();
            var literal = this.resource.GetEObject("recipe.ecore#//Unit/PIECE");

            Assert.That(literal, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(this.rootPackage.EClass().Name, Is.EqualTo("EPackage"));
                Assert.That(eClass.EClass().Name, Is.EqualTo("EClass"));
                Assert.That(attribute.EClass().Name, Is.EqualTo("EAttribute"));
                Assert.That(reference.EClass().Name, Is.EqualTo("EReference"));
                Assert.That(eEnum.EClass().Name, Is.EqualTo("EEnum"));
                Assert.That(literal!.EClass().Name, Is.EqualTo("EEnumLiteral"));
            });
        }

        [Test]
        public void Verify_that_the_returned_meta_class_is_the_resource_registered_instance()
        {
            var eClass = this.rootPackage.EClassifiers.OfType<EClass>().First();

            var metaClass = eClass.EClass();

            Assert.Multiple(() =>
            {
                // the meta class is itself an EClass, and is the same instance the resource exposes for "//EClass"
                Assert.That(metaClass, Is.InstanceOf<EClass>());
                Assert.That(metaClass, Is.SameAs(this.resource.GetEObject("//EClass")));
            });
        }

        [Test]
        public void Verify_that_the_meta_class_is_consistent_across_instances_of_the_same_type()
        {
            var attributes = this.rootPackage.EClassifiers.OfType<EClass>()
                .SelectMany(c => c.EStructuralFeatures).OfType<EAttribute>().Take(2).ToList();

            Assert.That(attributes, Has.Count.EqualTo(2));
            Assert.That(attributes[0].EClass(), Is.SameAs(attributes[1].EClass()));
        }

        [Test]
        public void Verify_that_EClass_throws_when_no_meta_class_is_registered_for_the_runtime_type()
        {
            // a runtime type whose name is not present in the Ecore meta model registry
            var unregistered = new UnregisteredEObject(this.resource);

            Assert.That(() => unregistered.EClass(), Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Verify_that_GetMetaClass_returns_null_for_an_unknown_type_name()
        {
            Assert.That(this.resource.GetMetaClass("ThisTypeDoesNotExist"), Is.Null);
        }

        /// <summary>
        /// A minimal <see cref="EObject"/> subtype whose runtime type name has no corresponding Ecore
        /// meta class, used to exercise the guard path of <see cref="EObject.EClass"/>.
        /// </summary>
        private sealed class UnregisteredEObject : EObject
        {
            public UnregisteredEObject(Resource resource)
                : base(resource)
            {
            }

            internal override void SetProperties()
            {
            }
        }
    }
}
