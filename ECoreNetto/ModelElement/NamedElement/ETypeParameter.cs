// ------------------------------------------------------------------------------------------------
// <copyright file="ETypeParameter.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto
{
    using System;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// Represents a declared type parameter (type variable) of an <see cref="EClassifier"/> or
    /// <see cref="EOperation"/> (the Ecore <c>ETypeParameter</c> metaclass), for example <c>E</c> in
    /// <c>EEList&lt;E&gt;</c>.
    /// </summary>
    public class ETypeParameter : ENamedElement
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory? loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<ETypeParameter> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ETypeParameter"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public ETypeParameter(Resource.Resource resource, ILoggerFactory? loggerFactory = null) : base(resource, loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            this.logger = this.loggerFactory == null ? NullLogger<ETypeParameter>.Instance : this.loggerFactory.CreateLogger<ETypeParameter>();

            this.EBounds = new ContainerList<EGenericType>(this);
        }

        /// <summary>
        /// Gets the bounds that constrain this type parameter (the <c>extends</c> constraints).
        /// </summary>
        public ContainerList<EGenericType> EBounds { get; }

        /// <summary>
        /// Instantiate new <see cref="EGenericType"/> bounds from the current node of the <see cref="XmlNode"/>
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlNode"/>
        /// </param>
        protected override void DeserializeChildNode(XmlNode reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            base.DeserializeChildNode(reader);

            if (reader.Name == "eBounds" && reader.NodeType == XmlNodeType.Element)
            {
                var bound = new EGenericType(this.EResource, this.loggerFactory);
                this.EBounds.Add(bound);
                bound.ReadXml(reader);
            }
        }

        /// <summary>
        /// Resolve the cross-references of this type parameter by propagating to its bounds.
        /// </summary>
        internal override void SetProperties()
        {
            this.logger.LogTrace("setting properties of ETypeParameter {0}:{1}", this.Identifier, this.Name);

            base.SetProperties();

            foreach (var bound in this.EBounds)
            {
                bound.SetProperties();
            }
        }

        /// <summary>
        /// Build the EModelElement.Identifier property
        /// </summary>
        /// <returns>
        /// The identifier
        /// </returns>
        protected override string BuildIdentifier()
        {
            return $"{this.EContainer!.Identifier}/{this.Name}";
        }
    }
}
