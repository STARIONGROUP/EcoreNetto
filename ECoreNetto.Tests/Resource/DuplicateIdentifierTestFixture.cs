// ------------------------------------------------------------------------------------------------
// <copyright file="DuplicateIdentifierTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests that verify duplicate-named siblings (e.g. overloaded <see cref="EOperation"/>s,
    /// which Ecore permits) load with distinct, EMF-compatible identifiers rather than crashing on a
    /// duplicate cache key (see issue #96). EMF disambiguates by appending a positional <c>.N</c> suffix
    /// to later occurrences; EcoreNetto now does the same when registering elements in the resource cache.
    /// </summary>
    [TestFixture]
    public class DuplicateIdentifierTestFixture
    {
        private ResourceSet resourceSet = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
        }

        [Test]
        public void Verify_that_overloaded_operations_load_with_distinct_identifiers()
        {
            var resource = this.CreateResourceForContent("overloads.ecore", OverloadedModel());

            EPackage root = null!;
            Assert.That(() => root = resource.Load(null), Throws.Nothing);

            var operations = root.EClassifiers
                .OfType<EClass>()
                .Single(c => c.Name == "EClass")
                .EOperations
                .Where(o => o.Name == "getEStructuralFeature")
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(operations, Has.Count.EqualTo(2), "both overloads should be present");
                Assert.That(operations[0].Identifier, Is.Not.EqualTo(operations[1].Identifier));
                Assert.That(operations[1].Identifier, Does.EndWith(".1"));
                Assert.That(resource.Errors, Is.Empty);
            });
        }

        [Test]
        public void Verify_that_a_duplicate_operation_resolves_by_its_disambiguated_identifier()
        {
            var resource = this.CreateResourceForContent("overloads-resolve.ecore", OverloadedModel());
            var root = resource.Load(null);

            var operations = root.EClassifiers
                .OfType<EClass>()
                .Single(c => c.Name == "EClass")
                .EOperations
                .Where(o => o.Name == "getEStructuralFeature")
                .ToList();

            var first = operations[0];
            var second = operations[1];

            Assert.Multiple(() =>
            {
                // each overload round-trips through the cache by its own (disambiguated) identifier
                Assert.That(resource.GetEObject(first.Identifier), Is.SameAs(first));
                Assert.That(resource.GetEObject(second.Identifier), Is.SameAs(second));
                Assert.That(second.Identifier, Is.EqualTo($"{first.Identifier}.1"));
            });
        }

        [Test]
        public void Verify_that_duplicate_operations_parameters_get_distinct_identifiers()
        {
            var resource = this.CreateResourceForContent("overloads-params.ecore", OverloadedModel());
            var root = resource.Load(null);

            var operations = root.EClassifiers
                .OfType<EClass>()
                .Single(c => c.Name == "EClass")
                .EOperations
                .Where(o => o.Name == "getEStructuralFeature")
                .ToList();

            var firstParameter = operations[0].EParameters.Single();
            var secondParameter = operations[1].EParameters.Single();

            // the parameters share a simple name but belong to different (disambiguated) operations,
            // so the parent suffix propagates and their identifiers differ
            Assert.That(firstParameter.Identifier, Is.Not.EqualTo(secondParameter.Identifier));
        }

        /// <summary>
        /// A minimal Ecore package with an <c>EClass</c> that declares two <c>getEStructuralFeature</c>
        /// operations (the <c>featureID</c> and <c>featureName</c> overloads, as in the real Ecore metamodel).
        /// </summary>
        private static string OverloadedModel()
        {
            return Package(
                "overloads",
                "<eClassifiers xsi:type=\"ecore:EClass\" name=\"EClass\">" +
                "<eOperations name=\"getEStructuralFeature\">" +
                "<eParameters name=\"featureID\"/>" +
                "</eOperations>" +
                "<eOperations name=\"getEStructuralFeature\">" +
                "<eParameters name=\"featureName\"/>" +
                "</eOperations>" +
                "</eClassifiers>");
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
        /// Writes the provided <paramref name="content"/> to a file in the test directory and
        /// creates a <see cref="Resource"/> for it.
        /// </summary>
        private Resource CreateResourceForContent(string fileName, string content)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);
            File.WriteAllText(path, content);

            return this.resourceSet.CreateResource(new Uri(path));
        }
    }
}
