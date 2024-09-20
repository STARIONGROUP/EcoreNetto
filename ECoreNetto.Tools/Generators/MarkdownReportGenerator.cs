// -------------------------------------------------------------------------------------------------
// <copyright file="MarkdownReportGenerator.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Tools.Generators
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using ECoreNetto.Extensions;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="MarkdownReportGenerator"/> is to generate a Markdown report of an
    /// Ecore Model
    /// </summary>
    public class MarkdownReportGenerator : HandleBarsReportGenerator, IMarkdownReportGenerator
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<MarkdownReportGenerator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReportGenerator"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public MarkdownReportGenerator(ILoggerFactory loggerFactory = null) : base(loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<MarkdownReportGenerator>.Instance : loggerFactory.CreateLogger<MarkdownReportGenerator>();
        }

        /// <summary>
        /// Generates a Markdown document with a table that contains all classes, attributes and their documentation
        /// </summary>
        /// <param name="modelPath">
        /// the path to the Ecore model of which the report is to be generated.
        /// </param>
        /// <param name="outputPath">
        /// the path, including filename, where the output is to be generated.
        /// </param>
        public void GenerateReport(FileInfo modelPath, FileInfo outputPath)
        {
            ArgumentNullException.ThrowIfNull(modelPath);

            ArgumentNullException.ThrowIfNull(outputPath);

            var sw = Stopwatch.StartNew();

            this.logger.LogInformation("Start Generating Markdown report");

            var template = this.Templates["ecore-to-markdown-docs"];

            var rootPackage = this.LoadRootPackage(modelPath);

            var payload = CreatePayload(rootPackage);

            var generatedHtml = template(payload);

            if (outputPath.Exists)
            {
                outputPath.Delete();
            }

            using var writer = outputPath.CreateText();
            writer.Write(generatedHtml);

            this.logger.LogInformation("Generated Markdown report in {ElapsedTime} [ms]", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Creates a <see cref="GeneratorPayload"/> based on the provided root <see cref="EPackage"/>
        /// </summary>
        /// <param name="rootPackage">
        /// the subject root <see cref="EPackage"/>
        /// </param>
        /// <returns>
        /// an instance of <see cref="GeneratorPayload"/>
        /// </returns>
        private static GeneratorPayload CreatePayload(EPackage rootPackage)
        {
            var packages = rootPackage.QueryPackages();

            var enums = new List<EEnum>();
            var dataTypes = new List<EDataType>();
            var eClasses = new List<EClass>();

            foreach (var package in packages)
            {
                enums.AddRange(package.EClassifiers.OfType<EEnum>());

                dataTypes.AddRange(package.EClassifiers.OfType<EDataType>());
                dataTypes.RemoveAll(x => x.GetType() == typeof(EEnum));

                eClasses.AddRange(package.EClassifiers.OfType<EClass>());
            }

            var orderedEnums = enums.OrderBy(x => x.Name);
            var orderedDataTypes = dataTypes.OrderBy(x => x.Name);
            var orderedClasses = eClasses.OrderBy(x => x.Name);

            var payload = new GeneratorPayload(rootPackage, orderedEnums, orderedDataTypes, orderedClasses);

            return payload;
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
        public override Tuple<bool, string> IsValidReportExtension(FileInfo outputPath)
        {
            if (outputPath.Extension == ".md")
            {
                return new Tuple<bool, string>(true, ".md is a supported report extension");
            }

            return new Tuple<bool, string>(false,
                $"The Extension of the output file '{outputPath.Extension}' is not supported. Supported extensions is '.md'");
        }

        /// <summary>
        /// Register the custom helpers
        /// </summary>
        protected override void RegisterHelpers()
        {
            ECoreNetto.HandleBars.StringHelper.RegisterStringHelper(this.Handlebars);
            ECoreNetto.HandleBars.StructuralFeatureHelper.RegisterStructuralFeatureHelper(this.Handlebars);
            ECoreNetto.HandleBars.GeneralizationHelper.RegisterGeneralizationHelper(this.Handlebars);
            ECoreNetto.HandleBars.DocumentationHelper.RegisteredDocumentationHelper(this.Handlebars);
        }

        /// <summary>
        /// Register the code templates
        /// </summary>
        protected override void RegisterTemplates()
        {
            this.RegisterEmbeddedTemplate("ecore-to-markdown-docs");
        }
    }
}
