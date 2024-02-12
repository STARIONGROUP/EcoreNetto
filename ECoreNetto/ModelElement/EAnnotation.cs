// -------------------------------------------------------------------------------------------------
// <copyright file="EAnnotation.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// The type representing an annotation
    /// </summary>
    public class EAnnotation : EModelElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EAnnotation"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        public EAnnotation(Resource.Resource resource) : base(resource)
        {
            this.Details = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the source of this <see cref="EAnnotation"/>
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Gets the 
        /// </summary>
        public Dictionary<string, string> Details { get; private set; }

        /// <summary>
        /// Gets the <see cref="EModelElement"/> that is annotated by the current <see cref="EAnnotation"/>
        /// </summary>
        public EModelElement EModelElement
        {
            get
            {
                return (EModelElement)this.EContainer;
            }
        }

        /// <summary>
        /// Set the properties of this <see cref="EModelElement"/>
        /// </summary>
        internal override void SetProperties()
        {
            base.SetProperties();

            if (this.Attributes.TryGetValue("source", out var source))
            {
                this.Source = source;
            }
        }

        /// <summary>
        /// Instantiate new <see cref="EModelElement"/> from the current node of the <see cref="XmlNode"/>
        /// </summary>
        /// <param name="reader">The <see cref="XmlNode"/></param>
        protected override void DeserializeChildNode(XmlNode reader)
        {
            base.DeserializeChildNode(reader);

            if (reader.Name == "details")
            {
                var key = reader.Attributes["key"];
                var value = reader.Attributes["value"];

                if (key != null)
                {
                    this.Details.Add(key.Value, value == null ? string.Empty : value.Value);
                }
            }
        }
    }
}
