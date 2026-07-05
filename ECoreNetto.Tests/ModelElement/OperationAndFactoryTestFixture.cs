// ------------------------------------------------------------------------------------------------
// <copyright file="OperationAndFactoryTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tests.ModelElement
{
    using System;
    using System.Xml;

    using ECoreNetto.Resource;

    using NUnit.Framework;

    [TestFixture]
    public class OperationAndFactoryTestFixture
    {
        [Test]
        public void Verify_that_diagnostic_properties_are_set_by_constructor()
        {
            var diagnostic = new Diagnostic(8, 12, "test-location", "test message");

            Assert.Multiple(() =>
            {
                Assert.That(diagnostic.Column, Is.EqualTo(8));
                Assert.That(diagnostic.Line, Is.EqualTo(12));
                Assert.That(diagnostic.Location, Is.EqualTo("test-location"));
                Assert.That(diagnostic.Message, Is.EqualTo("test message"));
            });
        }

        [Test]
        public void Verify_that_efactory_can_be_constructed()
        {
            var resource = new Resource();

            var eFactory = new EFactory(resource);

            Assert.Multiple(() =>
            {
                Assert.That(eFactory.EResource, Is.EqualTo(resource));
                Assert.That(eFactory.EPackage, Is.Null);
            });
        }

        [Test]
        public void Verify_that_eoperation_DeserializeChildNode_throws_for_null_reader()
        {
            var resource = new Resource();
            var operation = new TestableEOperation(resource);

            Assert.That(() => operation.ExposeDeserializeChildNode(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_eoperation_DeserializeChildNode_adds_parameter()
        {
            var resource = new Resource();
            var package = new EPackage(resource)
            {
                Name = "Pkg"
            };

            var eClass = new EClass(resource)
            {
                Name = "SampleClass"
            };

            package.EClassifiers.Add(eClass);

            var operation = new TestableEOperation(resource)
            {
                Name = "DoWork"
            };

            eClass.EOperations.Add(operation);

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<eParameters name=\"parameterOne\" />");

            operation.ExposeDeserializeChildNode(xmlDocument.DocumentElement!);

            Assert.Multiple(() =>
            {
                Assert.That(operation.EParameters.Count, Is.EqualTo(1));
                Assert.That(operation.EParameters[0].Name, Is.EqualTo("parameterOne"));
                Assert.That(operation.EParameters[0].EOperation, Is.SameAs(operation));
            });
        }

        [Test]
        public void Verify_that_eoperation_identifier_contains_containing_class_identifier_and_name()
        {
            var resource = new Resource();
            var package = new EPackage(resource)
            {
                Name = "Pkg"
            };

            var eClass = new EClass(resource)
            {
                Name = "SampleClass"
            };

            package.EClassifiers.Add(eClass);

            var operation = new TestableEOperation(resource)
            {
                Name = "DoWork"
            };

            eClass.EOperations.Add(operation);

            Assert.That(operation.Identifier, Is.EqualTo($"EOperation::{eClass.Identifier}/DoWork"));
        }

        private class TestableEOperation : EOperation
        {
            public TestableEOperation(Resource resource)
                : base(resource)
            {
            }

            public void ExposeDeserializeChildNode(XmlNode node)
            {
                this.DeserializeChildNode(node);
            }
        }
    }
}
