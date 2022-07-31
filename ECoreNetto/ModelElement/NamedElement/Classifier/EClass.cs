// -------------------------------------------------------------------------------------------------
// <copyright file="EClass.cs" company="RHEA System S.A.">
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

namespace ECoreNetto
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// A type that represents a class in the ECore model
    /// </summary>
    /// <remarks>
    /// Classes are identified by name and can have a number of attributes and references. To support inheritance, a class can
    /// refer to a number of other classes as its supertypes.
    /// </remarks>
    public class EClass : EClassifier
    {
        /// <summary>
        /// The ECORE abstract keyword.
        /// </summary>
        private const string EcoreAbstractKeyword = "abstract";

        /// <summary>
        /// The ECORE interface keyword.
        /// </summary>
        private const string EcoreInterfaceKeyword = "interface";

        /// <summary>
        /// The ECORE super type keyword.
        /// </summary>
        private const string EcoreSuperTypeKeyword = "eSuperTypes";

        /// <summary>
        /// Initializes a new instance of the <see cref="EClass"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        public EClass(Resource.Resource resource) : base(resource)
        {
            this.ESuperTypes = new List<EClass>();
            this.EOperations = new ContainerList<EOperation>(this);
            this.EStructuralFeatures = new ContainerList<EStructuralFeature>(this);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EClass"/> is abstract
        /// </summary>
        public bool Abstract { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EClass"/> represents an interface
        /// </summary>
        public bool Interface { get; internal set; }

        /// <summary>
        /// Gets the collection of super <see cref="EClass"/>
        /// </summary>
        public List<EClass> ESuperTypes { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="EOperation"/>
        /// </summary>
        public ContainerList<EOperation> EOperations { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="EStructuralFeature"/>
        /// </summary>
        public ContainerList<EStructuralFeature> EStructuralFeatures { get; private set; }

        /// <summary>
        /// Gets all <see cref="EStructuralFeature"/> from this <see cref="EClass"/> and all its <see cref="ESuperTypes"/>
        /// </summary>
        /// <remarks>
        /// There might be duplicates depending on the model
        /// </remarks>
        public IEnumerable<EStructuralFeature> AllEStructuralFeatures
        {
            get { return this.EStructuralFeatures.Union(this.ESuperTypes.SelectMany(x => x.AllEStructuralFeatures)) ; }
        }

        /// <summary>
        /// Gets all <see cref="EStructuralFeature"/> from this <see cref="EClass"/> and all its <see cref="ESuperTypes"/> ordered by name
        /// </summary>
        /// <remarks>
        /// There might be duplicates depending on the model
        /// </remarks>
        public IEnumerable<EStructuralFeature> AllEStructuralFeaturesOrderByName
        {
            get { return this.AllEStructuralFeatures.OrderBy(x => x.Name); }
        }

        /// <summary>
        /// Gets all <see cref="EStructuralFeature"/> from this <see cref="EClass"/> and all its <see cref="ESuperTypes"/> that are marked as interfaces
        /// </summary>
        /// <remarks>
        /// There might be duplicates depending on the model
        /// </remarks>
        public IEnumerable<EStructuralFeature> InterfaceAndOwnStructuralFeatures
        {
            get
            {
                return this.EStructuralFeatures.Union(this.ESuperTypes.Where(x => x.Interface).SelectMany(x => x.AllEStructuralFeatures));
            }
        }

        /// <summary>
        /// Read the attributes of the current node
        /// </summary>
        internal override void SetProperties()
        {
            base.SetProperties();

            if (this.Attributes.TryGetValue(EcoreAbstractKeyword, out var output))
            {
                this.Abstract = bool.Parse(output);
            }

            if (this.Attributes.TryGetValue(EcoreInterfaceKeyword, out output))
            {
                this.Interface = bool.Parse(output);
            }

            if (this.Attributes.TryGetValue(EcoreSuperTypeKeyword, out output))
            {
                var typeNames = output.Split(' ');
                foreach (var typeName in typeNames)
                {
                    this.ESuperTypes.Add((EClass)this.EResource.GetEObject(typeName));
                }
            }
        }

        /// <summary>
        /// Instantiate new <see cref="EModelElement"/> from the current node of the <see cref="XmlNode"/>
        /// </summary>
        /// <param name="reader">The <see cref="XmlNode"/></param>
        protected override void DeserializeChildNode(XmlNode reader)
        {
            base.DeserializeChildNode(reader);

            if (reader.Name == "eStructuralFeatures" && reader.NodeType == XmlNodeType.Element)
            {
                var ecoreType = reader.Attributes["xsi:type"].Value;
                switch (ecoreType)
                {
                    case "ecore:EReference":
                        var ecoreReference = new EReference(this.EResource);
                        this.EStructuralFeatures.Add(ecoreReference);
                        ecoreReference.ReadXml(reader);
                        break;
                    case "ecore:EAttribute":
                        var ecoreAttribute = new EAttribute(this.EResource);
                        this.EStructuralFeatures.Add(ecoreAttribute);
                        ecoreAttribute.ReadXml(reader);
                        break;
                    default:
                        throw new InvalidOperationException($"Type of structural feature not recognized: {ecoreType}");
                }
            }

            if (reader.Name == "eOperations" && reader.NodeType == XmlNodeType.Element)
            {
                var ecoreOperation = new EOperation(this.EResource);
                this.EOperations.Add(ecoreOperation);
                ecoreOperation.ReadXml(reader);
            }
        }
    }
}