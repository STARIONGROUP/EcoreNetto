// -------------------------------------------------------------------------------------------------
// <copyright file="ReportGenerator.cs" company="Starion Group S.A">
// 
//   Copyright 2017-2024 Starion Group S.A.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Reporting.Generators
{
    using System.IO;

    using ECoreNetto.Resource;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using System;

    /// <summary>
    /// abstract class from which all report generators need to derive
    /// </summary>
    public abstract class ReportGenerator
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<ReportGenerator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportGenerator"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected ReportGenerator(ILoggerFactory loggerFactory = null)
        {
            this.loggerFactory = loggerFactory;

            this.logger = this.loggerFactory == null ? NullLogger<ReportGenerator>.Instance : this.loggerFactory.CreateLogger<ReportGenerator>();
        }

        /// <summary>
        /// Verifies whether the extension of the <paramref name="outputPath"/> is valid or not
        /// </summary>
        /// <param name="outputPath">
        /// The subject <see cref="FileInfo"/> to check
        /// </param>
        /// <returns>
        /// A Tuple of bool and string, where the string contains a description of the verification.
        /// Either stating that the extension is valid or not.
        /// </returns>
        public abstract Tuple<bool, string> IsValidReportExtension(FileInfo outputPath);

        /// <summary>
        /// Loads the root Ecore package from the provided model
        /// </summary>
        /// <param name="modelPath">
        /// the path to the Ecore model that is to be loaded
        /// </param>
        /// <returns>
        /// an instance of <see cref="EPackage"/>
        /// </returns>
        protected EPackage LoadRootPackage(FileInfo modelPath)
        {
            this.logger.LogInformation("Loading Ecore model from {0}", modelPath.FullName);

            var uri = new System.Uri(modelPath.FullName);

            var resourceSet = new ResourceSet(this.loggerFactory);
            var resource = resourceSet.CreateResource(uri);

            var rootPackage = resource.Load(null);

            return rootPackage;
        }
    }
}
