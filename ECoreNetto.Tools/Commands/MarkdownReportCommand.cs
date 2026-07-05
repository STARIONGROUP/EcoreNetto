// ------------------------------------------------------------------------------------------------
// <copyright file="MarkdownReportCommand.cs" company="Starion Group S.A.">
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
    /// The <see cref="MarkdownReportCommand"/> that generates a Markdown report
    /// </summary>
    public class MarkdownReportCommand : ReportCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownReportCommand"/>
        /// </summary>
        public MarkdownReportCommand() : base("md-report", "Generates a Markdown report of the ECore model")
        {
            var reportFileOption = new Option<FileInfo>(name: "--output-report")
            {
                Description = "The path to the markdown report file. Supported extensions are '.md'",
                DefaultValueFactory = parseResult => new FileInfo("md-report.md"),
                Required = true,
            };

            reportFileOption.Aliases.Add("-o");
            
            this.Add(reportFileOption);
        }

        /// <summary>
        /// The Command Handler of the <see cref="MarkdownReportCommand"/>
        /// </summary>
        public class Handler : ReportHandler
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="markdownReportGenerator">
            /// The (injected) <see cref="IMarkdownReportGenerator"/> that is used to generate the
            /// markdown report
            /// </param>
            /// <param name="versionChecker">
            /// The <see cref="IVersionChecker"/> used to check the github version
            /// </param>
            public Handler(IMarkdownReportGenerator markdownReportGenerator, IVersionChecker versionChecker) 
                : base(markdownReportGenerator, versionChecker)
            {
            }
        }
    }
}
