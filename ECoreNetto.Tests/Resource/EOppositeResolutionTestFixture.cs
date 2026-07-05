// ------------------------------------------------------------------------------------------------
// <copyright file="EOppositeResolutionTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests that verify <c>eOpposite</c> references resolve to the correct <see cref="EReference"/>
    /// regardless of parse order and across files (see issue #81). The crash described in that issue was
    /// resolved by the file-name resource segment (#79) and the resolution-cycle guard (#80); these tests
    /// positively assert that same-file (both directions), cross-file, and forward opposites resolve.
    /// </summary>
    [TestFixture]
    public class EOppositeResolutionTestFixture
    {
        private ResourceSet resourceSet = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
        }

        [Test]
        public void Verify_that_same_file_opposites_resolve_in_both_directions()
        {
            // class A (and its reference toB, whose opposite points at B/toA) is declared BEFORE class B,
            // so toB -> B/toA is a forward reference at the point A is parsed
            var body =
                "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\">" +
                "<eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"toB\" eType=\"#//B\" eOpposite=\"#//B/toA\"/>" +
                "</eClassifiers>" +
                "<eClassifiers xsi:type=\"ecore:EClass\" name=\"B\">" +
                "<eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"toA\" eType=\"#//A\" eOpposite=\"#//A/toB\"/>" +
                "</eClassifiers>";

            var resource = this.CreateResourceForContent("same-file-opposite.ecore", Package("samefileopposite", body));
            var root = resource.Load(null);

            var toB = Reference(root, "A", "toB");
            var toA = Reference(root, "B", "toA");

            Assert.Multiple(() =>
            {
                Assert.That(toB.EOpposite, Is.SameAs(toA));
                Assert.That(toA.EOpposite, Is.SameAs(toB));
                // the opposite relation is symmetric
                Assert.That(toB.EOpposite!.EOpposite, Is.SameAs(toB));
            });
        }

        [Test]
        public void Verify_that_a_cross_file_opposite_resolves_to_the_reference_in_the_sibling_resource()
        {
            WriteModel(
                "cross-child.ecore",
                Package(
                    "crosschild",
                    "<eClassifiers xsi:type=\"ecore:EClass\" name=\"B\">" +
                    "<eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"toA\" eType=\"cross-main.ecore#//A\" eOpposite=\"cross-main.ecore#//A/toB\"/>" +
                    "</eClassifiers>"));

            var main = this.CreateResourceForContent(
                "cross-main.ecore",
                Package(
                    "crossmain",
                    "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\">" +
                    "<eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"toB\" eType=\"cross-child.ecore#//B\" eOpposite=\"cross-child.ecore#//B/toA\"/>" +
                    "</eClassifiers>"));

            var root = main.Load(null);

            var toB = Reference(root, "A", "toB");

            Assert.That(toB.EOpposite, Is.Not.Null, "the cross-file opposite did not resolve");
            Assert.Multiple(() =>
            {
                Assert.That(toB.EOpposite!.Name, Is.EqualTo("toA"));
                Assert.That(toB.EOpposite!.EContainingClass.Name, Is.EqualTo("B"));
                // the opposite lives in the demand-loaded sibling resource
                Assert.That(Path.GetFileName(toB.EOpposite!.EResource.URI.LocalPath), Is.EqualTo("cross-child.ecore"));
                // and the relation is symmetric back into this resource
                Assert.That(toB.EOpposite!.EOpposite, Is.SameAs(toB));
            });
        }

        [Test]
        public void Verify_that_a_forward_cross_file_opposite_does_not_throw()
        {
            WriteModel(
                "forward-child.ecore",
                Package(
                    "forwardchild",
                    "<eClassifiers xsi:type=\"ecore:EClass\" name=\"B\">" +
                    "<eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"toA\" eType=\"forward-main.ecore#//A\" eOpposite=\"forward-main.ecore#//A/toB\"/>" +
                    "</eClassifiers>"));

            var mainUri = new Uri(Path.Combine(TestContext.CurrentContext.TestDirectory, "forward-main.ecore"));
            WriteModel(
                "forward-main.ecore",
                Package(
                    "forwardmain",
                    "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\">" +
                    "<eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"toB\" eType=\"forward-child.ecore#//B\" eOpposite=\"forward-child.ecore#//B/toA\"/>" +
                    "</eClassifiers>"));

            // demand-loading main pulls in forward-child while resolving the forward cross-file opposite;
            // this must not raise the IOException / StackOverflowException reported in issue #81
            Assert.That(() => this.resourceSet.Resource(mainUri, true), Throws.Nothing);

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

        /// <summary>
        /// Resolves the <see cref="EReference"/> named <paramref name="referenceName"/> on the class named
        /// <paramref name="className"/> within the supplied root <see cref="EPackage"/>.
        /// </summary>
        private static EReference Reference(EPackage root, string className, string referenceName)
        {
            return root.EClassifiers
                .OfType<EClass>()
                .Single(c => c.Name == className)
                .EStructuralFeatures
                .OfType<EReference>()
                .Single(r => r.Name == referenceName);
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
