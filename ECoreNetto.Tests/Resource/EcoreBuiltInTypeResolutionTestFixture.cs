// ------------------------------------------------------------------------------------------------
// <copyright file="EcoreBuiltInTypeResolutionTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tests.Resource
{
    using ECoreNetto.Resource;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests that verify <see cref="Resource.GetEObject(string)"/> resolves the built-in Ecore
    /// types by exact key (see issue #83). References to those types appear either as the bare
    /// <c>//EName</c> fragment or in the fully-qualified
    /// <c>http://www.eclipse.org/emf/2002/Ecore#//EName</c> form; both must resolve to exactly the right
    /// type without the substring-matching defects (misnamed <c>EBool</c> key, and
    /// <c>Sequence contains more than one matching element</c> for prefix-overlapping names).
    /// </summary>
    [TestFixture]
    public class EcoreBuiltInTypeResolutionTestFixture
    {
        private const string EcoreNamespacePrefix = "http://www.eclipse.org/emf/2002/Ecore#";

        private Resource resource = null!;

        [SetUp]
        public void SetUp()
        {
            this.resource = new Resource();
        }

        [Test]
        public void Verify_that_a_fully_qualified_EBoolean_reference_resolves_to_the_EBoolean_data_type()
        {
            // the Capella metamodel references 'Ecore#//EBoolean'; it must resolve to the EBoolean data type
            // and not to a misnamed 'EBool' via substring matching
            var resolved = this.resource.GetEObject($"{EcoreNamespacePrefix}//EBoolean");

            Assert.That(resolved, Is.InstanceOf<EDataType>());
            Assert.That(((EDataType)resolved!).Name, Is.EqualTo("EBoolean"));
        }

        [Test]
        public void Verify_that_a_bare_fragment_reference_resolves_the_built_in_type()
        {
            var resolved = this.resource.GetEObject("//EString");

            Assert.That(resolved, Is.InstanceOf<EDataType>());
            Assert.That(((EDataType)resolved!).Name, Is.EqualTo("EString"));
        }

        [Test]
        public void Verify_that_a_prefix_overlapping_reference_resolves_exactly_and_does_not_throw()
        {
            // '//EClassifier' contains '//EClass'; substring matching used to throw
            // "Sequence contains more than one matching element"
            EObject? resolved = null;

            Assert.That(
                () => resolved = this.resource.GetEObject($"{EcoreNamespacePrefix}//EClassifier"),
                Throws.Nothing);

            Assert.That(((EClass)resolved!).Name, Is.EqualTo("EClassifier"));
        }

        [Test]
        public void Verify_that_EStringToStringMapEntry_resolves_exactly_and_not_to_EString()
        {
            // '//EStringToStringMapEntry' contains '//EString'; used to be ambiguous
            EObject? resolved = null;

            Assert.That(
                () => resolved = this.resource.GetEObject($"{EcoreNamespacePrefix}//EStringToStringMapEntry"),
                Throws.Nothing);

            Assert.That(((EClass)resolved!).Name, Is.EqualTo("EStringToStringMapEntry"));
        }

        [Test]
        public void Verify_that_EEnumLiteral_resolves_exactly_and_not_to_EEnum()
        {
            // '//EEnumLiteral' contains '//EEnum'; used to be ambiguous
            EObject? resolved = null;

            Assert.That(
                () => resolved = this.resource.GetEObject($"{EcoreNamespacePrefix}//EEnumLiteral"),
                Throws.Nothing);

            Assert.That(((EClass)resolved!).Name, Is.EqualTo("EEnumLiteral"));
        }
    }
}
