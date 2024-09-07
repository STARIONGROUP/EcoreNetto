// -------------------------------------------------------------------------------------------------
// <copyright file="EFactory.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
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
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The type representing a ECore Factory
    /// </summary>
    public class EFactory : EModelElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EFactory"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public EFactory(Resource.Resource resource, ILoggerFactory loggerFactory = null) : base(resource, loggerFactory)
        {
        }

        /// <summary>
        /// Gets the <see cref="EPackage"/>
        /// </summary>
        public EPackage EPackage { get; private set; }
    }
}
