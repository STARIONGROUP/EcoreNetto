// ------------------------------------------------------------------------------------------------
// <copyright file="XmlTypeResolutionTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests that verify references into the <c>http://www.eclipse.org/emf/2003/XMLType</c>
    /// namespace resolve to their built-in data types without a backing file (see issue #98), the way EMF
    /// resolves them through its package registry.
    /// </summary>
    [TestFixture]
    public class XmlTypeResolutionTestFixture
    {
        private const string XmlTypeNamespace = "http://www.eclipse.org/emf/2003/XMLType";

        private ResourceSet resourceSet = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
        }

        [TestCase("String")]
        [TestCase("Int")]
        [TestCase("Boolean")]
        [TestCase("AnySimpleType")]
        [TestCase("QName")]
        [TestCase("Date")]
        [TestCase("IDREFS")]
        public void Verify_that_a_fully_qualified_XMLType_reference_resolves_to_the_data_type(string typeName)
        {
            var resource = new Resource();

            var resolved = resource.GetEObject($"{XmlTypeNamespace}#//{typeName}");

            Assert.That(resolved, Is.InstanceOf<EDataType>());
            Assert.That(((EDataType)resolved!).Name, Is.EqualTo(typeName));
        }

        [Test]
        public void Verify_that_a_model_referencing_XMLType_data_types_loads_and_resolves_the_attribute_type()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "xmltype.ecore");
            File.WriteAllText(path, Model);

            var resource = this.resourceSet.CreateResource(new Uri(path));

            EPackage root = null!;
            Assert.That(() => root = resource.Load(null), Throws.Nothing);

            Assert.That(resource.Errors, Is.Empty, string.Join("; ", resource.Errors.Select(e => e.Message)));

            var value = root.EClassifiers
                .OfType<EClass>()
                .Single(c => c.Name == "Node")
                .EStructuralFeatures
                .OfType<EAttribute>()
                .Single(a => a.Name == "value");

            Assert.That(value.EType, Is.InstanceOf<EDataType>());
            Assert.That(value.EType!.Name, Is.EqualTo("String"));
        }

        [Test]
        public void Verify_that_an_unknown_XMLType_reference_does_not_resolve()
        {
            var resource = new Resource();

            var resolved = resource.GetEObject($"{XmlTypeNamespace}#//NotARealType");

            Assert.That(resolved, Is.Null);
        }

        private const string Model =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
            "<ecore:EPackage xmi:version=\"2.0\" xmlns:xmi=\"http://www.omg.org/XMI\" " +
            "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
            "xmlns:ecore=\"http://www.eclipse.org/emf/2002/Ecore\" " +
            "name=\"xmltype\" nsURI=\"xmltype\" nsPrefix=\"xmltype\">\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"Node\">\r\n" +
            "    <eStructuralFeatures xsi:type=\"ecore:EAttribute\" name=\"value\" " +
            "eType=\"ecore:EDataType http://www.eclipse.org/emf/2003/XMLType#//String\"/>\r\n" +
            "  </eClassifiers>\r\n" +
            "</ecore:EPackage>";
    }
}
