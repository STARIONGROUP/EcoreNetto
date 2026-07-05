// ------------------------------------------------------------------------------------------------
// <copyright file="ResourceLifecycleTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests that verify resource lifecycle semantics: <see cref="Resource.Load"/> populates
    /// <see cref="Resource.Contents"/> so a (possibly demand-loaded) resource exposes its root package
    /// (see issue #82), and <see cref="ResourceSet.CreateResource"/> does not register a duplicate for an
    /// already-known URI (see issue #84).
    /// </summary>
    [TestFixture]
    public class ResourceLifecycleTestFixture
    {
        private ResourceSet resourceSet = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
        }

        [Test]
        public void Verify_that_Load_populates_Contents_with_the_root_package()
        {
            var resource = this.CreateResourceForContent(
                "contents.ecore",
                Package("contents", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\"/>"));

            var root = resource.Load(null);

            Assert.Multiple(() =>
            {
                Assert.That(resource.Contents, Has.Count.EqualTo(1));
                Assert.That(resource.Contents[0], Is.SameAs(root));
            });
        }

        [Test]
        public void Verify_that_a_demand_loaded_resource_exposes_its_root_package_via_Contents()
        {
            WriteModel("dep-child.ecore", Package("depchild", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"Base\"/>"));
            var main = this.CreateResourceForContent(
                "dep-main.ecore",
                Package("depmain", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\" eSuperTypes=\"dep-child.ecore#//Base\"/>"));

            main.Load(null);

            // the child was demand-loaded while resolving the cross-file super type; its root package must
            // now be reachable through Contents
            var childUri = new Uri(Path.Combine(TestContext.CurrentContext.TestDirectory, "dep-child.ecore"));
            var child = this.resourceSet.Resource(childUri, false);

            Assert.That(child, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(child!.Contents, Has.Count.EqualTo(1));
                Assert.That(((EPackage)child.Contents[0]).Name, Is.EqualTo("depchild"));
            });
        }

        [Test]
        public void Verify_that_ResourceSet_AllContents_returns_the_contents_of_all_resources()
        {
            var resource = this.CreateResourceForContent(
                "allcontents.ecore",
                Package("allcontents", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\"/>"));
            resource.Load(null);

            var allContents = this.resourceSet.AllContents().ToList();

            Assert.That(allContents, Is.Not.Empty);
            Assert.That(allContents.OfType<EClass>().Any(c => c.Name == "A"), Is.True);
        }

        [Test]
        public void Verify_that_CreateResource_returns_the_existing_resource_for_a_known_uri()
        {
            var uri = new Uri(Path.Combine(TestContext.CurrentContext.TestDirectory, "dup.ecore"));

            var first = this.resourceSet.CreateResource(uri);
            var second = this.resourceSet.CreateResource(uri);

            Assert.Multiple(() =>
            {
                Assert.That(second, Is.SameAs(first));
                Assert.That(this.resourceSet.Resources.Count(r => r.URI.AbsoluteUri == uri.AbsoluteUri), Is.EqualTo(1));
            });
        }

        [Test]
        public void Verify_that_creating_and_loading_an_already_demand_loaded_resource_does_not_corrupt_the_set()
        {
            WriteModel("loop-child.ecore", Package("loopchild", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"Base\"/>"));
            var main = this.CreateResourceForContent(
                "loop-main.ecore",
                Package("loopmain", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\" eSuperTypes=\"loop-child.ecore#//Base\"/>"));

            // loads main and demand-loads loop-child as a dependency
            main.Load(null);

            // the naive "register each file then load it" pattern must not create a duplicate nor throw when
            // it reaches a file that was already demand-loaded (issue #84)
            var childUri = new Uri(Path.Combine(TestContext.CurrentContext.TestDirectory, "loop-child.ecore"));
            Resource child = null!;
            Assert.That(() =>
            {
                child = this.resourceSet.CreateResource(childUri);
                child.Load(null);
            }, Throws.Nothing);

            Assert.Multiple(() =>
            {
                Assert.That(this.resourceSet.Resources.Count(r => r.URI.AbsoluteUri == childUri.AbsoluteUri), Is.EqualTo(1));
                // resolution still works across the set (no duplicate URIs poisoning the lookup)
                Assert.That(main.GetEObject("loop-child.ecore#//Base"), Is.Not.Null);
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

        /// <summary>
        /// Writes the provided <paramref name="content"/> to a file in the test directory.
        /// </summary>
        private static void WriteModel(string fileName, string content)
        {
            File.WriteAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, fileName), content);
        }

        /// <summary>
        /// Writes the provided <paramref name="content"/> to a file in the test directory and
        /// creates a <see cref="Resource"/> for it.
        /// </summary>
        private Resource CreateResourceForContent(string fileName, string content)
        {
            WriteModel(fileName, content);

            var uri = new Uri(Path.Combine(TestContext.CurrentContext.TestDirectory, fileName));

            return this.resourceSet.CreateResource(uri);
        }
    }
}
