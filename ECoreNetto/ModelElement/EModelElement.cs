// -------------------------------------------------------------------------------------------------
// <copyright file="EModelElement.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2023 RHEA System S.A.
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
    using System.Xml;
    
    /// <summary>
    /// The base abstract class for ECore objects
    /// </summary>
    public abstract class EModelElement : EObject
    {
        /// <summary>
        /// The ECORE annotation.
        /// </summary>
        private const string EcoreAnnotation = "eAnnotations";

        /// <summary>
        /// Initializes a new instance of the <see cref="EModelElement"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        protected EModelElement(Resource.Resource resource) : base(resource)
        {            
            this.EAnnotations = new ContainerList<EAnnotation>(this);
        }
        
        /// <summary>
        /// Gets the <see cref="EAnnotation"/>s that are contained by this <see cref="EModelElement"/>
        /// </summary>
        public ContainerList<EAnnotation> EAnnotations { get; private set; }

        /// <summary>
        /// Set the properties of this <see cref="EModelElement"/>
        /// </summary>
        internal override void SetProperties()
        {
            // Ensure invocation of setProperties on contained EAnnotations collection
            foreach (var annotation in this.EAnnotations)
            {
                annotation.SetProperties();
            }
        }

        /// <summary>
        /// Instantiate new <see cref="EModelElement"/> from the current node of the <see cref="XmlReader"/>
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/></param>
        protected override void DeserializeChildNode(XmlNode reader)
        {
            if (reader.Name == EcoreAnnotation && reader.NodeType == XmlNodeType.Element)
            {
                var annotation = new EAnnotation(this.EResource);
                this.EAnnotations.Add(annotation);
                annotation.ReadXml(reader);
            }
        }
    }
}
