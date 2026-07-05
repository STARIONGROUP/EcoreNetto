// ------------------------------------------------------------------------------------------------
// <copyright file="EClassifier.cs" company="Starion Group S.A.">
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
    /// The super abstract type for a type representing a classifier
    /// </summary>
    public abstract class EClassifier : ENamedElement
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory? loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<EClassifier> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EClassifier"/> class
        /// </summary>        
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected EClassifier(Resource.Resource resource, ILoggerFactory? loggerFactory = null) : base(resource, loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            this.logger = loggerFactory == null ? NullLogger<EClassifier>.Instance : loggerFactory.CreateLogger<EClassifier>();

            this.ETypeParameters = new ContainerList<ETypeParameter>(this);
        }

        /// <summary>
        /// Gets the instance class name.
        /// </summary>
        public string? InstanceClassName { get; private set; }

        /// <summary>
        /// Gets the instance type name (the <c>instanceTypeName</c> feature), which may include type
        /// arguments and differs from <see cref="InstanceClassName"/> for parameterized instance types.
        /// </summary>
        public string? InstanceTypeName { get; private set; }

        /// <summary>
        /// Gets the type parameters (type variables) declared by this <see cref="EClassifier"/>.
        /// </summary>
        public ContainerList<ETypeParameter> ETypeParameters { get; }

        /// <summary>
        /// Gets the containing <see cref="EPackage"/>
        /// </summary>
        public EPackage EPackage => (EPackage)this.EContainer!;

        /// <summary>
        /// Gets the hierarchy of  containing <see cref="EPackage"/>
        /// </summary>
        public IEnumerable<EPackage> EPackageTree
        {
            get
            {
                var tree = new List<EPackage>();
                var package = this.EPackage;
                while (package != null)
                {
                    tree.Add(package);
                    package = package.ESuperPackage;
                }

                tree.Reverse();
                return tree;
            }
        }

        /// <summary>
        /// Set the properties of this <see cref="EClassifier"/>
        /// </summary>
        internal override void SetProperties()
        {
            this.logger.LogTrace("setting properties of EClassifier {0}:{1}", this.Identifier, this.Name);

            base.SetProperties();

            if (this.Attributes.TryGetValue("instanceClassName", out var output))
            {
                this.InstanceClassName = output;
            }

            if (this.Attributes.TryGetValue("instanceTypeName", out output))
            {
                this.InstanceTypeName = output;
            }
        }

        /// <summary>
        /// Instantiate the type parameters declared by this <see cref="EClassifier"/> from the current node
        /// of the <see cref="XmlNode"/>
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

            if (reader.Name == "eTypeParameters" && reader.NodeType == XmlNodeType.Element)
            {
                var typeParameter = new ETypeParameter(this.EResource, this.loggerFactory);
                this.ETypeParameters.Add(typeParameter);
                typeParameter.ReadXml(reader);
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
            return $"{this.EPackage.Identifier}/{this.Name}";
        }
    }
}
