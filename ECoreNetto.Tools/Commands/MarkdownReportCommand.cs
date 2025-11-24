// -------------------------------------------------------------------------------------------------
// <copyright file="MarkdownReportCommand.cs" company="Starion Group S.A">
// 
//   Copyright 2017-2025 Starion Group S.A.
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
        /// Initializes a new instance of the <see cref="XlReportCommand"/>
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
            /// Initializes a nwe instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="markdownReportGenerator">
            /// The (injected) <see cref="IMarkdownReportGenerator"/> that is used to generate the
            /// excel report
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
