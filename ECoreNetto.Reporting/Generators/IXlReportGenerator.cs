// ------------------------------------------------------------------------------------------------
// <copyright file="IXlReportGenerator.cs" company="Starion Group S.A.">
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
    /// The purpose of the <see cref="IXlReportGenerator"/> is to generate reports of an
    /// Ecore Model
    /// </summary>
    public interface IXlReportGenerator : IReportGenerator
    {
        /// <summary>
        /// Generates a single combined Excel report of every <c>.ecore</c> model in the provided directory and
        /// writes it to the provided <paramref name="outputPath"/>.
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
