// -------------------------------------------------------------------------------------------------
// <copyright file="EcoreObjectInstantiator.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2022 RHEA System S.A.
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

namespace ECoreNetto.Utils
{
    using Resource;

    /// <summary>
    /// internal class responsible for instantiating ECORE class objects
    /// </summary>
    internal class EcoreObjectInstantiator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EcoreObjectInstantiator"/>
        /// </summary>
        /// <param name="resource">The <see cref="Resource"/></param>
        public EcoreObjectInstantiator(Resource resource)
        {
            this.EObject = new EClass(resource) { Name = "EObject", Abstract = true };

            this.EModelElement = new EClass(resource) { Name = "EModelElement", Abstract = true };
            this.EModelElement.ESuperTypes.Add(this.EObject);

            this.ENamedElement = new EClass(resource) { Name = "ENamedElement", Abstract = true };
            this.ENamedElement.ESuperTypes.Add(this.EModelElement);

            this.EFactory = new EClass(resource) { Name = "EFactory" };
            this.EFactory.ESuperTypes.Add(this.EModelElement);

            this.EAnnotation = new EClass(resource) {Name = "EAnnotation" };
            this.EAnnotation.ESuperTypes.Add(this.EModelElement);

            this.EClassifier = new EClass(resource) { Name = "EClassifier", Abstract = true };
            this.EClassifier.ESuperTypes.Add(this.ENamedElement);

            this.EEnumLiteral = new EClass(resource) { Name = "EEnumLiteral" };
            this.EEnumLiteral.ESuperTypes.Add(this.ENamedElement);

            this.EPackage = new EClass(resource) { Name = "EPackage" };
            this.EPackage.ESuperTypes.Add(this.ENamedElement);

            this.ETypedElement = new EClass(resource) { Name = "ETypedElement", Abstract = true };
            this.ETypedElement.ESuperTypes.Add(this.ENamedElement);

            this.EClass = new EClass(resource) { Name = "EClass" };
            this.EClass.ESuperTypes.Add(this.EClassifier);

            this.EDataType = new EClass(resource) { Name = "EDataType" };
            this.EDataType.ESuperTypes.Add(this.EClassifier);

            this.EEnum = new EClass(resource) { Name = "EEnum" };
            this.EEnum.ESuperTypes.Add(this.EClassifier);

            this.EOperation = new EClass(resource) { Name = "EOperation" };
            this.EOperation.ESuperTypes.Add(this.ETypedElement);

            this.EParameter = new EClass(resource) { Name = "EParameter" };
            this.EParameter.ESuperTypes.Add(this.ETypedElement);

            this.EStructuralFeature = new EClass(resource) { Name = "EStructuralFeature", Abstract = true };
            this.EStructuralFeature.ESuperTypes.Add(this.ETypedElement);

            this.EAttribute = new EClass(resource) { Name = "EAttribute" };
            this.EAttribute.ESuperTypes.Add(this.EStructuralFeature);

            this.EReference = new EClass(resource) { Name = "EReference" };
            this.EReference.ESuperTypes.Add(this.EStructuralFeature);

            this.EGenericType = new EClass(resource) { Name = "EGenericType" };

            this.ETypeParameter = new EClass(resource) { Name = "ETypeParameter" };
            this.ETypeParameter.ESuperTypes.Add(this.ENamedElement);

            this.EBigDecimal = new EDataType(resource) { Name = "EBigDecimal" };
            this.EBigInteger = new EDataType(resource) { Name = "EBigInteger" };
            this.EBool = new EDataType(resource) { Name = "EBool"};
            this.EBooleanObject = new EDataType(resource) { Name = "EBooleanObject" };
            this.EByte = new EDataType(resource) { Name = "EByte" };
            this.EByteArray = new EDataType(resource) { Name = "EByteArray" };
            this.EByteObject = new EDataType(resource) { Name = "EByteObject" };
            this.EChar = new EDataType(resource) { Name = "EChar" };
            this.ECharacterObject = new EDataType(resource) { Name = "ECharacterObject" };
            this.EDate = new EDataType(resource) { Name = "EDate" };
            this.EDiagnosticChain = new EDataType(resource) { Name = "EDiagnosticChain" };
            this.EDouble = new EDataType(resource) { Name = "EDouble" };
            this.EDoubleObject = new EDataType(resource) { Name = "EDoubleObject" };
            this.EEList = new EDataType(resource) { Name = "EEList" };
            this.EEnumerator = new EDataType(resource) { Name = "EEnumerator" };
            this.EFeatureMap = new EDataType(resource) { Name = "EFeatureMap" };
            this.EFeatureMapEntry = new EDataType(resource) { Name = "EFeatureMapEntry" };
            this.EFloat = new EDataType(resource) { Name = "EFloat" };
            this.EFloatObject = new EDataType(resource) { Name = "EFloatObject" };
            this.EInt = new EDataType(resource) { Name = "EInt" };
            this.EIntegerObject = new EDataType(resource) { Name = "EIntegerObject" };
            this.EJavaClass = new EDataType(resource) { Name = "EJavaClass" };
            this.EJavaObject = new EDataType(resource) { Name = "EJavaObject" };
            this.ELong = new EDataType(resource) { Name = "ELong" };
            this.ELongObject = new EDataType(resource) { Name = "ELongObject" };
            this.EMap = new EDataType(resource) { Name = "EMap" };
            this.EResource = new EDataType(resource) { Name = "EResource" };
            this.EResourceSet = new EDataType(resource) { Name = "EResourceSet" };
            this.EShort = new EDataType(resource) { Name = "EShort" };
            this.EShortObject = new EDataType(resource) { Name = "EShortObject" };
            this.EString = new EDataType(resource) { Name = "EString" };
            this.ETreeIterator = new EDataType(resource) { Name = "ETreeIterator" };
            this.EInvocationTargetException = new EDataType(resource) { Name = "EInvocationTargetException" };
        }

        /// <summary>
        /// Gets the <see cref="EObject"/> instance
        /// </summary>
        public EClass EObject { get; private set; }

        /// <summary>
        /// Gets the <see cref="EModelElement"/> instance
        /// </summary>
        public EClass EModelElement { get; private set; }

        /// <summary>
        /// Gets the <see cref="ENamedElement"/> instance
        /// </summary>
        public EClass ENamedElement { get; private set; }

        /// <summary>
        /// Gets the <see cref="EFactory"/> instance
        /// </summary>
        public EClass EFactory { get; private set; }

        /// <summary>
        /// Gets the <see cref="EAnnotation"/> instance
        /// </summary>
        public EClass EAnnotation { get; private set; }

        /// <summary>
        /// Gets the <see cref="EClassifier"/> instance
        /// </summary>
        public EClass EClassifier { get; private set; }

        /// <summary>
        /// Gets the <see cref="EEnumLiteral"/> instance
        /// </summary>
        public EClass EEnumLiteral { get; private set; }

        /// <summary>
        /// Gets the <see cref="EPackage"/> instance
        /// </summary>
        public EClass EPackage { get; private set; }

        /// <summary>
        /// Gets the <see cref="ETypedElement"/> instance
        /// </summary>
        public EClass ETypedElement { get; private set; }

        /// <summary>
        /// Gets the <see cref="EClass"/> instance
        /// </summary>
        public EClass EClass { get; private set; }

        /// <summary>
        /// Gets the <see cref="EDataType"/> instance
        /// </summary>
        public EClass EDataType { get; private set; }

        /// <summary>
        /// Gets the <see cref="EEnum"/> instance
        /// </summary>
        public EClass EEnum { get; private set; }

        /// <summary>
        /// Gets the <see cref="EOperation"/> instance
        /// </summary>
        public EClass EOperation { get; private set; }

        /// <summary>
        /// Gets the <see cref="EParameter"/> instance
        /// </summary>
        public EClass EParameter { get; private set; }

        /// <summary>
        /// Gets the <see cref="EStructuralFeature"/> instance
        /// </summary>
        public EClass EStructuralFeature { get; private set; }
        
        /// <summary>
        /// Gets the <see cref="EAttribute"/> instance
        /// </summary>
        public EClass EAttribute { get; private set; }

        /// <summary>
        /// Gets the <see cref="EReference"/> instance
        /// </summary>
        public EClass EReference { get; private set; }

        /// <summary>
        /// Gets the <see cref="EStringToStringMapEntry"/> instance
        /// </summary>
        public EClass EStringToStringMapEntry { get; private set; }

        /// <summary>
        /// Gets the <see cref="EGenericType"/> instance
        /// </summary>
        public EClass EGenericType { get; private set; }

        /// <summary>
        /// Gets the <see cref="ETypeParameter"/> instance
        /// </summary>
        public EClass ETypeParameter { get; private set; }

        /// <summary>
        /// Gets the <see cref="EBigDecimal"/> instance
        /// </summary>
        public EDataType EBigDecimal { get; private set; }

        /// <summary>
        /// Gets the <see cref="EBigInteger"/> instance
        /// </summary>
        public EDataType EBigInteger { get; private set; }

        /// <summary>
        /// Gets the <see cref="EBool"/> instance
        /// </summary>
        public EDataType EBool { get; private set; }

        /// <summary>
        /// Gets the <see cref="EBooleanObject"/> instance
        /// </summary>
        public EDataType EBooleanObject { get; private set; }

        /// <summary>
        /// Gets the <see cref="EByte"/> instance
        /// </summary>
        public EDataType EByte { get; private set; }

        /// <summary>
        /// Gets the <see cref="EByteArray"/> instance
        /// </summary>
        public EDataType EByteArray { get; private set; }

        /// <summary>
        /// Gets the <see cref="EByteObject"/> instance
        /// </summary>
        public EDataType EByteObject { get; private set; }

        /// <summary>
        /// Gets the <see cref="EChar"/> instance
        /// </summary>
        public EDataType EChar { get; private set; }

        /// <summary>
        /// Gets the <see cref="ECharacterObject"/> instance
        /// </summary>
        public EDataType ECharacterObject { get; private set; }

        /// <summary>
        /// Gets the <see cref="EDate"/> instance
        /// </summary>
        public EDataType EDate { get; private set; }

        /// <summary>
        /// Gets the <see cref="EDiagnosticChain"/> instance
        /// </summary>
        public EDataType EDiagnosticChain { get; private set; }

        /// <summary>
        /// Gets the <see cref="EDouble"/> instance
        /// </summary>
        public EDataType EDouble { get; private set; }

        /// <summary>
        /// Gets the <see cref="EDoubleObject"/> instance
        /// </summary>
        public EDataType EDoubleObject { get; private set; }

        /// <summary>
        /// Gets the <see cref="EEList"/> instance
        /// </summary>
        public EDataType EEList { get; private set; }

        /// <summary>
        /// Gets the <see cref="EEnumerator"/> instance
        /// </summary>
        public EDataType EEnumerator { get; private set; }

        /// <summary>
        /// Gets the <see cref="EFeatureMap"/> instance
        /// </summary>
        public EDataType EFeatureMap { get; private set; }

        /// <summary>
        /// Gets the <see cref="EFeatureMapEntry"/> instance
        /// </summary>
        public EDataType EFeatureMapEntry { get; private set; }

        /// <summary>
        /// Gets the <see cref="EFloat"/> instance
        /// </summary>
        public EDataType EFloat { get; private set; }

        /// <summary>
        /// Gets the <see cref="EFloatObject"/> instance
        /// </summary>
        public EDataType EFloatObject { get; private set; }

        /// <summary>
        /// Gets the <see cref="EInt"/> instance
        /// </summary>
        public EDataType EInt { get; private set; }

        /// <summary>
        /// Gets the <see cref="EIntegerObject"/> instance
        /// </summary>
        public EDataType EIntegerObject { get; private set; }

        /// <summary>
        /// Gets the <see cref="EJavaClass"/> instance
        /// </summary>
        public EDataType EJavaClass { get; private set; }

        /// <summary>
        /// Gets the <see cref="EJavaObject"/> instance
        /// </summary>
        public EDataType EJavaObject { get; private set; }

        /// <summary>
        /// Gets the <see cref="ELong"/> instance
        /// </summary>
        public EDataType ELong { get; private set; }

        /// <summary>
        /// Gets the <see cref="ELongObject"/> instance
        /// </summary>
        public EDataType ELongObject { get; private set; }

        /// <summary>
        /// Gets the <see cref="EMap"/> instance
        /// </summary>
        public EDataType EMap { get; private set; }

        /// <summary>
        /// Gets the <see cref="EResource"/> instance
        /// </summary>
        public EDataType EResource { get; private set; }

        /// <summary>
        /// Gets the <see cref="EResourceSet"/> instance
        /// </summary>
        public EDataType EResourceSet { get; private set; }

        /// <summary>
        /// Gets the <see cref="EShort"/> instance
        /// </summary>
        public EDataType EShort { get; private set; }

        /// <summary>
        /// Gets the <see cref="EShortObject"/> instance
        /// </summary>
        public EDataType EShortObject { get; private set; }
        
        /// <summary>
        /// Gets the <see cref="EString"/> instance
        /// </summary>
        public EDataType EString { get; private set; }

        /// <summary>
        /// Gets the <see cref="ETreeIterator"/> instance
        /// </summary>
        public EDataType ETreeIterator { get; private set; }

        /// <summary>
        /// Gets the <see cref="EInvocationTargetException"/> instance
        /// </summary>
        public EDataType EInvocationTargetException { get; private set; }
    }
}