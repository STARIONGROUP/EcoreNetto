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
    using System.CommandLine.Invocation;
    using System.IO;

    using ECoreNetto.Reporting.Generators;
    
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
            var reportFileOption = new Option<FileInfo>(
                name: "--output-report",
                description: "The path to the markdown report file. Supported extensions are '.md'",
                getDefaultValue: () => new FileInfo("md-report.md"));
            reportFileOption.AddAlias("-o");
            reportFileOption.IsRequired = true;
            this.AddOption(reportFileOption);
        }

        /// <summary>
        /// The Command Handler of the <see cref="MarkdownReportCommand"/>
        /// </summary>
        public new class Handler : ReportHandler, ICommandHandler
        {
            /// <summary>
            /// Initializes a nwe instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="markdownReportGenerator">
            /// The (injected) <see cref="IMarkdownReportGenerator"/> that is used to generate the
            /// excel report
            /// </param>
            public Handler(IMarkdownReportGenerator markdownReportGenerator) : base(markdownReportGenerator)
            {
            }
        }
    }
}
