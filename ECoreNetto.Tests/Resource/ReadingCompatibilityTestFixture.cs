// ------------------------------------------------------------------------------------------------
// <copyright file="ReadingCompatibilityTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the EMF reading-compatibility items of issue #99: the previously unread
    /// features (<c>EReference.eKeys</c>, <c>EClassifier.instanceTypeName</c>, <c>EEnumLiteral.literal</c>,
    /// <c>EAnnotation.references</c>) and unwrapping an <c>xmi:XMI</c> document root instead of returning a
    /// silently empty package.
    /// </summary>
    [TestFixture]
    public class ReadingCompatibilityTestFixture
    {
        private ResourceSet resourceSet = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
        }

        [Test]
        public void Verify_that_eKeys_instanceTypeName_literal_and_annotation_references_are_read()
        {
            var root = this.Load("features.ecore", FeaturesModel);

            var person = Class(root, "Person");
            var ssn = person.EStructuralFeatures.OfType<EAttribute>().Single(a => a.Name == "ssn");
            var manager = person.EStructuralFeatures.OfType<EReference>().Single(r => r.Name == "manager");
            var idType = root.EClassifiers.OfType<EDataType>().Single(c => c.Name == "IdType");
            var status = root.EClassifiers.OfType<EEnum>().Single(c => c.Name == "Status");
            var annotated = Class(root, "Annotated");

            Assert.Multiple(() =>
            {
                Assert.That(manager.EKeys, Does.Contain(ssn));
                Assert.That(idType.InstanceTypeName, Is.EqualTo("java.lang.String"));
                Assert.That(status.ELiterals.Single(l => l.Name == "ACTIVE").Literal, Is.EqualTo("active"));
                Assert.That(annotated.EAnnotations.Single().References, Does.Contain(person));
            });
        }

        [Test]
        public void Verify_that_an_xmi_wrapped_single_package_is_unwrapped_and_read()
        {
            var root = this.Load("xmi-single.ecore", XmiWrappedSinglePackage);

            Assert.Multiple(() =>
            {
                Assert.That(root.Name, Is.EqualTo("wrapped"));
                Assert.That(root.EClassifiers.OfType<EClass>().Any(c => c.Name == "Widget"), Is.True);
            });
        }

        [Test]
        public void Verify_that_an_xmi_root_with_no_package_reports_a_clear_error()
        {
            var resource = this.CreateResource("xmi-empty.ecore", XmiWrappedNoPackage);

            Assert.That(() => resource.Load(null), Throws.InstanceOf<InvalidOperationException>());
            Assert.That(
                resource.Errors.Select(e => e.Message),
                Has.Some.Contains("no ecore:EPackage"));
        }

        [Test]
        public void Verify_that_an_xmi_root_with_multiple_packages_reports_a_clear_error()
        {
            var resource = this.CreateResource("xmi-multi.ecore", XmiWrappedMultiplePackages);

            Assert.That(() => resource.Load(null), Throws.InstanceOf<InvalidOperationException>());
            Assert.That(
                resource.Errors.Select(e => e.Message),
                Has.Some.Contains("multiple root packages"));
        }

        private static EClass Class(EPackage root, string name)
        {
            return root.EClassifiers.OfType<EClass>().Single(c => c.Name == name);
        }

        private Resource CreateResource(string fileName, string content)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);
            File.WriteAllText(path, content);

            return this.resourceSet.CreateResource(new Uri(path));
        }

        private EPackage Load(string fileName, string content)
        {
            var resource = this.CreateResource(fileName, content);
            var root = resource.Load(null);

            Assert.That(resource.Errors, Is.Empty, string.Join("; ", resource.Errors.Select(e => e.Message)));

            return root;
        }

        private const string FeaturesModel =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
            "<ecore:EPackage xmi:version=\"2.0\" xmlns:xmi=\"http://www.omg.org/XMI\" " +
            "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
            "xmlns:ecore=\"http://www.eclipse.org/emf/2002/Ecore\" " +
            "name=\"features\" nsURI=\"features\" nsPrefix=\"features\">\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"Person\">\r\n" +
            "    <eStructuralFeatures xsi:type=\"ecore:EAttribute\" name=\"ssn\" eType=\"#//IdType\"/>\r\n" +
            "    <eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"manager\" eType=\"#//Person\" eKeys=\"#//Person/ssn\"/>\r\n" +
            "  </eClassifiers>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EDataType\" name=\"IdType\" instanceClassName=\"java.lang.String\" instanceTypeName=\"java.lang.String\"/>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EEnum\" name=\"Status\">\r\n" +
            "    <eLiterals name=\"ACTIVE\" value=\"0\" literal=\"active\"/>\r\n" +
            "  </eClassifiers>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"Annotated\">\r\n" +
            "    <eAnnotations source=\"http://example.org/refs\" references=\"#//Person\"/>\r\n" +
            "  </eClassifiers>\r\n" +
            "</ecore:EPackage>";

        private const string XmiHeader =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
            "<xmi:XMI xmi:version=\"2.0\" xmlns:xmi=\"http://www.omg.org/XMI\" " +
            "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
            "xmlns:ecore=\"http://www.eclipse.org/emf/2002/Ecore\">\r\n";

        private const string XmiWrappedSinglePackage =
            XmiHeader +
            "  <ecore:EPackage name=\"wrapped\" nsURI=\"wrapped\" nsPrefix=\"wrapped\">\r\n" +
            "    <eClassifiers xsi:type=\"ecore:EClass\" name=\"Widget\"/>\r\n" +
            "  </ecore:EPackage>\r\n" +
            "</xmi:XMI>";

        private const string XmiWrappedNoPackage =
            XmiHeader +
            "  <ecore:EAnnotation source=\"http://example.org\"/>\r\n" +
            "</xmi:XMI>";

        private const string XmiWrappedMultiplePackages =
            XmiHeader +
            "  <ecore:EPackage name=\"first\" nsURI=\"first\" nsPrefix=\"first\"/>\r\n" +
            "  <ecore:EPackage name=\"second\" nsURI=\"second\" nsPrefix=\"second\"/>\r\n" +
            "</xmi:XMI>";
    }
}
