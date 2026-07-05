// ------------------------------------------------------------------------------------------------
// <copyright file="EDataType.cs" company="Starion Group S.A.">
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
    /// Represents a simple datatype
    /// </summary>
    /// <remarks>
    /// <see cref="EDataType"/> is used to represent simple types whose details are not modeled as classes. Instead, the are associated
    /// with a primitive or object type fully defined in C#. Data types are also identified by name, and they are used as the types of 
    /// attributes.
    /// </remarks>
    public class EDataType : EClassifier
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<EDataType> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EDataType"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public EDataType(Resource.Resource resource, ILoggerFactory? loggerFactory = null) : base(resource, loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<EDataType>.Instance : loggerFactory.CreateLogger<EDataType>();

            this.Serializable = true;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="EDataType"/> is serializable
        /// </summary>
        public bool Serializable { get; private set; }

        /// <summary>
        /// Read the attributes of the current node
        /// </summary>
        internal override void SetProperties()
        {
            this.logger.LogTrace("setting properties of EDataType {0}:{1}", this.Identifier, this.Name);

            base.SetProperties();

            if (this.Attributes.TryGetValue("serializable", out var output))
            {
                this.Serializable = this.ParseBoolean("serializable", output);
            }
        }
    }
}
