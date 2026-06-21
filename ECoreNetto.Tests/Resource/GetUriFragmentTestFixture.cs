// -------------------------------------------------------------------------------------------------
// <copyright file="GetUriFragmentTestFixture.cs" company="Starion Group S.A.">
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

namespace ECoreNetto.Tests.Resource
{
    using System;
    using System.IO;
    using System.Linq;

    using ECoreNetto.Resource;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests that verify <see cref="Resource.GetURIFragment"/> is the inverse of
    /// <see cref="Resource.GetEObject(string)"/> and is appropriately guarded (see issue #76).
    /// </summary>
    [TestFixture]
    public class GetUriFragmentTestFixture
    {
        private ResourceSet resourceSet = null!;

        private Resource resource = null!;

        private EPackage rootPackage = null!;

        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");
            var uri = new Uri(Path.GetFullPath(path));

            this.resourceSet = new ResourceSet();
            this.resource = this.resourceSet.CreateResource(uri);
            this.rootPackage = this.resource.Load(null);
        }

        [Test]
        public void Verify_that_the_root_package_round_trips()
        {
            var fragment = this.resource.GetURIFragment(this.rootPackage);

            Assert.That(this.resource.GetEObject(fragment), Is.SameAs(this.rootPackage));
        }

        [Test]
        public void Verify_that_a_class_round_trips()
        {
            var eClass = this.rootPackage.EClassifiers.OfType<EClass>().First();

            var fragment = this.resource.GetURIFragment(eClass);

            Assert.Multiple(() =>
            {
                Assert.That(fragment, Is.EqualTo(eClass.Identifier));
                Assert.That(this.resource.GetEObject(fragment), Is.SameAs(eClass));
            });
        }

        [Test]
        public void Verify_that_a_structural_feature_round_trips()
        {
            var feature = this.rootPackage.EClassifiers
                .OfType<EClass>()
                .SelectMany(eClass => eClass.EStructuralFeatures)
                .First();

            var fragment = this.resource.GetURIFragment(feature);

            Assert.That(this.resource.GetEObject(fragment), Is.SameAs(feature));
        }

        [Test]
        public void Verify_that_an_enum_literal_round_trips()
        {
            // the recipe model defines the 'Unit' enumeration with a 'PIECE' literal
            var literal = this.resource.GetEObject("recipe.ecore#//Unit/PIECE");
            Assert.That(literal, Is.Not.Null);

            var fragment = this.resource.GetURIFragment(literal!);

            Assert.That(this.resource.GetEObject(fragment), Is.SameAs(literal));
        }

        [Test]
        public void Verify_that_a_null_object_throws()
        {
            Assert.That(() => this.resource.GetURIFragment(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_an_object_not_contained_in_the_resource_throws()
        {
            var eClass = this.rootPackage.EClassifiers.OfType<EClass>().First();

            // a different resource does not contain the object, so a fragment cannot be produced
            var otherResource = new Resource();

            Assert.That(() => otherResource.GetURIFragment(eClass), Throws.TypeOf<InvalidOperationException>());
        }
    }
}
