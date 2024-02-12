// -------------------------------------------------------------------------------------------------
// <copyright file="EParameter.cs" company="RHEA System S.A.">
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
    /// <summary>
    /// The ECore parameter class.
    /// </summary>
    public class EParameter : ETypedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EParameter"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        public EParameter(Resource.Resource resource) : base(resource)
        {
        }

        /// <summary>
        /// Gets the containing <see cref="EOperation"/>
        /// </summary>
        public EOperation EOperation
        {
            get
            {
                return (EOperation)this.EContainer;
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
            return $"{this.EOperation.Identifier}/{this.Name}";
        }
    }
}
