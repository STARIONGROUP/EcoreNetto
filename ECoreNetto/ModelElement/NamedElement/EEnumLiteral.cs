// ------------------------------------------------------------------------------------------------
// <copyright file="EEnumLiteral.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// Represents the literal of an enumeration
    /// </summary>
    public class EEnumLiteral : ENamedElement
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<EEnumLiteral> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EEnumLiteral"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public EEnumLiteral(Resource.Resource resource, ILoggerFactory? loggerFactory = null) : base(resource, loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<EEnumLiteral>.Instance : loggerFactory.CreateLogger<EEnumLiteral>();
        }
        
        /// <summary>
        /// Gets the containing <see cref="EEnum"/>
        /// </summary>
        public EEnum EEnum => (EEnum)this.EContainer!;

        /// <summary>
        /// Gets or sets int value of an enumerator.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Gets the literal string of this enumerator (the <c>literal</c> feature), i.e. the serialized
        /// form, which may differ from the <see cref="ENamedElement.Name"/>.
        /// </summary>
        public string? Literal { get; private set; }

        /// <summary>
        /// The set properties.
        /// </summary>
        internal override void SetProperties()
        {
            this.logger.LogTrace("setting properties of EEnumLiteral {0}:{1}", this.Identifier, this.Name);

            base.SetProperties();

            if (this.Attributes.TryGetValue("value", out var output))
            {
                this.Value = this.ParseInt32("value", output);
            }

            if (this.Attributes.TryGetValue("literal", out output))
            {
                this.Literal = output;
            }
        }

        /// <summary>
        /// Build the EModelElement.Identifier property
        /// </summary>
        /// <returns>The identifier</returns>
        protected override string BuildIdentifier()
        {
            return $"{this.EEnum.Identifier}/{this.Name}";
        }
    }
}
