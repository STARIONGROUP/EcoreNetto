// -------------------------------------------------------------------------------------------------
// <copyright file="ECoreParser.cs" company="Starion Group S.A.">
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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="ECoreParser"/> class is to deserialize an Ecore model
    /// </summary>
    internal class ECoreParser
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory? loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<ECoreParser> logger;

        /// <summary>
        /// The <see cref="Resource.Resource"/> that is populated using the current <see cref="ECoreParser"/>
        /// </summary>
        private readonly Resource.Resource resource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ECoreParser"/> class.
        /// </summary>
        /// <param name="resource">
        /// The <see cref="Resource.Resource"/> that is populated using the current <see cref="ECoreParser"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        internal ECoreParser(Resource.Resource resource, ILoggerFactory? loggerFactory = null)
        {
            this.loggerFactory = loggerFactory;

            this.logger = this.loggerFactory == null ? NullLogger<ECoreParser>.Instance : this.loggerFactory.CreateLogger<ECoreParser>();

            this.resource = resource;
        }

        /// <summary>
        /// Parse an ECore document into a <see cref="EPackage"/>
        /// </summary>
        /// <returns>
        /// The top level <see cref="EPackage"/> contained by the <see cref="resource"/>
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// If the source file could not be found. A descriptive <see cref="Resource.Diagnostic"/> is recorded in
        /// <see cref="Resource.Resource.Errors"/> before the exception is thrown.
        /// </exception>
        /// <exception cref="XmlException">
        /// If the source file is not well-formed XML. A descriptive <see cref="Resource.Diagnostic"/> is recorded in
        /// <see cref="Resource.Resource.Errors"/> before the exception is re-thrown.
        /// </exception>
        internal EPackage ParseXml()
        {
            this.logger.LogDebug("start parsing Ecore file");

            var sw = Stopwatch.StartNew();

            // harden against XXE: prohibit DTD processing and disable external entity resolution
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null
            };

            // URI.LocalPath yields the unescaped local file path (handles %20 and any other escaped characters)
            var fullPath = Path.GetFullPath(this.resource.URI.LocalPath);

            if (!File.Exists(fullPath))
            {
                // record a descriptive diagnostic before surfacing the documented FileNotFoundException
                var message = $"The Ecore file '{fullPath}' could not be found.";
                this.resource.AddError(message);
                this.logger.LogError(message);

                throw new FileNotFoundException(message, fullPath);
            }

            // now read the actual model file
            var xmlDocument = new XmlDocument { XmlResolver = null };

            try
            {
                using var xmlReader = XmlReader.Create(fullPath, settings);
                xmlDocument.Load(xmlReader);
            }
            catch (XmlException xmlException)
            {
                // record a descriptive diagnostic in Resource.Errors, then re-throw the raw XmlException
                // unchanged so callers still observe the original type (see XxeHardeningTestFixture). We do
                // not also log here: that would be a redundant log-and-rethrow (Sonar S2139).
                var message = $"The Ecore file '{fullPath}' is not well-formed XML and could not be parsed. {xmlException.Message}";
                this.resource.AddError(message);

                throw;
            }

            // an .ecore document root is normally the ecore:EPackage itself, but some tools wrap it in an
            // <xmi:XMI> element. Unwrap that so the root package is read rather than silently returning an
            // empty, nameless package.
            var rootElement = xmlDocument.DocumentElement;

            if (rootElement.LocalName == "XMI")
            {
                var packageElements = rootElement.ChildNodes
                    .OfType<XmlElement>()
                    .Where(element => element.LocalName == "EPackage")
                    .ToList();

                if (packageElements.Count != 1)
                {
                    var message = packageElements.Count == 0
                        ? $"The Ecore file '{fullPath}' has an XMI document root but contains no ecore:EPackage element."
                        : $"The Ecore file '{fullPath}' has an XMI document root wrapping {packageElements.Count} ecore:EPackage elements; multiple root packages in a single resource are not supported.";

                    this.resource.AddError(message);
                    this.logger.LogError(message);

                    throw new InvalidOperationException(message);
                }

                rootElement = packageElements[0];
            }

            var package = new EPackage(this.resource, this.loggerFactory);
            package.ReadXml(rootElement);

            foreach (var modelElement in this.resource.AllContents().ToArray())
            {
                modelElement.SetProperties();
            }

            this.logger.LogDebug("Ecore file parsed in {0} [ms]", sw.ElapsedMilliseconds);

            return package;
        }
    }
}
