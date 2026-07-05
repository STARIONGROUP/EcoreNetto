// ------------------------------------------------------------------------------------------------
// <copyright file="ENamedElement.cs" company="Starion Group S.A.">
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

    /// <summary>
    /// The base abstract type for <see cref="EModelElement"/> with a name
    /// </summary>
    public abstract class ENamedElement : EModelElement
    {
        /// <summary>
        /// Backing field for <see cref="Identifier"/>
        /// </summary>
        private string? identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="ENamedElement"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected ENamedElement(Resource.Resource resource, ILoggerFactory? loggerFactory = null) : base(resource)
        {
        }

        /// <summary>
        /// Gets the name of this <see cref="ENamedElement"/>
        /// </summary>
        public string Name { get; internal set; } = null!;

        /// <summary>
        /// Gets the identifier for this <see cref="ENamedElement"/>
        /// </summary>
        public override string Identifier
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.identifier))
                {
                    this.identifier = this.BuildIdentifier();
                }

                return this.identifier!;
            }
        }

        /// <summary>
        /// Process the current node and its children of the current <see cref="XmlNode"/>
        /// </summary>
        /// <param name="element">
        /// The <see cref="XmlNode"/>
        /// </param>
        /// <remarks>
        /// This overrides the base implementation to set the name property
        /// This is required as the name acts like an identifier for the <see cref="EModelElement"/>
        /// </remarks>
        public override void ReadXml(XmlNode element)
        {
            this.SetName(element);

            // Register in the resource cache, disambiguating duplicate identifiers the way EMF does: the
            // first occurrence keeps its identifier; a subsequent sibling that would collide gets a '.N'
            // suffix (e.g. two overloaded operations -> '.../getX' and '.../getX.1'). Ecore permits
            // same-named EOperations, so an unguarded Dictionary.Add would throw and abort the load.
            var baseIdentifier = this.BuildIdentifier();
            var uniqueIdentifier = baseIdentifier;
            var duplicateCount = 0;
            while (this.EResource.Cache.ContainsKey(uniqueIdentifier))
            {
                duplicateCount++;
                uniqueIdentifier = $"{baseIdentifier}.{duplicateCount}";
            }

            this.identifier = uniqueIdentifier;
            this.EResource.Cache.Add(uniqueIdentifier, this);

            base.ReadXml(element);
        }
        
        /// <summary>
        /// Build the EModelElement.Identifier property
        /// </summary>
        /// <returns>
        /// The identifier
        /// </returns>
        protected abstract string BuildIdentifier();

        /// <summary>
        /// Set the <see cref="Name"/> of this <see cref="ENamedElement"/>
        /// </summary>
        /// <param name="reader">
        /// The <see cref="XmlReader"/>
        /// </param>
        protected void SetName(XmlNode reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var nameAtt = reader.Attributes["name"];
            if (nameAtt != null)
            {
                this.Name = nameAtt.Value;
            }
        }
    }
}
