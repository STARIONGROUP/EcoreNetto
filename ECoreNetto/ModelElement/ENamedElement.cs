// -------------------------------------------------------------------------------------------------
// <copyright file="ENamedElement.cs" company="RHEA System S.A.">
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
    using System.Xml;

    /// <summary>
    /// The base abstract type for <see cref="EModelElement"/> with a name
    /// </summary>
    public abstract class ENamedElement : EModelElement
    {
        /// <summary>
        /// Backing field for <see cref="Identifier"/>
        /// </summary>
        private string identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="ENamedElement"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        protected ENamedElement(Resource.Resource resource) : base(resource)
        {
        }

        /// <summary>
        /// Gets the name of this <see cref="ENamedElement"/>
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the identifier for this <see cref="ENamedElement"/>
        /// </summary>
        public override string Identifier
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.identifier))
                {
                    this.identifier = this.BuildIdentifier();
                }

                return this.identifier;
            }
        }

        /// <summary>
        /// Process the current node and its children of the current <see cref="XmlNode"/>
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlNode"/>
        /// </param>
        /// <remarks>
        /// This overrides the base implementation to set the name property
        /// This is required as the name acts like an identifier for the <see cref="EModelElement"/>
        /// </remarks>
        public override void ReadXml(XmlNode reader)
        {
            this.SetName(reader);
            this.EResource.Cache.Add(this.Identifier, this);
            
            base.ReadXml(reader);
        }
        
        /// <summary>
        /// Build the <see cref="EModelElement.Identifier"/> property
        /// </summary>
        /// <returns>
        /// The identifier
        /// </returns>
        protected abstract string BuildIdentifier();

        /// <summary>
        /// Set the <see cref="Name"/> of this <see cref="ENamedElement"/>
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/>
        /// </param>
        protected void SetName(XmlNode reader)
        {
            var nameAtt = reader.Attributes["name"];
            if (nameAtt != null)
            {
                this.Name = nameAtt.Value;
            }
        }
    }
}
