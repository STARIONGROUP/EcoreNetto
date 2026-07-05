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

        private string capellaDirectory = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
            this.capellaDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "capella");
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

        [Test]
        public void Verify_that_loading_the_capella_metamodel_populates_Contents_for_every_resource()
        {
            this.DemandLoadCapellaMetamodel();

            // the 21 cross-referencing Capella files were pulled into the set (some directly, some as
            // demand-loaded dependencies); every one must expose its single root package via Contents (#82)
            Assert.That(this.resourceSet.Resources, Is.Not.Empty);
            Assert.Multiple(() =>
            {
                foreach (var resource in this.resourceSet.Resources)
                {
                    Assert.That(resource.Contents, Has.Count.EqualTo(1), $"resource '{resource.URI}' did not expose its root package");
                    Assert.That(resource.Contents[0], Is.InstanceOf<EPackage>());
                }
            });

            // AllContents flattens every resource's content tree across the whole metamodel
            var allContents = this.resourceSet.AllContents().ToList();
            Assert.That(allContents.OfType<EClass>().Any(), Is.True);
        }

        [Test]
        public void Verify_that_re_registering_and_loading_capella_files_does_not_duplicate_resources()
        {
            // first pass: demand-load the whole metamodel (files reference each other across the set)
            this.DemandLoadCapellaMetamodel();
            var resourceCountAfterFirstPass = this.resourceSet.Resources.Count;

            // second pass: the naive "register each file then load it" loop must reuse the already-loaded
            // resources rather than registering duplicates or re-parsing (and corrupting) the set (#84)
            Assert.That(() =>
            {
                foreach (var file in Directory.EnumerateFiles(this.capellaDirectory, "*.ecore"))
                {
                    var uri = new Uri(Path.GetFullPath(file));
                    var resource = this.resourceSet.CreateResource(uri);
                    resource.Load(null);
                }
            }, Throws.Nothing);

            Assert.Multiple(() =>
            {
                // no new resources were added, and none carry a duplicate URI
                Assert.That(this.resourceSet.Resources.Count, Is.EqualTo(resourceCountAfterFirstPass));
                var duplicateUris = this.resourceSet.Resources
                    .GroupBy(r => r.URI.AbsoluteUri)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
                Assert.That(duplicateUris, Is.Empty, $"duplicate resource URIs: {string.Join("; ", duplicateUris)}");

                // resolution across the set still works after the re-registration pass
                foreach (var resource in this.resourceSet.Resources)
                {
                    Assert.That(resource.Errors, Is.Empty, $"resource '{resource.URI}' recorded errors after re-registration");
                }
            });
        }

        /// <summary>
        /// Demand-loads every <c>.ecore</c> file in the Capella test-data directory into
        /// <see cref="resourceSet"/> using <see cref="ResourceSet.Resource(Uri, bool)"/>, so files already
        /// pulled in through cross-file references are not loaded twice.
        /// </summary>
        private void DemandLoadCapellaMetamodel()
        {
            foreach (var file in Directory.EnumerateFiles(this.capellaDirectory, "*.ecore"))
            {
                var uri = new Uri(Path.GetFullPath(file));
                this.resourceSet.Resource(uri, true);
            }
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
