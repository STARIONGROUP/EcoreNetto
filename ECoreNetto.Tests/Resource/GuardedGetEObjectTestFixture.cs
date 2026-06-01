// -------------------------------------------------------------------------------------------------
// <copyright file="GuardedGetEObjectTestFixture.cs" company="Starion Group S.A.">
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

    using ECoreNetto.Resource;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests that verify the typed, guarded <see cref="Resource.GetEObject{T}"/> resolver and the
    /// unsafe-cast call sites that use it (see issue #29 / #32). Unresolved or wrong-typed references must
    /// yield a clear <see cref="InvalidOperationException"/> that names the offending fragment, instead of a
    /// vague <see cref="NullReferenceException"/>, <see cref="InvalidCastException"/> or stack overflow.
    /// </summary>
    [TestFixture]
    public class GuardedGetEObjectTestFixture
    {
        private ResourceSet resourceSet = null!;
        private Resource resource = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
            this.resource = new Resource { ResourceSet = this.resourceSet };
            this.resourceSet.Resources.Add(this.resource);
        }

        [Test]
        public void Verify_that_a_cached_object_of_the_expected_type_is_returned()
        {
            var eClass = new EClass(this.resource);
            this.resource.Cache.Add("a-fragment", eClass);

            Assert.That(this.resource.GetEObject<EClass>("a-fragment"), Is.SameAs(eClass));
        }

        [Test]
        public void Verify_that_resolving_an_EClass_reference_to_a_wrong_type_throws_a_descriptive_exception()
        {
            // an EDataType is an EClassifier but not an EClass; this is what an EClass.eSuperTypes
            // pointing at a datatype would yield
            this.resource.Cache.Add("super-type", new EDataType(this.resource));

            var exception = Assert.Throws<InvalidOperationException>(
                () => this.resource.GetEObject<EClass>("super-type"));
            var message = exception!.Message;

            Assert.Multiple(() =>
            {
                Assert.That(message, Does.Contain("super-type"));
                Assert.That(message, Does.Contain(nameof(EClass)));
                Assert.That(message, Does.Contain(nameof(EDataType)));
            });
        }

        [Test]
        public void Verify_that_resolving_an_EClassifier_reference_to_a_wrong_type_throws_a_descriptive_exception()
        {
            // an EReference is not an EClassifier; this is what an eType pointing at a feature would yield
            this.resource.Cache.Add("the-type", new EReference(this.resource));

            var exception = Assert.Throws<InvalidOperationException>(
                () => this.resource.GetEObject<EClassifier>("the-type"));
            var message = exception!.Message;

            Assert.Multiple(() =>
            {
                Assert.That(message, Does.Contain("the-type"));
                Assert.That(message, Does.Contain(nameof(EClassifier)));
            });
        }

        [Test]
        public void Verify_that_resolving_an_EReference_reference_to_a_wrong_type_throws_a_descriptive_exception()
        {
            // an EAttribute is an EStructuralFeature but not an EReference; this is what an eOpposite
            // pointing at an attribute would yield
            this.resource.Cache.Add("the-opposite", new EAttribute(this.resource));

            var exception = Assert.Throws<InvalidOperationException>(
                () => this.resource.GetEObject<EReference>("the-opposite"));
            var message = exception!.Message;

            Assert.Multiple(() =>
            {
                Assert.That(message, Does.Contain("the-opposite"));
                Assert.That(message, Does.Contain(nameof(EReference)));
            });
        }

        [Test]
        public void Verify_that_resolving_an_unresolvable_fragment_throws_a_descriptive_exception()
        {
            // the fragment is not in the cache, is not a known ECore type, and does not point at another
            // .ecore resource: the base GetEObject returns null and the typed resolver reports it clearly
            var exception = Assert.Throws<InvalidOperationException>(
                () => this.resource.GetEObject<EClass>("does-not-exist"));
            var message = exception!.Message;

            Assert.Multiple(() =>
            {
                Assert.That(message, Does.Contain("does-not-exist"));
                Assert.That(message, Does.Contain("null (unresolved)"));
            });
        }

        [Test]
        public void Verify_that_the_base_resolver_returns_null_for_an_unresolvable_fragment()
        {
            Assert.That(this.resource.GetEObject("does-not-exist"), Is.Null);
        }

        [Test]
        public void Verify_that_an_unresolved_eSuperTypes_reference_is_reported_when_loading()
        {
            // EClass.SetProperties resolves eSuperTypes via GetEObject<EClass>
            const string model =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                "<ecore:EPackage xmi:version=\"2.0\" xmlns:xmi=\"http://www.omg.org/XMI\" " +
                "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                "xmlns:ecore=\"http://www.eclipse.org/emf/2002/Ecore\" name=\"superpkg\" nsURI=\"superpkg\" nsPrefix=\"superpkg\">\r\n" +
                "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"A\" eSuperTypes=\"#//DoesNotExist\"/>\r\n" +
                "</ecore:EPackage>";

            // the file name must match the root package name so the implicit '#//' reference,
            // which is rewritten to '<package>.ecore#//...', resolves back to this resource
            var resource = this.CreateResourceForContent("superpkg.ecore", model);

            var exception = Assert.Throws<InvalidOperationException>(() => resource.Load(null));
            Assert.That(exception!.Message, Does.Contain("DoesNotExist"));
        }

        [Test]
        public void Verify_that_an_unresolved_eType_reference_is_reported_when_loading()
        {
            // ETypedElement.SetProperties resolves eType via GetEObject<EClassifier>
            const string model =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                "<ecore:EPackage xmi:version=\"2.0\" xmlns:xmi=\"http://www.omg.org/XMI\" " +
                "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                "xmlns:ecore=\"http://www.eclipse.org/emf/2002/Ecore\" name=\"typepkg\" nsURI=\"typepkg\" nsPrefix=\"typepkg\">\r\n" +
                "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"A\">\r\n" +
                "    <eStructuralFeatures xsi:type=\"ecore:EAttribute\" name=\"x\" eType=\"#//Missing\"/>\r\n" +
                "  </eClassifiers>\r\n" +
                "</ecore:EPackage>";

            // the file name must match the root package name so the implicit '#//' reference,
            // which is rewritten to '<package>.ecore#//...', resolves back to this resource
            var resource = this.CreateResourceForContent("typepkg.ecore", model);

            var exception = Assert.Throws<InvalidOperationException>(() => resource.Load(null));
            Assert.That(exception!.Message, Does.Contain("Missing"));
        }

        /// <summary>
        /// Writes the provided <paramref name="content"/> to a file in the test directory and
        /// creates a <see cref="Resource"/> for it.
        /// </summary>
        private Resource CreateResourceForContent(string fileName, string content)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);
            File.WriteAllText(path, content);

            var uri = new Uri(Path.GetFullPath(path));

            return this.resourceSet.CreateResource(uri);
        }
    }
}
