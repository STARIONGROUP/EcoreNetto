// -------------------------------------------------------------------------------------------------
// <copyright file="EEnumLiteral.cs" company="RHEA System S.A.">
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
    /// Represents the literal of an enumeration
    /// </summary>
    public class EEnumLiteral : ENamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EEnumLiteral"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        public EEnumLiteral(Resource.Resource resource) : base(resource)
        {
        }
        
        /// <summary>
        /// Gets the containing <see cref="EEnum"/>
        /// </summary>
        public EEnum EEnum
        {
            get
            {
                return (EEnum)this.EContainer;
            }
        }

        /// <summary>
        /// Gets or sets int value of an enumerator. 
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// The set properties.
        /// </summary>
        internal override void SetProperties()
        {
            base.SetProperties();

            if (this.Attributes.TryGetValue("value", out var output))
            {
                this.Value = int.Parse(output);
            }
        }

        /// <summary>
        /// Build the <see cref="EModelElement.Identifier"/> property
        /// </summary>
        /// <returns>The identifier</returns>
        protected override string BuildIdentifier()
        {
            return $"{this.EEnum.Identifier}/{this.Name}";
        }
    }
}
