// -------------------------------------------------------------------------------------------------
// <copyright file="EcoreObjectInstantiator.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
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
    }
}