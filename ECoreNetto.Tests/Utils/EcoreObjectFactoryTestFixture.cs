// -------------------------------------------------------------------------------------------------
// <copyright file="EcoreObjectFactoryTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tests.Utils
{
    using ECoreNetto.Resource;
    using ECoreNetto.Utils;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="EcoreObjectFactory"/> class.
    /// </summary>
    [TestFixture]
    public class EcoreObjectFactoryTestFixture
    {
        /// <summary>
        /// The <see cref="Resource"/> passed to the factory under test
        /// </summary>
        private Resource resource = null!;

        /// <summary>
        /// The class that is being tested
        /// </summary>
        private EcoreObjectFactory factory = null!;

        [SetUp]
        public void SetUp()
        {
            this.resource = new Resource();
            this.factory = new EcoreObjectFactory(this.resource);
        }

        [Test]
        public void Verify_that_factory_can_be_constructed_with_null_logger_factory()
        {
            Assert.That(() => new EcoreObjectFactory(new Resource(), null), Throws.Nothing);
        }

        [Test]
        public void Verify_that_all_meta_class_instances_are_created_with_expected_names()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.factory.EObject?.Name, Is.EqualTo("EObject"));
                Assert.That(this.factory.EModelElement?.Name, Is.EqualTo("EModelElement"));
                Assert.That(this.factory.ENamedElement?.Name, Is.EqualTo("ENamedElement"));
                Assert.That(this.factory.EFactory?.Name, Is.EqualTo("EFactory"));
                Assert.That(this.factory.EAnnotation?.Name, Is.EqualTo("EAnnotation"));
                Assert.That(this.factory.EClassifier?.Name, Is.EqualTo("EClassifier"));
                Assert.That(this.factory.EEnumLiteral?.Name, Is.EqualTo("EEnumLiteral"));
                Assert.That(this.factory.EPackage?.Name, Is.EqualTo("EPackage"));
                Assert.That(this.factory.ETypedElement?.Name, Is.EqualTo("ETypedElement"));
                Assert.That(this.factory.EClass?.Name, Is.EqualTo("EClass"));
                Assert.That(this.factory.EDataType?.Name, Is.EqualTo("EDataType"));
                Assert.That(this.factory.EEnum?.Name, Is.EqualTo("EEnum"));
                Assert.That(this.factory.EOperation?.Name, Is.EqualTo("EOperation"));
                Assert.That(this.factory.EParameter?.Name, Is.EqualTo("EParameter"));
                Assert.That(this.factory.EStructuralFeature?.Name, Is.EqualTo("EStructuralFeature"));
                Assert.That(this.factory.EAttribute?.Name, Is.EqualTo("EAttribute"));
                Assert.That(this.factory.EReference?.Name, Is.EqualTo("EReference"));
                Assert.That(this.factory.EStringToStringMapEntry?.Name, Is.EqualTo("EStringToStringMapEntry"));
                Assert.That(this.factory.EGenericType?.Name, Is.EqualTo("EGenericType"));
                Assert.That(this.factory.ETypeParameter?.Name, Is.EqualTo("ETypeParameter"));
            });
        }

        [Test]
        public void Verify_that_abstract_meta_classes_are_marked_abstract()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.factory.EObject.Abstract, Is.True);
                Assert.That(this.factory.EModelElement.Abstract, Is.True);
                Assert.That(this.factory.ENamedElement.Abstract, Is.True);
                Assert.That(this.factory.EClassifier.Abstract, Is.True);
                Assert.That(this.factory.ETypedElement.Abstract, Is.True);
                Assert.That(this.factory.EStructuralFeature.Abstract, Is.True);

                // a representative non-abstract meta class
                Assert.That(this.factory.EClass.Abstract, Is.False);
            });
        }

        [Test]
        public void Verify_that_the_meta_class_super_type_hierarchy_is_wired()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.factory.EModelElement.ESuperTypes, Does.Contain(this.factory.EObject));
                Assert.That(this.factory.ENamedElement.ESuperTypes, Does.Contain(this.factory.EModelElement));
                Assert.That(this.factory.EClassifier.ESuperTypes, Does.Contain(this.factory.ENamedElement));
                Assert.That(this.factory.ETypedElement.ESuperTypes, Does.Contain(this.factory.ENamedElement));
                Assert.That(this.factory.EClass.ESuperTypes, Does.Contain(this.factory.EClassifier));
                Assert.That(this.factory.EDataType.ESuperTypes, Does.Contain(this.factory.EClassifier));
                Assert.That(this.factory.EEnum.ESuperTypes, Does.Contain(this.factory.EClassifier));
                Assert.That(this.factory.EOperation.ESuperTypes, Does.Contain(this.factory.ETypedElement));
                Assert.That(this.factory.EParameter.ESuperTypes, Does.Contain(this.factory.ETypedElement));
                Assert.That(this.factory.EStructuralFeature.ESuperTypes, Does.Contain(this.factory.ETypedElement));
                Assert.That(this.factory.EAttribute.ESuperTypes, Does.Contain(this.factory.EStructuralFeature));
                Assert.That(this.factory.EReference.ESuperTypes, Does.Contain(this.factory.EStructuralFeature));
            });
        }

        [Test]
        public void Verify_that_all_data_type_instances_are_created_with_expected_names()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.factory.EBigDecimal?.Name, Is.EqualTo("EBigDecimal"));
                Assert.That(this.factory.EBigInteger?.Name, Is.EqualTo("EBigInteger"));
                Assert.That(this.factory.EBool?.Name, Is.EqualTo("EBool"));
                Assert.That(this.factory.EBooleanObject?.Name, Is.EqualTo("EBooleanObject"));
                Assert.That(this.factory.EByte?.Name, Is.EqualTo("EByte"));
                Assert.That(this.factory.EByteArray?.Name, Is.EqualTo("EByteArray"));
                Assert.That(this.factory.EByteObject?.Name, Is.EqualTo("EByteObject"));
                Assert.That(this.factory.EChar?.Name, Is.EqualTo("EChar"));
                Assert.That(this.factory.ECharacterObject?.Name, Is.EqualTo("ECharacterObject"));
                Assert.That(this.factory.EDate?.Name, Is.EqualTo("EDate"));
                Assert.That(this.factory.EDiagnosticChain?.Name, Is.EqualTo("EDiagnosticChain"));
                Assert.That(this.factory.EDouble?.Name, Is.EqualTo("EDouble"));
                Assert.That(this.factory.EDoubleObject?.Name, Is.EqualTo("EDoubleObject"));
                Assert.That(this.factory.EEList?.Name, Is.EqualTo("EEList"));
                Assert.That(this.factory.EEnumerator?.Name, Is.EqualTo("EEnumerator"));
                Assert.That(this.factory.EFeatureMap?.Name, Is.EqualTo("EFeatureMap"));
                Assert.That(this.factory.EFeatureMapEntry?.Name, Is.EqualTo("EFeatureMapEntry"));
                Assert.That(this.factory.EFloat?.Name, Is.EqualTo("EFloat"));
                Assert.That(this.factory.EFloatObject?.Name, Is.EqualTo("EFloatObject"));
                Assert.That(this.factory.EInt?.Name, Is.EqualTo("EInt"));
                Assert.That(this.factory.EIntegerObject?.Name, Is.EqualTo("EIntegerObject"));
                Assert.That(this.factory.EJavaClass?.Name, Is.EqualTo("EJavaClass"));
                Assert.That(this.factory.EJavaObject?.Name, Is.EqualTo("EJavaObject"));
                Assert.That(this.factory.ELong?.Name, Is.EqualTo("ELong"));
                Assert.That(this.factory.ELongObject?.Name, Is.EqualTo("ELongObject"));
                Assert.That(this.factory.EMap?.Name, Is.EqualTo("EMap"));
                Assert.That(this.factory.EResource?.Name, Is.EqualTo("EResource"));
                Assert.That(this.factory.EResourceSet?.Name, Is.EqualTo("EResourceSet"));
                Assert.That(this.factory.EShort?.Name, Is.EqualTo("EShort"));
                Assert.That(this.factory.EShortObject?.Name, Is.EqualTo("EShortObject"));
                Assert.That(this.factory.EString?.Name, Is.EqualTo("EString"));
                Assert.That(this.factory.ETreeIterator?.Name, Is.EqualTo("ETreeIterator"));
                Assert.That(this.factory.EInvocationTargetException?.Name, Is.EqualTo("EInvocationTargetException"));
            });
        }

        [Test]
        public void Verify_that_created_instances_reference_the_provided_resource()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.factory.EObject.EResource, Is.SameAs(this.resource));
                Assert.That(this.factory.EString.EResource, Is.SameAs(this.resource));
                Assert.That(this.factory.EStringToStringMapEntry.EResource, Is.SameAs(this.resource));
            });
        }
    }
}
