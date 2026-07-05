// ------------------------------------------------------------------------------------------------
// <copyright file="GenericTypeTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests that verify Ecore generics (<see cref="EGenericType"/> / <see cref="ETypeParameter"/>)
    /// and operation exceptions are read rather than silently dropped (see issue #97): type parameters and
    /// their bounds, generic feature types, generic super types, generic exceptions, wildcard bounds, and
    /// the derived erased <c>EType</c>/<c>ESuperTypes</c>/<c>EExceptions</c> views.
    /// </summary>
    [TestFixture]
    public class GenericTypeTestFixture
    {
        private ResourceSet resourceSet = null!;
        private EPackage root = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();

            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "generics.ecore");
            File.WriteAllText(path, Model);

            var resource = this.resourceSet.CreateResource(new Uri(path));
            this.root = resource.Load(null);

            Assert.That(
                resource.Errors,
                Is.Empty,
                string.Join("; ", resource.Errors.Select(e => e.Message)));
        }

        [Test]
        public void Verify_that_type_parameters_and_their_bounds_are_read()
        {
            var collection = this.Class("Collection");

            Assert.That(collection.ETypeParameters, Has.Count.EqualTo(1));

            var typeParameter = collection.ETypeParameters.Single();

            Assert.Multiple(() =>
            {
                Assert.That(typeParameter.Name, Is.EqualTo("E"));
                Assert.That(typeParameter.EBounds, Has.Count.EqualTo(1));
                Assert.That(typeParameter.EBounds.Single().EClassifier, Is.SameAs(this.Class("Item")));
            });
        }

        [Test]
        public void Verify_that_a_generic_feature_type_and_its_type_parameter_argument_resolve()
        {
            var elements = this.Reference("Collection", "elements");
            var typeParameter = this.Class("Collection").ETypeParameters.Single();

            Assert.That(elements.EGenericType, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(elements.EGenericType!.EClassifier, Is.SameAs(this.Class("Collection")));
                // the erased EType is derived from the generic type's raw classifier
                Assert.That(elements.EType, Is.SameAs(this.Class("Collection")));
                Assert.That(elements.EGenericType!.ETypeArguments, Has.Count.EqualTo(1));
                Assert.That(elements.EGenericType!.ETypeArguments.Single().ETypeParameter, Is.SameAs(typeParameter));
            });
        }

        [Test]
        public void Verify_that_wildcard_upper_and_lower_bounds_are_read()
        {
            var item = this.Class("Item");

            var upper = this.Reference("Collection", "upperWild").EGenericType!.ETypeArguments.Single();
            var lower = this.Reference("Collection", "lowerWild").EGenericType!.ETypeArguments.Single();

            Assert.Multiple(() =>
            {
                Assert.That(upper.EUpperBound, Is.Not.Null);
                Assert.That(upper.EUpperBound!.EClassifier, Is.SameAs(item));
                Assert.That(lower.ELowerBound, Is.Not.Null);
                Assert.That(lower.ELowerBound!.EClassifier, Is.SameAs(item));
            });
        }

        [Test]
        public void Verify_that_generic_super_types_resolve_and_feed_the_erased_super_type_view()
        {
            var itemCollection = this.Class("ItemCollection");
            var collection = this.Class("Collection");

            Assert.That(itemCollection.EGenericSuperTypes, Has.Count.EqualTo(1));

            var genericSuperType = itemCollection.EGenericSuperTypes.Single();

            Assert.Multiple(() =>
            {
                Assert.That(genericSuperType.EClassifier, Is.SameAs(collection));
                Assert.That(genericSuperType.ETypeArguments.Single().EClassifier, Is.SameAs(this.Class("Item")));
                // the raw class of the generic super type is added to the erased ESuperTypes view
                Assert.That(itemCollection.ESuperTypes, Does.Contain(collection));
            });
        }

        [Test]
        public void Verify_that_operation_exceptions_are_read_from_both_the_attribute_and_generic_forms()
        {
            var notFound = this.Class("NotFound");

            var findItems = this.Class("Collection").EOperations.Single(o => o.Name == "findItems");
            var loadItems = this.Class("Collection").EOperations.Single(o => o.Name == "loadItems");

            Assert.Multiple(() =>
            {
                // eExceptions attribute form
                Assert.That(findItems.EExceptions, Does.Contain(notFound));
                // eGenericExceptions element form, also surfaced through the erased EExceptions view
                Assert.That(loadItems.EGenericExceptions.Single().EClassifier, Is.SameAs(notFound));
                Assert.That(loadItems.EExceptions, Does.Contain(notFound));
            });
        }

        [Test]
        public void Verify_that_an_operation_type_parameter_is_read()
        {
            var transform = this.Class("Collection").EOperations.Single(o => o.Name == "transform");

            Assert.That(transform.ETypeParameters, Has.Count.EqualTo(1));
            Assert.That(transform.ETypeParameters.Single().Name, Is.EqualTo("R"));
        }

        private EClass Class(string name)
        {
            return this.root.EClassifiers.OfType<EClass>().Single(c => c.Name == name);
        }

        private EReference Reference(string className, string referenceName)
        {
            return this.Class(className).EStructuralFeatures.OfType<EReference>().Single(r => r.Name == referenceName);
        }

        private const string Model =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
            "<ecore:EPackage xmi:version=\"2.0\" xmlns:xmi=\"http://www.omg.org/XMI\" " +
            "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
            "xmlns:ecore=\"http://www.eclipse.org/emf/2002/Ecore\" " +
            "name=\"generics\" nsURI=\"generics\" nsPrefix=\"generics\">\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"Collection\">\r\n" +
            "    <eTypeParameters name=\"E\">\r\n" +
            "      <eBounds eClassifier=\"#//Item\"/>\r\n" +
            "    </eTypeParameters>\r\n" +
            "    <eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"elements\" upperBound=\"-1\">\r\n" +
            "      <eGenericType eClassifier=\"#//Collection\">\r\n" +
            "        <eTypeArguments eTypeParameter=\"#//Collection/E\"/>\r\n" +
            "      </eGenericType>\r\n" +
            "    </eStructuralFeatures>\r\n" +
            "    <eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"upperWild\" upperBound=\"-1\">\r\n" +
            "      <eGenericType eClassifier=\"#//Collection\">\r\n" +
            "        <eTypeArguments>\r\n" +
            "          <eUpperBound eClassifier=\"#//Item\"/>\r\n" +
            "        </eTypeArguments>\r\n" +
            "      </eGenericType>\r\n" +
            "    </eStructuralFeatures>\r\n" +
            "    <eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"lowerWild\" upperBound=\"-1\">\r\n" +
            "      <eGenericType eClassifier=\"#//Collection\">\r\n" +
            "        <eTypeArguments>\r\n" +
            "          <eLowerBound eClassifier=\"#//Item\"/>\r\n" +
            "        </eTypeArguments>\r\n" +
            "      </eGenericType>\r\n" +
            "    </eStructuralFeatures>\r\n" +
            "    <eOperations name=\"findItems\" eExceptions=\"#//NotFound\"/>\r\n" +
            "    <eOperations name=\"loadItems\">\r\n" +
            "      <eGenericExceptions eClassifier=\"#//NotFound\"/>\r\n" +
            "    </eOperations>\r\n" +
            "    <eOperations name=\"transform\">\r\n" +
            "      <eTypeParameters name=\"R\"/>\r\n" +
            "    </eOperations>\r\n" +
            "  </eClassifiers>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"Item\"/>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"NotFound\"/>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"ItemCollection\">\r\n" +
            "    <eGenericSuperTypes eClassifier=\"#//Collection\">\r\n" +
            "      <eTypeArguments eClassifier=\"#//Item\"/>\r\n" +
            "    </eGenericSuperTypes>\r\n" +
            "  </eClassifiers>\r\n" +
            "</ecore:EPackage>";
    }
}
