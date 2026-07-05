// ------------------------------------------------------------------------------------------------
// <copyright file="CapellaMetamodelTestFixture.cs" company="Starion Group S.A.">
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
    /// Integration tests that load the full Eclipse Capella metamodel (21 cross-referencing <c>.ecore</c>
    /// files in one <see cref="ResourceSet"/>) and verify that cross-file references resolve even when a
    /// file name differs from its root package name (see issue #79). 17 of the 21 files have
    /// <c>fileName != rootPackageName</c> (e.g. <c>CompositeStructure.ecore</c> / package <c>cs</c>).
    /// </summary>
    [TestFixture]
    public class CapellaMetamodelTestFixture
    {
        private string capellaDirectory = null!;
        private ResourceSet resourceSet = null!;

        [SetUp]
        public void SetUp()
        {
            this.capellaDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "capella");
            this.resourceSet = new ResourceSet();
        }

        [Test]
        public void Verify_that_the_entire_capella_metamodel_loads_without_errors()
        {
            Assert.DoesNotThrow(() => this.LoadCapellaMetamodel());

            // every reference across the 21 files must have resolved: no resource may record an error
            Assert.Multiple(() =>
            {
                foreach (var resource in this.resourceSet.Resources)
                {
                    Assert.That(
                        resource.Errors,
                        Is.Empty,
                        $"resource '{resource.URI}' recorded errors: {string.Join("; ", resource.Errors.Select(e => e.Message))}");
                }
            });
        }

        [Test]
        public void Verify_that_a_cross_file_supertype_resolves_when_file_name_differs_from_package_name()
        {
            this.LoadCapellaMetamodel();

            // LogicalArchitecture.ecore (package 'la') declares:
            //   <eClassifiers name="LogicalArchitecture" eSuperTypes="CompositeStructure.ecore#//ComponentArchitecture"/>
            // The super type lives in CompositeStructure.ecore, whose ROOT PACKAGE is named 'cs' (file != package).
            var logicalArchitectureUri = new Uri(Path.Combine(this.capellaDirectory, "LogicalArchitecture.ecore"));
            var logicalArchitectureResource = this.resourceSet.Resource(logicalArchitectureUri, false);
            Assert.That(logicalArchitectureResource, Is.Not.Null);

            var logicalArchitecture = logicalArchitectureResource!.AllContents()
                .OfType<EClass>()
                .Single(c => c.Name == "LogicalArchitecture");

            var componentArchitecture = logicalArchitecture.ESuperTypes
                .SingleOrDefault(s => s.Name == "ComponentArchitecture");

            Assert.That(componentArchitecture, Is.Not.Null, "the cross-file super type did not resolve");
            Assert.Multiple(() =>
            {
                // resolved into CompositeStructure.ecore, whose root package name ('cs') differs from the file name
                Assert.That(componentArchitecture!.EPackage.Name, Is.EqualTo("cs"));
                Assert.That(
                    Path.GetFileName(componentArchitecture.EResource.URI.LocalPath),
                    Is.EqualTo("CompositeStructure.ecore"));
            });
        }

        [Test]
        public void Verify_that_a_cross_file_eOpposite_resolves_across_capella_files()
        {
            this.LoadCapellaMetamodel();

            // LogicalArchitecture.ecore (package 'la') declares on class LogicalArchitecture:
            //   <eStructuralFeatures name="allocatedSystemAnalyses"
            //       eOpposite="ContextArchitecture.ecore#//SystemAnalysis/allocatingLogicalArchitectures"/>
            // The opposite feature lives in ContextArchitecture.ecore (a different file).
            var logicalArchitectureUri = new Uri(Path.Combine(this.capellaDirectory, "LogicalArchitecture.ecore"));
            var logicalArchitectureResource = this.resourceSet.Resource(logicalArchitectureUri, false);
            Assert.That(logicalArchitectureResource, Is.Not.Null);

            var allocatedSystemAnalyses = logicalArchitectureResource!.AllContents()
                .OfType<EReference>()
                .Single(r => r.Name == "allocatedSystemAnalyses" && r.EContainingClass.Name == "LogicalArchitecture");

            Assert.That(allocatedSystemAnalyses.EOpposite, Is.Not.Null, "the cross-file eOpposite did not resolve");
            Assert.Multiple(() =>
            {
                Assert.That(allocatedSystemAnalyses.EOpposite!.Name, Is.EqualTo("allocatingLogicalArchitectures"));
                Assert.That(allocatedSystemAnalyses.EOpposite!.EContainingClass.Name, Is.EqualTo("SystemAnalysis"));
                // resolved into ContextArchitecture.ecore, a different file than the referring feature
                Assert.That(
                    Path.GetFileName(allocatedSystemAnalyses.EOpposite!.EResource.URI.LocalPath),
                    Is.EqualTo("ContextArchitecture.ecore"));
                // the opposite relation is symmetric
                Assert.That(allocatedSystemAnalyses.EOpposite!.EOpposite, Is.SameAs(allocatedSystemAnalyses));
            });
        }

        /// <summary>
        /// Loads every <c>.ecore</c> file in the Capella test-data directory into <see cref="resourceSet"/>.
        /// Uses the demand-loading <see cref="ResourceSet.Resource(Uri, bool)"/> so files already pulled in
        /// through cross-file references are not loaded twice.
        /// </summary>
        private void LoadCapellaMetamodel()
        {
            foreach (var file in Directory.EnumerateFiles(this.capellaDirectory, "*.ecore"))
            {
                var uri = new Uri(Path.GetFullPath(file));
                this.resourceSet.Resource(uri, true);
            }
        }
    }
}
