// -------------------------------------------------------------------------------------------------
// <copyright file="EDataType.cs" company="RHEA System S.A.">
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
    /// <summary>
    /// Represents a simple datatype
    /// </summary>
    /// <remarks>
    /// <see cref="EDataType"/> is used to represent simple types whose details are not modeled as classes. Instead, the are associated
    /// with a primitive or object type fully defined in C#. Data types are also identified by name, and they are used as the types of 
    /// attributes.
    /// </remarks>
    public class EDataType : EClassifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EDataType"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        public EDataType(Resource.Resource resource) : base(resource)
        {
            this.Serializable = true;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EDataType"/> is serializable
        /// </summary>
        public bool Serializable { get; private set; }

        /// <summary>
        /// Read the attributes of the current node
        /// </summary>
        internal override void SetProperties()
        {
            base.SetProperties();

            if (this.Attributes.TryGetValue("serializable", out var output))
            {
                this.Serializable = bool.Parse(output);
            }
        }
    }
}
