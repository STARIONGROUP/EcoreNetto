// ------------------------------------------------------------------------------------------------
// <copyright file="EscapedPathTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests that verify models load and cross-resource references resolve when the model path
    /// contains characters that are percent-escaped in a URI beyond a plain space (%20), such as a space
    /// and a non-ASCII character (see issue #38 — reference handling must use proper URI decoding).
    /// </summary>
    [TestFixture]
    public class EscapedPathTestFixture
    {
        // a directory name with a space (%20) and a non-ASCII character (escapes to %C3%A4) exercises
        // escaped path segments beyond %20
        private string escapedDirectory = null!;

        [SetUp]
        public void SetUp()
        {
            this.escapedDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "escaped path ä");
            Directory.CreateDirectory(this.escapedDirectory);
        }

        [Test]
        public void Verify_that_a_model_in_an_escaped_path_can_be_loaded()
        {
            var path = Path.Combine(this.escapedDirectory, "single.ecore");
            File.WriteAllText(path, Package("single", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\"/>"));

            var resourceSet = new ResourceSet();
            var resource = resourceSet.CreateResource(new Uri(Path.GetFullPath(path)));

            EPackage root = null!;
            Assert.That(() => root = resource.Load(null), Throws.Nothing);
            Assert.Multiple(() =>
            {
                Assert.That(root.Name, Is.EqualTo("single"));
                Assert.That(resource.Errors, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_a_cross_resource_reference_resolves_across_an_escaped_path()
        {
            var childPath = Path.Combine(this.escapedDirectory, "child.ecore");
            File.WriteAllText(childPath, Package("child", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"Base\"/>"));

            var mainPath = Path.Combine(this.escapedDirectory, "main.ecore");
            File.WriteAllText(
                mainPath,
                Package("main", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\" eSuperTypes=\"child.ecore#//Base\"/>"));

            var resourceSet = new ResourceSet();
            var resource = resourceSet.CreateResource(new Uri(Path.GetFullPath(mainPath)));

            EPackage root = null!;
            Assert.That(() => root = resource.Load(null), Throws.Nothing);

            var a = root.EClassifiers.OfType<EClass>().Single(c => c.Name == "A");
            var superType = a.ESuperTypes.SingleOrDefault();

            Assert.Multiple(() =>
            {
                Assert.That(superType, Is.Not.Null);
                Assert.That(superType!.Name, Is.EqualTo("Base"));
                Assert.That(resource.Errors, Is.Empty);
            });
        }

        /// <summary>
        /// Wraps the supplied classifier markup in a minimal Ecore package.
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
    }
}
