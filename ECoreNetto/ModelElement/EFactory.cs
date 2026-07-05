// ------------------------------------------------------------------------------------------------
// <copyright file="EFactory.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto
{
    using Microsoft.Extensions.Logging;

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
        public EFactory(Resource.Resource resource, ILoggerFactory? loggerFactory = null) : base(resource, loggerFactory)
        {
        }

        /// <summary>
        /// Gets the <see cref="EPackage"/>
        /// </summary>
        public EPackage? EPackage { get; private set; }
    }
}
