// ------------------------------------------------------------------------------------------------
// <copyright file="EParameter.cs" company="Starion Group S.A.">
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
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public EParameter(Resource.Resource resource, ILoggerFactory? loggerFactory = null) : base(resource, loggerFactory)
        {
        }

        /// <summary>
        /// Gets the containing <see cref="EOperation"/>
        /// </summary>
        public EOperation EOperation => (EOperation)this.EContainer!;

        /// <summary>
        /// Build the EModelElement.Identifier property
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
