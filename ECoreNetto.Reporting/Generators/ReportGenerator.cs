// ------------------------------------------------------------------------------------------------
// <copyright file="ReportGenerator.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Reporting.Generators
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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
        private readonly ILoggerFactory? loggerFactory;

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
        protected ReportGenerator(ILoggerFactory? loggerFactory = null)
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
            if (modelPath == null)
            {
                throw new ArgumentNullException(nameof(modelPath));
            }

            this.logger.LogInformation("Loading Ecore model from {0}", modelPath.FullName);

            var uri = new System.Uri(modelPath.FullName);

            var resourceSet = new ResourceSet(this.loggerFactory);
            var resource = resourceSet.CreateResource(uri);

            var rootPackage = resource.Load(null);

            return rootPackage;
        }

        /// <summary>
        /// Loads the entry Ecore model together with every cross-referenced model that is demand-loaded while
        /// resolving it, and returns the root <see cref="EPackage"/> of every resource in the resulting
        /// <see cref="ResourceSet"/>.
        /// </summary>
        /// <param name="modelPath">
        /// the path to the entry Ecore model that is to be loaded.
        /// </param>
        /// <returns>
        /// A read-only list of the root <see cref="EPackage"/>s of the entry model and of every model that is
        /// reachable from it through cross-references. The entry model's root package is first.
        /// </returns>
        protected IReadOnlyList<EPackage> LoadRootPackages(FileInfo modelPath)
        {
            if (modelPath == null)
            {
                throw new ArgumentNullException(nameof(modelPath));
            }

            this.logger.LogInformation("Loading Ecore model and referenced models from {0}", modelPath.FullName);

            var resourceSet = new ResourceSet(this.loggerFactory);
            var uri = new System.Uri(modelPath.FullName);
            var resource = resourceSet.CreateResource(uri);
            resource.Load(null);

            return QueryRootPackages(resourceSet);
        }

        /// <summary>
        /// Loads every <c>.ecore</c> file in the provided <paramref name="inputDirectory"/> into a single
        /// <see cref="ResourceSet"/> and returns the root <see cref="EPackage"/> of every resource, so that a
        /// single report can be produced for a multi-file metamodel.
        /// </summary>
        /// <param name="inputDirectory">
        /// the directory that contains the <c>.ecore</c> files that are to be loaded.
        /// </param>
        /// <returns>
        /// A read-only list of the root <see cref="EPackage"/>s of every loaded model.
        /// </returns>
        protected IReadOnlyList<EPackage> LoadRootPackages(DirectoryInfo inputDirectory)
        {
            if (inputDirectory == null)
            {
                throw new ArgumentNullException(nameof(inputDirectory));
            }

            this.logger.LogInformation("Loading all Ecore models from directory {0}", inputDirectory.FullName);

            var resourceSet = new ResourceSet(this.loggerFactory);

            foreach (var file in inputDirectory.EnumerateFiles("*.ecore"))
            {
                var uri = new System.Uri(file.FullName);

                // demand-load so files already pulled in through cross-references are not loaded twice
                resourceSet.Resource(uri, true);
            }

            return QueryRootPackages(resourceSet);
        }

        /// <summary>
        /// Queries the root <see cref="EPackage"/> of every resource contained in the provided
        /// <see cref="ResourceSet"/>.
        /// </summary>
        /// <param name="resourceSet">
        /// the <see cref="ResourceSet"/> whose resources' root packages are collected.
        /// </param>
        /// <returns>
        /// A read-only list of the root <see cref="EPackage"/>s, in resource order.
        /// </returns>
        private static IReadOnlyList<EPackage> QueryRootPackages(ResourceSet resourceSet)
        {
            return resourceSet.Resources
                .SelectMany(resource => resource.Contents.OfType<EPackage>())
                .ToList();
        }
    }
}
