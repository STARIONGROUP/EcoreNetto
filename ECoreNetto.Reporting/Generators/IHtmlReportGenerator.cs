// ------------------------------------------------------------------------------------------------
// <copyright file="IHtmlReportGenerator.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Reporting.Generators
{
    using System.IO;

    /// <summary>
    /// The purpose of the <see cref="IHtmlReportGenerator"/> is to generate an HTML report of an
    /// Ecore Model
    /// </summary>
    public interface IHtmlReportGenerator : IReportGenerator
    {
        /// <summary>
        /// Generates a table that contains all classes, attributes and their documentation
        /// </summary>
        /// <param name="modelPath">
        /// the path to the Ecore model of which the report is to be generated.
        /// </param>
        /// <param name="customHtml">
        /// optional custom HTML that is injected into the report at the custom-HTML injection point.
        /// </param>
        /// <returns>
        /// the content of an HTML report in a string
        /// </returns>
        public string GenerateReport(FileInfo modelPath, string customHtml = "");

        /// <summary>
        /// Generates an HTML report and writes it to the provided <paramref name="outputPath"/>.
        /// </summary>
        /// <param name="modelPath">
        /// the path to the Ecore model of which the report is to be generated.
        /// </param>
        /// <param name="outputPath">
        /// the path, including filename, where the output is to be generated.
        /// </param>
        /// <param name="customHtml">
        /// custom HTML that is injected into the report at the custom-HTML injection point; pass
        /// <see cref="string.Empty"/> when none is required.
        /// </param>
        public void GenerateReport(FileInfo modelPath, FileInfo outputPath, string customHtml);

        /// <summary>
        /// Generates a single combined HTML report of the entry model together with every cross-referenced
        /// model that is reachable from it.
        /// </summary>
        /// <param name="modelPath">
        /// the path to the entry Ecore model of which the combined report is to be generated.
        /// </param>
        /// <param name="customHtml">
        /// optional custom HTML that is injected into the report at the custom-HTML injection point.
        /// </param>
        /// <returns>
        /// the content of an HTML report in a string
        /// </returns>
        public string GenerateCombinedReport(FileInfo modelPath, string customHtml = "");

        /// <summary>
        /// Generates a single combined HTML report of every <c>.ecore</c> model in the provided directory.
        /// </summary>
        /// <param name="inputDirectory">
        /// the directory that contains the <c>.ecore</c> models of which the combined report is to be generated.
        /// </param>
        /// <param name="customHtml">
        /// optional custom HTML that is injected into the report at the custom-HTML injection point.
        /// </param>
        /// <returns>
        /// the content of an HTML report in a string
        /// </returns>
        public string GenerateCombinedReport(DirectoryInfo inputDirectory, string customHtml = "");

        /// <summary>
        /// Generates a single combined HTML report of every <c>.ecore</c> model in the provided directory and
        /// writes it to the provided <paramref name="outputPath"/>.
        /// </summary>
        /// <param name="inputDirectory">
        /// the directory that contains the <c>.ecore</c> models of which the combined report is to be generated.
        /// </param>
        /// <param name="outputPath">
        /// the path, including filename, where the output is to be generated.
        /// </param>
        /// <param name="customHtml">
        /// custom HTML that is injected into the report at the custom-HTML injection point; pass
        /// <see cref="string.Empty"/> when none is required.
        /// </param>
        public void GenerateCombinedReport(DirectoryInfo inputDirectory, FileInfo outputPath, string customHtml);
    }
}
