// -------------------------------------------------------------------------------------------------
// <copyright file="EReference.cs" company="RHEA System S.A.">
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
    /// <summary>
    /// A representation of the model object <see cref="EReference"/>. 
    /// </summary>
    /// <remarks>
    /// <see cref="EReference"/> is used in modeling associations between classes. it models one end of such an association. Like attributes, 
    /// references are identified by name and have a type. However, this type must be the <see cref="EClass"/> at the other end of the 
    /// association.
    /// </remarks>
    public class EReference : EStructuralFeature
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EReference"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        public EReference(Resource.Resource resource) : base(resource)
        {
            this.IsResolveProxies = true;
        }

        /// <summary>
        /// Gets a value indicating whether containment.
        /// </summary>
        public bool IsContainment { get; private set; }

        /// <summary>
        /// Gets a value indicating whether container.
        /// </summary>
        public bool IsContainer { get; private set; }

        /// <summary>
        /// Gets a value indicating whether resolve proxies.
        /// </summary>
        public bool IsResolveProxies { get; private set; }

        /// <summary>
        /// Gets the e opposite.
        /// </summary>
        public EReference EOpposite { get; private set; }

        /// <summary>
        /// Read the attributes of the current node
        /// </summary>
        internal override void SetProperties()
        {
            base.SetProperties();

            if (this.Attributes.TryGetValue("container", out var output))
            {
                this.IsContainer = bool.Parse(output);
            }

            if (this.Attributes.TryGetValue("containment", out output))
            {
                this.IsContainment = bool.Parse(output);
            }

            if (this.Attributes.TryGetValue("resolveProxies", out output))
            {
                this.IsResolveProxies = bool.Parse(output);
            }

            if (this.Attributes.TryGetValue("eOpposite", out output))
            {
                var typeName = output;
                this.EOpposite = (EReference)this.EResource.GetEObject($"EStructuralFeature::{typeName}");
            }
        }
    }
}