// ------------------------------------------------------------------------------------------------
// <copyright file="GetEObjectRecursionTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests that verify <see cref="Resource.GetEObject(string)"/> always terminates: an
    /// unresolvable fragment whose file part resolves back to a resource already on the resolution path
    /// must be reported as unresolved (with a recorded <see cref="Diagnostic"/>) rather than recursing
    /// unboundedly into a <see cref="StackOverflowException"/> (see issue #80).
    /// </summary>
    [TestFixture]
    public class GetEObjectRecursionTestFixture
    {
        private ResourceSet resourceSet = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
        }

        [Test]
        public void Verify_that_a_fragment_pointing_at_the_own_resource_for_an_unknown_element_returns_null_without_recursing()
        {
            var resource = this.CreateResourceForContent(
                "self-cycle.ecore",
                Package("self-cycle", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\"/>"));
            resource.Load(null);

            EObject? result = null;

            // the fragment names this resource's own file but an element that does not exist; resolution
            // resolves the file back to this resource and must stop instead of recursing forever
            Assert.That(() => result = resource.GetEObject("self-cycle.ecore#//DoesNotExist"), Throws.Nothing);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(resource.Errors, Is.Not.Empty);
                Assert.That(resource.Errors.Last().Message, Does.Contain("DoesNotExist"));
            });
        }

        [Test]
        public void Verify_that_an_eOpposite_style_fragment_for_an_unknown_feature_returns_null_without_recursing()
        {
            var resource = this.CreateResourceForContent(
                "opposite-cycle.ecore",
                Package("opposite-cycle", "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\"/>"));
            resource.Load(null);

            EObject? result = null;

            // an eOpposite reference carries an 'EStructuralFeature::' prefix before the file name; an unknown
            // feature must resolve to null without recursing
            Assert.That(
                () => result = resource.GetEObject("EStructuralFeature::opposite-cycle.ecore#//A/missingOpposite"),
                Throws.Nothing);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void Verify_that_loading_a_model_with_a_self_referencing_missing_super_type_reports_it_without_recursing()
        {
            var model = Package(
                "broken-super",
                "<eClassifiers xsi:type=\"ecore:EClass\" name=\"A\" eSuperTypes=\"broken-super.ecore#//Missing\"/>");
            var resource = this.CreateResourceForContent("broken-super.ecore", model);

            // EClass.SetProperties resolves eSuperTypes via the guarded GetEObject<EClass>; the unresolved
            // reference must surface as a clear InvalidOperationException, not a stack overflow
            var exception = Assert.Throws<InvalidOperationException>(() => resource.Load(null));
            Assert.That(exception!.Message, Does.Contain("Missing"));
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

            var uri = new Uri(Path.GetFullPath(path));

            return this.resourceSet.CreateResource(uri);
        }
    }
}
