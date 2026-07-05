// ------------------------------------------------------------------------------------------------
// <copyright file="HtmlReportCommand.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tools.Commands
{
    using System.CommandLine;
    using System.IO;

    using ECoreNetto.Reporting.Generators;
    using ECoreNetto.Tools.Services;

    /// <summary>
    /// The <see cref="HtmlReportCommand"/> that generates an HTML report
    /// </summary>
    public class HtmlReportCommand : ReportCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlReportCommand"/>
        /// </summary>
        public HtmlReportCommand() : base("html-report", "Generates a html report of the ECore model")
        {
            var reportFileOption = new Option<FileInfo>(name: "--output-report")
            {
                Description = "The path to the html report file. Supported extensions are '.html'",
                DefaultValueFactory = parseResult => new FileInfo("html-report.html"),
                Required = true
            };

            reportFileOption.Aliases.Add("-o");

            this.Add(reportFileOption);
        }

        /// <summary>
        /// The Command Handler of the <see cref="HtmlReportCommand"/>
        /// </summary>
        public class Handler : ReportHandler
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="htmlReportGenerator">
            /// The (injected) <see cref="IHtmlReportGenerator"/> that is used to generate the
            /// html report
            /// </param>
            /// <param name="versionChecker">
            /// The <see cref="IVersionChecker"/> used to check the github version
            /// </param>
            public Handler(IHtmlReportGenerator htmlReportGenerator, IVersionChecker versionChecker)
                : base(htmlReportGenerator, versionChecker)
            {
            }
        }
    }
}
