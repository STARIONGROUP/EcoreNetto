// -------------------------------------------------------------------------------------------------
// <copyright file="EStructuralFeature.cs" company="RHEA System S.A.">
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
    /// The abstract ECORE structural feature class.
    /// </summary>
    public abstract class EStructuralFeature : ETypedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EStructuralFeature"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        protected EStructuralFeature(Resource.Resource resource) : base(resource)
        {
            this.Changeable = true;
        }

        /// <summary>
        /// Gets a value indicating whether changeable.
        /// </summary>
        public bool Changeable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether volatile.
        /// </summary>
        public bool Volatile { get; private set; }

        /// <summary>
        /// Gets a value indicating whether transient.
        /// </summary>
        public bool Transient { get; private set; }

        /// <summary>
        /// Gets a value indicating whether it can be unset.
        /// </summary>
        public bool Unsettable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether derived.
        /// </summary>
        public bool Derived { get; private set; }

        /// <summary>
        /// Gets the default value literal.
        /// </summary>
        public string DefaultValueLiteral { get; private set; }

        // todo defaultvalue

        /// <summary>
        /// Gets the containing <see cref="EClass"/>
        /// </summary>
        public EClass EContainingClass
        {
            get
            {
                return (EClass)this.EContainer;
            }
        }

        /// <summary>
        /// Read the attributes of the current node
        /// </summary>
        internal override void SetProperties()
        {
            base.SetProperties();

            if (this.Attributes.TryGetValue("changeable", out var output))
            {
                this.Changeable = bool.Parse(output);
            }

            if (this.Attributes.TryGetValue("volatile", out output))
            {
                this.Volatile = bool.Parse(output);
            }

            if (this.Attributes.TryGetValue("transient", out output))
            {
                this.Transient = bool.Parse(output);
            }

            if (this.Attributes.TryGetValue("defaultValueLiteral", out output))
            {
                this.DefaultValueLiteral = output;
            }

            if (this.Attributes.TryGetValue("unsettable", out output))
            {
                this.Unsettable = bool.Parse(output);
            }

            if (this.Attributes.TryGetValue("derived", out output))
            {
                this.Derived = bool.Parse(output);
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
            return $"{this.EContainingClass.Identifier}/{this.Name}";
        }
    }
}