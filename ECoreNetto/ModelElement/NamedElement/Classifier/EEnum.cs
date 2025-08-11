// -------------------------------------------------------------------------------------------------
// <copyright file="EEnum.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
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
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// Represents an enumeration
    /// </summary>
    public class EEnum : EDataType
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<EEnum> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EEnum"/> class
        /// </summary>
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public EEnum(Resource.Resource resource, ILoggerFactory loggerFactory = null) : base(resource, loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            this.logger = this.loggerFactory == null ? NullLogger<EEnum>.Instance : this.loggerFactory.CreateLogger<EEnum>();

            this.ELiterals = new ContainerList<EEnumLiteral>(this);
        }

        /// <summary>
        /// Gets the <see cref="EEnumLiteral"/> of this <see cref="EEnum"/>
        /// </summary>
        public ContainerList<EEnumLiteral> ELiterals { get; private set; }

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

            this.logger.LogTrace("deserializing child nodes of EEnum {0}:{1}", this.Identifier, this.Name);

            base.DeserializeChildNode(reader);

            if (reader.Name == "eLiterals" && reader.NodeType == XmlNodeType.Element)
            {
                var ecoreEnumLiteral = new EEnumLiteral(this.EResource, this.loggerFactory);
                this.ELiterals.Add(ecoreEnumLiteral);
                ecoreEnumLiteral.ReadXml(reader);
            }
        }
    }
}
