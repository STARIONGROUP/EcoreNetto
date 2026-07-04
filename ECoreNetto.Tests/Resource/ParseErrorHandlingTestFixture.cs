// ------------------------------------------------------------------------------------------------
// <copyright file="ParseErrorHandlingTestFixture.cs" company="Starion Group S.A.">
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
    using System.Xml;

    using ECoreNetto.Resource;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests that verify the <see cref="ECoreNetto.ECoreParser"/> surfaces a clear, documented
    /// error and records a <see cref="Diagnostic"/> in <see cref="Resource.Errors"/> when the file is
    /// missing or the XML is malformed (see issue #36).
    /// </summary>
    [TestFixture]
    public class ParseErrorHandlingTestFixture
    {
        private ResourceSet resourceSet = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
        }

        [Test]
        public void Verify_that_a_missing_file_throws_FileNotFoundException_and_records_an_error()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "does-not-exist.ecore");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var resource = this.resourceSet.CreateResource(new Uri(Path.GetFullPath(path)));

            Assert.Throws<FileNotFoundException>(() => resource.Load(null));

            var error = resource.Errors.SingleOrDefault();
            Assert.That(error, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(error!.Message, Does.Contain("could not be found"));
                Assert.That(error.Message, Does.Contain("does-not-exist.ecore"));
                Assert.That(error.Location, Is.EqualTo(resource.URI.AbsoluteUri));
            });
        }

        [Test]
        public void Verify_that_malformed_xml_throws_XmlException_and_records_an_error()
        {
            // an unclosed element renders the document not well-formed
            var resource = this.CreateResourceForContent(
                "malformed-xml.ecore",
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<ecore:EPackage><eClassifiers");

            Assert.That(() => resource.Load(null), Throws.InstanceOf<XmlException>());

            var error = resource.Errors.SingleOrDefault();
            Assert.That(error, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(error!.Message, Does.Contain("not well-formed"));
                Assert.That(error.Location, Is.EqualTo(resource.URI.AbsoluteUri));
            });
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
