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
        /// /// the path to the Ecore model of which the report is to be generated.
        /// </param>
        /// <returns>
        /// the content of an HTML report in a string
        /// </returns>
        public string GenerateReport(FileInfo modelPath);
    }
}
