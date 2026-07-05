// ------------------------------------------------------------------------------------------------
// <copyright file="EGenericType.cs" company="Starion Group S.A.">
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
    /// Represents a generic type reference (the Ecore <c>EGenericType</c> metaclass): a use of a
    /// classifier or a type parameter, optionally parameterized with type arguments and/or constrained
    /// by wildcard bounds.
    /// </summary>
    /// <remarks>
    /// An <see cref="EGenericType"/> is not an <see cref="ENamedElement"/> and therefore is not registered
    /// in the resource cache. Its cross-references are resolved during the second deserialization pass by
    /// the containing element, which propagates <see cref="SetProperties"/> into it.
    /// </remarks>
    public class EGenericType : EObject
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory? loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<EGenericType> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EGenericType"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public EGenericType(Resource.Resource resource, ILoggerFactory? loggerFactory = null) : base(resource, loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            this.logger = this.loggerFactory == null ? NullLogger<EGenericType>.Instance : this.loggerFactory.CreateLogger<EGenericType>();

            this.ETypeArguments = new ContainerList<EGenericType>(this);
        }

        /// <summary>
        /// Gets the referenced <see cref="EClassifier"/> (the raw type), or null when this is a
        /// type-variable or unbounded-wildcard use.
        /// </summary>
        public EClassifier? EClassifier { get; private set; }

        /// <summary>
        /// Gets the referenced <see cref="ETypeParameter"/> when this generic type is a use of a type
        /// variable, or null otherwise.
        /// </summary>
        public ETypeParameter? ETypeParameter { get; private set; }

        /// <summary>
        /// Gets the type arguments with which the <see cref="EClassifier"/> is parameterized.
        /// </summary>
        public ContainerList<EGenericType> ETypeArguments { get; }

        /// <summary>
        /// Gets the upper bound of this generic type (the <c>? extends X</c> part of a wildcard), or null.
        /// </summary>
        public EGenericType? EUpperBound { get; private set; }

        /// <summary>
        /// Gets the lower bound of this generic type (the <c>? super X</c> part of a wildcard), or null.
        /// </summary>
        public EGenericType? ELowerBound { get; private set; }

        /// <summary>
        /// Instantiate new <see cref="EGenericType"/> children from the current node of the <see cref="XmlNode"/>
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

            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            switch (reader.Name)
            {
                case "eTypeArguments":
                    var argument = new EGenericType(this.EResource, this.loggerFactory);
                    this.ETypeArguments.Add(argument);
                    argument.ReadXml(reader);
                    break;
                case "eUpperBound":
                    this.EUpperBound = new EGenericType(this.EResource, this.loggerFactory) { EContainer = this };
                    this.EUpperBound.ReadXml(reader);
                    break;
                case "eLowerBound":
                    this.ELowerBound = new EGenericType(this.EResource, this.loggerFactory) { EContainer = this };
                    this.ELowerBound.ReadXml(reader);
                    break;
            }
        }

        /// <summary>
        /// Resolve the cross-references of this generic type and propagate to its nested generic types.
        /// </summary>
        internal override void SetProperties()
        {
            this.logger.LogTrace("setting properties of EGenericType");

            if (this.Attributes.TryGetValue("eClassifier", out var output))
            {
                var parts = output.Split(' ');
                this.EClassifier = this.EResource.GetEObject<EClassifier>(parts[parts.Length - 1]);
            }

            if (this.Attributes.TryGetValue("eTypeParameter", out output))
            {
                var parts = output.Split(' ');
                this.ETypeParameter = this.EResource.GetEObject<ETypeParameter>(parts[parts.Length - 1]);
            }

            this.EUpperBound?.SetProperties();
            this.ELowerBound?.SetProperties();

            foreach (var argument in this.ETypeArguments)
            {
                argument.SetProperties();
            }
        }
    }
}
