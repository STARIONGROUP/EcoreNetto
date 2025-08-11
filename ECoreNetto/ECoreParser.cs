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
        private readonly ILoggerFactory loggerFactory;

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
        internal ECoreParser(Resource.Resource resource, ILoggerFactory loggerFactory = null)
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
        /// If the source file not found
        /// </exception>
        internal EPackage ParseXml()
        {
            this.logger.LogDebug("start parsing Ecore file");

            var sw = Stopwatch.StartNew();

            var settings = new XmlReaderSettings();
            var fileInfo = new FileInfo(this.resource.URI.AbsolutePath.Replace("%20", " "));
            var fullPath = Path.GetFullPath(fileInfo.FullName);

            // now read the actual model file
            var xmlReader = XmlReader.Create(fullPath, settings);
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlReader);
            var package = new EPackage(this.resource, this.loggerFactory);
            package.ReadXml(xmlDocument.DocumentElement);
            
            foreach (var modelElement in this.resource.AllContents().ToArray())
            {
                modelElement.SetProperties();
            }

            this.logger.LogDebug("Ecore file parsed in {0} [ms]", sw.ElapsedMilliseconds);

            return package;
        }
    }
}
