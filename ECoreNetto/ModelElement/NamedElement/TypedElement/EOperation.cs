// -------------------------------------------------------------------------------------------------
// <copyright file="EOperation.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2020 System S.A.
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
    using System;
    using System.Collections.Generic;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The ECore operation class.
    /// </summary>
    public class EOperation : ETypedElement
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory? loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<EOperation> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EOperation"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public EOperation(Resource.Resource resource, ILoggerFactory? loggerFactory = null) : base(resource, loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            this.logger = this.loggerFactory == null ? NullLogger<EOperation>.Instance : this.loggerFactory.CreateLogger<EOperation>();

            this.EParameters = new ContainerList<EParameter>(this);
            this.EExceptions = new List<EClassifier>();
            this.ETypeParameters = new ContainerList<ETypeParameter>(this);
            this.EGenericExceptions = new ContainerList<EGenericType>(this);
        }

        /// <summary>
        /// Gets the <see cref="EParameter"/>s of this <see cref="EOperation"/>
        /// </summary>
        public ContainerList<EParameter> EParameters { get; private set; }

        /// <summary>
        /// Gets the exceptions that may be thrown by this <see cref="EOperation"/>
        /// </summary>
        public List<EClassifier> EExceptions { get; private set; }

        /// <summary>
        /// Gets the type parameters (type variables) declared by this <see cref="EOperation"/>.
        /// </summary>
        public ContainerList<ETypeParameter> ETypeParameters { get; private set; }

        /// <summary>
        /// Gets the generic exceptions that may be thrown by this <see cref="EOperation"/> (exceptions
        /// expressed generically).
        /// </summary>
        /// <remarks>
        /// The raw <see cref="EClassifier"/> of each generic exception is also added to
        /// <see cref="EExceptions"/>, so the erased exception view remains complete.
        /// </remarks>
        public ContainerList<EGenericType> EGenericExceptions { get; private set; }

        /// <summary>
        /// Gets the containing <see cref="EClass"/>
        /// </summary>
        public EClass EContainingClass => (EClass)this.EContainer!;

        /// <summary>
        /// Returns whether this operation is an override of some other operation
        /// </summary>
        /// <param name="someOperation">
        /// Some other operation.
        /// </param>
        /// <returns>
        /// whether this operation is an override of some other operation.
        /// </returns>
        public bool IsOverrideOf(EOperation someOperation)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read the attributes of the current node
        /// </summary>
        internal override void SetProperties()
        {
            this.logger.LogTrace("setting properties of EOperation {0}:{1}", this.Identifier, this.Name);

            base.SetProperties();

            if (this.Attributes.TryGetValue("eExceptions", out var output))
            {
                var exceptionNames = output.Split(' ');
                foreach (var exceptionName in exceptionNames)
                {
                    this.EExceptions.Add(this.EResource.GetEObject<EClassifier>(exceptionName));
                }
            }

            foreach (var genericException in this.EGenericExceptions)
            {
                genericException.SetProperties();

                // keep the erased exception view complete: add the raw classifier of each generic exception
                if (genericException.EClassifier != null && !this.EExceptions.Contains(genericException.EClassifier))
                {
                    this.EExceptions.Add(genericException.EClassifier);
                }
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
            return $"EOperation::{this.EContainingClass.Identifier}/{this.Name}";
        }

        /// <summary>
        /// Instantiate new <see cref="EModelElement"/> from the current node of the <see cref="XmlNode"/>
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

            this.logger.LogTrace("deserializing child nodes of EPackage {0}:{1}", this.Identifier, this.Name);

            base.DeserializeChildNode(reader);

            if (reader.Name == "eParameters" && reader.NodeType == XmlNodeType.Element)
            {
                var parameter = new EParameter(this.EResource, this.loggerFactory);
                this.EParameters.Add(parameter);
                parameter.ReadXml(reader);
            }

            if (reader.Name == "eTypeParameters" && reader.NodeType == XmlNodeType.Element)
            {
                var typeParameter = new ETypeParameter(this.EResource, this.loggerFactory);
                this.ETypeParameters.Add(typeParameter);
                typeParameter.ReadXml(reader);
            }

            if (reader.Name == "eGenericExceptions" && reader.NodeType == XmlNodeType.Element)
            {
                var genericException = new EGenericType(this.EResource, this.loggerFactory);
                this.EGenericExceptions.Add(genericException);
                genericException.ReadXml(reader);
            }
        }
    }
}
