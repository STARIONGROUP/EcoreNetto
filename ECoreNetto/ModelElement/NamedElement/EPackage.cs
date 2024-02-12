// -------------------------------------------------------------------------------------------------
// <copyright file="EPackage.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2024 RHEA System S.A.
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
    /// The ECore package.
    /// </summary>
    public class EPackage : ENamedElement
    {
        /// <summary>
        /// The ECORE class key.
        /// </summary>
        private const string EcoreClassKey = "ecore:EClass";

        /// <summary>
        /// The ECORE data type key.
        /// </summary>
        private const string EcoreDataTypeKey = "ecore:EDataType";

        /// <summary>
        /// The ECORE enum key.
        /// </summary>
        private const string EcoreEnumKey = "ecore:EEnum";

        /// <summary>
        /// The XSI type.
        /// </summary>
        private const string XsiType = "xsi:type";

        /// <summary>
        /// Initializes a new instance of the <see cref="EPackage"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        public EPackage(Resource.Resource resource) : base(resource)
        {
            this.ESubPackages = new ContainerList<EPackage>(this);
            this.EClassifiers = new ContainerList<EClassifier>(this);
        }
        
        /// <summary>
        /// Gets the URI
        /// </summary>
        public string NsUri { get; private set; }

        /// <summary>
        /// Gets the prefix
        /// </summary>
        public string NsPrefix { get; private set; }

        /// <summary>
        /// Gets the <see cref="EFactoryInstance"/>
        /// </summary>
        public EFactory EFactoryInstance { get; private set; }

        /// <summary>
        /// Gets the collection of sub <see cref="EPackage"/>
        /// </summary>
        public ContainerList<EPackage> ESubPackages { get; private set; }

        /// <summary>
        /// Gets the collection of <see cref="EClassifier"/> contained in this <see cref="EPackage"/>
        /// </summary>
        public ContainerList<EClassifier> EClassifiers { get; private set; }

        /// <summary>
        /// Gets the super <see cref="EPackage"/>
        /// </summary>
        /// <remarks>
        /// This may be null if this is the top <see cref="EPackage"/>
        /// </remarks>
        public EPackage ESuperPackage
        {
            get
            {
                return (EPackage)this.EContainer;
            }
        }

        /// <summary>
        /// Read the attributes of the current node
        /// </summary>
        internal override void SetProperties()
        {
            base.SetProperties();

            if (this.Attributes.TryGetValue("nsURI", out var output))
            {
                this.NsUri = output;
            }

            if (this.Attributes.TryGetValue("nsPrefix", out output))
            {
                this.NsPrefix = output;
            }
        }

        /// <summary>
        /// Instantiate new <see cref="EModelElement"/> from the current node of the <see cref="XmlNode"/>
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlNode"/>
        /// </param>
        protected override void DeserializeChildNode(XmlNode reader)
        {
            base.DeserializeChildNode(reader);
            if (reader.Name == "eSubpackages" && reader.NodeType == XmlNodeType.Element)
            {
                var package = new EPackage(this.EResource);
                this.ESubPackages.Add(package);
                package.ReadXml(reader);
            }

            if (reader.Name != "eClassifiers" || reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            var ecoreType = reader.Attributes[XsiType].Value;
            switch (ecoreType)
            {
                case EcoreClassKey:
                    var ecoreClass = new EClass(this.EResource);
                    this.EClassifiers.Add(ecoreClass);
                    ecoreClass.ReadXml(reader);
                    break;
                case EcoreDataTypeKey:
                    var ecoreDatatype = new EDataType(this.EResource);
                    this.EClassifiers.Add(ecoreDatatype);
                    ecoreDatatype.ReadXml(reader);
                    break;
                case EcoreEnumKey:
                    var ecoreEnum = new EEnum(this.EResource);
                    this.EClassifiers.Add(ecoreEnum);
                    ecoreEnum.ReadXml(reader);
                    break;
                default:
                    throw new InvalidOperationException($"Type of classifier not recognized: {ecoreType}");
            }
        }

        /// <summary>
        /// Build the <see cref="EModelElement.Identifier"/> property
        /// </summary>
        /// <returns>
        /// The identifier
        /// </returns>
        protected override string BuildIdentifier()
        {
            var packageHierarchy = new List<string> { this.Name };
            var superPackage = this.ESuperPackage;
            if (superPackage == null)
            {
                // set the current package
                EModelElement.TopPackageName = this.Name;
            }

            while (superPackage != null)
            {
                packageHierarchy.Add(superPackage.Name);
                superPackage = superPackage.ESuperPackage;
            }

            packageHierarchy[packageHierarchy.Count - 1] = $"{packageHierarchy.Last()}.ecore#/";
            packageHierarchy.Reverse();

            return string.Join("/", packageHierarchy);
        }
    }
}
