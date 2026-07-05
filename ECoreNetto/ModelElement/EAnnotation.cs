// ------------------------------------------------------------------------------------------------
// <copyright file="EAnnotation.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The type representing an annotation
    /// </summary>
    public class EAnnotation : EModelElement
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<EAnnotation> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EAnnotation"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public EAnnotation(Resource.Resource resource, ILoggerFactory? loggerFactory = null) : base(resource, loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<EAnnotation>.Instance : loggerFactory.CreateLogger<EAnnotation>();

            this.Details = new Dictionary<string, string>();
            this.References = new List<EObject>();
        }

        /// <summary>
        /// Gets the source of this <see cref="EAnnotation"/>
        /// </summary>
        public string? Source { get; private set; }

        /// <summary>
        /// Gets the
        /// </summary>
        public Dictionary<string, string> Details { get; private set; }

        /// <summary>
        /// Gets the <see cref="EObject"/>s referenced by this <see cref="EAnnotation"/> (the
        /// <c>references</c> feature).
        /// </summary>
        public List<EObject> References { get; private set; }

        /// <summary>
        /// Gets the <see cref="EModelElement"/> that is annotated by the current <see cref="EAnnotation"/>
        /// </summary>
        public EModelElement EModelElement => (EModelElement)this.EContainer!;

        /// <summary>
        /// Set the properties of this <see cref="EModelElement"/>
        /// </summary>
        internal override void SetProperties()
        {
            this.logger.LogTrace("setting properties of EAnnotation:EModelElement {0}:{1}", this.Identifier, this.EModelElement.Identifier);

            base.SetProperties();

            if (this.Attributes.TryGetValue("source", out var source))
            {
                this.Source = source;
            }

            if (this.Attributes.TryGetValue("references", out var references))
            {
                foreach (var reference in references.Split(' '))
                {
                    var referenced = this.EResource.GetEObject(reference);
                    if (referenced != null)
                    {
                        this.References.Add(referenced);
                    }
                }
            }
        }

        /// <summary>
        /// Instantiate new <see cref="EModelElement"/> from the current node of the <see cref="XmlNode"/>
        /// </summary>
        /// <param name="reader">The <see cref="XmlNode"/></param>
        protected override void DeserializeChildNode(XmlNode reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            this.logger.LogTrace("deserializing child nodes of EAnnotation of EModelElement {0}", this.EModelElement.Identifier);

            base.DeserializeChildNode(reader);

            if (reader.Name == "details")
            {
                var key = reader.Attributes["key"];
                var value = reader.Attributes["value"];

                if (key != null)
                {
                    this.Details.Add(key.Value, value == null ? string.Empty : value.Value);
                }
            }
        }
    }
}
