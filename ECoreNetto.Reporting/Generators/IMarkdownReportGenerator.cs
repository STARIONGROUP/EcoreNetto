// ------------------------------------------------------------------------------------------------
// <copyright file="IMarkdownReportGenerator.cs" company="Starion Group S.A.">
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
    /// The purpose of the <see cref="IMarkdownReportGenerator"/> is to generate a Markdown report of an
    /// Ecore Model
    /// </summary>
    public interface IMarkdownReportGenerator : IReportGenerator
    {
        /// <summary>
        /// Generates a Markdown document with a table that contains all classes, attributes and their documentation
        /// </summary>
        /// <param name="modelPath">
        /// the path to the Ecore model of which the report is to be generated.
        /// </param>
        /// <returns>
        /// the content of a Markdown report in a string
        /// </returns>
        public string GenerateReport(FileInfo modelPath);

        /// <summary>
        /// Generates a single combined Markdown report of the entry model together with every cross-referenced
        /// model that is reachable from it.
        /// </summary>
        /// <param name="modelPath">
        /// the path to the entry Ecore model of which the combined report is to be generated.
        /// </param>
        /// <returns>
        /// the content of a Markdown report in a string
        /// </returns>
        public string GenerateCombinedReport(FileInfo modelPath);

        /// <summary>
        /// Generates a single combined Markdown report of every <c>.ecore</c> model in the provided directory.
        /// </summary>
        /// <param name="inputDirectory">
        /// the directory that contains the <c>.ecore</c> models of which the combined report is to be generated.
        /// </param>
        /// <returns>
        /// the content of a Markdown report in a string
        /// </returns>
        public string GenerateCombinedReport(DirectoryInfo inputDirectory);

        /// <summary>
        /// Generates a single combined Markdown report of every <c>.ecore</c> model in the provided directory
        /// and writes it to the provided <paramref name="outputPath"/>.
        /// </summary>
        /// <param name="inputDirectory">
        /// the directory that contains the <c>.ecore</c> models of which the combined report is to be generated.
        /// </param>
        /// <param name="outputPath">
        /// the path, including filename, where the output is to be generated.
        /// </param>
        public void GenerateCombinedReport(DirectoryInfo inputDirectory, FileInfo outputPath);
    }
}
