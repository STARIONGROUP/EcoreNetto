// -------------------------------------------------------------------------------------------------
// <copyright file="ECoreParser.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2022 RHEA System S.A.
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
    using System.IO;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// The purpose of the <see cref="ECoreParser"/> class is to deserialize an Ecore meta model
    /// </summary>
    internal class ECoreParser
    {
        /// <summary>
        /// The <see cref="Resource.Resource"/> that is populated using the current <see cref="ECoreParser"/>
        /// </summary>
        private Resource.Resource resource;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ECoreParser"/> class.
        /// </summary>
        /// <param name="resource">
        /// The <see cref="Resource.Resource"/> that is populated using the current <see cref="ECoreParser"/>
        /// </param>
        internal ECoreParser(Resource.Resource resource)
        {
            this.resource = resource;
        }

        /// <summary>
        /// Parse an ECore document into a <see cref="EPackage"/>
        /// </summary>
        /// <param name="resource">
        /// The <see cref="Resource.Resource"/> that is being parsed
        /// </param>
        /// <returns>
        /// The top level <see cref="EPackage"/> contained by the <see cref="resource"/>
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// If the source file not found
        /// </exception>
        /// <exception cref="UriFormatException">
        /// An invalid URI was detected
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// An invalid argument was supplied
        /// </exception>
        internal EPackage ParseXml()
        {
            var settings = new XmlReaderSettings();
            var fileInfo = new FileInfo(this.resource.URI.AbsolutePath.Replace("%20", " "));
            var fullPath = Path.GetFullPath(fileInfo.FullName);

            // now read the actual model file
            var xmlReader = XmlReader.Create(fullPath, settings);
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlReader);
            var package = new EPackage(this.resource);
            package.ReadXml(xmlDocument.DocumentElement);
            
            foreach (var modelElement in this.resource.AllContents().ToArray())
            {
                modelElement.SetProperties();
            }

            return package;
        }
    }
}