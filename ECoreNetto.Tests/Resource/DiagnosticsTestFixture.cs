// ------------------------------------------------------------------------------------------------
// <copyright file="DiagnosticsTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
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
    /// Suite of tests that verify the <see cref="ECoreNetto.ECoreParser"/> records a <see cref="Diagnostic"/>
    /// in <see cref="Resource.Errors"/> for malformed or unrecognized input before aborting the load (see issue #35).
    /// </summary>
    [TestFixture]
    public class DiagnosticsTestFixture
    {
        private ResourceSet resourceSet = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
        }

        [Test]
        public void Verify_that_a_malformed_boolean_attribute_is_recorded_as_an_error_and_aborts_the_load()
        {
            var model = Package("malformed-bool", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\" abstract=\"notabool\"/>");
            var resource = this.CreateResourceForContent("malformed-bool.ecore", model);

            Assert.Throws<FormatException>(() => resource.Load(null));

            var error = resource.Errors.SingleOrDefault();
            Assert.That(error, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(error!.Message, Does.Contain("abstract"));
                Assert.That(error.Message, Does.Contain("notabool"));
                Assert.That(error.Location, Is.EqualTo(resource.URI.AbsoluteUri));
            });
        }

        [Test]
        public void Verify_that_a_malformed_integer_attribute_is_recorded_as_an_error_and_aborts_the_load()
        {
            var model = Package(
                "malformed-int",
                "<eClassifiers xsi:type=\"ecore:EEnum\" name=\"E\">\r\n" +
                "    <eLiterals name=\"X\" value=\"notanint\"/>\r\n" +
                "  </eClassifiers>");
            var resource = this.CreateResourceForContent("malformed-int.ecore", model);

            Assert.Throws<FormatException>(() => resource.Load(null));

            var error = resource.Errors.SingleOrDefault();
            Assert.That(error, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(error!.Message, Does.Contain("value"));
                Assert.That(error.Message, Does.Contain("notanint"));
            });
        }

        [Test]
        public void Verify_that_a_reference_to_a_missing_cross_resource_uri_is_recorded_as_an_error_and_aborts_the_load()
        {
            // the eType points at another '.ecore' resource that does not exist; the malformed/unresolvable
            // URI must be recorded as a descriptive diagnostic instead of throwing a raw FileNotFoundException,
            // and the typed resolver then reports the unresolved reference
            var model = Package(
                "missing-uri",
                "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\">\r\n" +
                "    <eStructuralFeatures xsi:type=\"ecore:EAttribute\" name=\"x\" eType=\"nonexistent.ecore#//X\"/>\r\n" +
                "  </eClassifiers>");
            var resource = this.CreateResourceForContent("missing-uri.ecore", model);

            Assert.Throws<InvalidOperationException>(() => resource.Load(null));

            var error = resource.Errors.SingleOrDefault();
            Assert.That(error, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(error!.Message, Does.Contain("nonexistent.ecore"));
                Assert.That(error.Location, Is.EqualTo(resource.URI.AbsoluteUri));
            });
        }

        [Test]
        public void Verify_that_a_reference_to_an_existing_cross_resource_is_followed_without_recording_a_missing_resource_error()
        {
            // write the referenced resource to disk but do NOT pre-register it in the resource set, so the
            // loader has to follow the reference (the matching resource is null and the file exists)
            var referencedModel = Package("xref-other", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"T\"/>");
            File.WriteAllText(
                Path.Combine(TestContext.CurrentContext.TestDirectory, "xref-other.ecore"), referencedModel);

            var host = new Resource
            {
                ResourceSet = this.resourceSet,
                URI = new Uri(Path.Combine(TestContext.CurrentContext.TestDirectory, "xref-host.ecore"))
            };
            this.resourceSet.Resources.Add(host);

            host.GetEObject("xref-other.ecore#//T");

            // the referenced resource exists, so no missing-resource diagnostic must be recorded
            Assert.That(host.Errors, Is.Empty);
        }

        [Test]
        public void Verify_that_an_unrecognized_classifier_type_is_recorded_as_an_error_and_aborts_the_load()
        {
            var model = Package("unknown-classifier", "<eClassifiers xsi:type=\"ecore:Bogus\" name=\"A\"/>");
            var resource = this.CreateResourceForContent("unknown-classifier.ecore", model);

            Assert.Throws<InvalidOperationException>(() => resource.Load(null));

            var error = resource.Errors.SingleOrDefault();
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Message, Does.Contain("Bogus"));
        }

        [Test]
        public void Verify_that_an_unrecognized_structural_feature_type_is_recorded_as_an_error_and_aborts_the_load()
        {
            var model = Package(
                "unknown-feature",
                "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\">\r\n" +
                "    <eStructuralFeatures xsi:type=\"ecore:Bogus\" name=\"f\"/>\r\n" +
                "  </eClassifiers>");
            var resource = this.CreateResourceForContent("unknown-feature.ecore", model);

            Assert.Throws<InvalidOperationException>(() => resource.Load(null));

            var error = resource.Errors.SingleOrDefault();
            Assert.That(error, Is.Not.Null);
            Assert.That(error!.Message, Does.Contain("Bogus"));
        }

        /// <summary>
        /// Wraps the supplied classifier markup in a minimal Ecore package whose name matches
        /// <paramref name="packageName"/>.
        /// </summary>
        private static string Package(string packageName, string body)
        {
            return
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                "<ecore:EPackage xmi:version=\"2.0\" xmlns:xmi=\"http://www.omg.org/XMI\" " +
                "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                "xmlns:ecore=\"http://www.eclipse.org/emf/2002/Ecore\" " +
                $"name=\"{packageName}\" nsURI=\"{packageName}\" nsPrefix=\"{packageName}\">\r\n" +
                $"  {body}\r\n" +
                "</ecore:EPackage>";
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
