﻿// -------------------------------------------------------------------------------------------------
// <copyright file="EAttribute.cs" company="Starion Group S.A.">
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
    /// A representation of the model object <see cref="EAttribute"/>. 
    /// </summary>
    /// <remarks>
    /// <see cref="EAttribute"/> models attributes, the components of an object's data. They are identified by name,
    /// and they have a type.
    /// </remarks>
    public class EAttribute : EStructuralFeature
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<EAttribute> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EAttribute"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public EAttribute(Resource.Resource resource, ILoggerFactory loggerFactory = null) : base(resource, loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<EAttribute>.Instance : loggerFactory.CreateLogger<EAttribute>();
        }

        /// <summary>
        /// Gets a value indicating whether the ID attribute is set
        /// </summary>
        /// <remarks>
        /// An ID attribute explicitly models the one unique <see cref="ID"/> of an object. 
        /// </remarks>
        public bool ID { get; private set; }

        /// <summary>
        /// Read the attributes of the current node
        /// </summary>
        internal override void SetProperties()
        {
            this.logger.LogTrace("setting properties of EAttribute {0}:{1}", this.Identifier, this.Name);

            base.SetProperties();

            if (this.Attributes.TryGetValue("iD", out var output))
            {
                this.ID = bool.Parse(output);
            }
        }
    }
}
