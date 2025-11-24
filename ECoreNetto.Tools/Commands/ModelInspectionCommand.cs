// -------------------------------------------------------------------------------------------------
// <copyright file="ModelInspectionCommand.cs" company="Starion Group S.A">
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
    /// The <see cref="ModelInspectionCommand"/> that inspects an ECore model and generates
    /// a text report
    /// </summary>
    public class ModelInspectionCommand : ReportCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelInspectionCommand"/>
        /// </summary>
        public ModelInspectionCommand() : base("inspect", "Inspects an ECore model and generates a text report")
        {
            var reportFileOption = new Option<FileInfo>(name: "--output-report")
            {
                Description = "The path to the text report file. Supported extensions is '.txt'",
                DefaultValueFactory = parseResult => new FileInfo("inspection-report.txt"),
                Required = true
            };

            reportFileOption.Aliases.Add("-o");

            this.Add(reportFileOption);
        }

        /// <summary>
        /// The Command Handler of the <see cref="XlReportCommand"/>
        /// </summary>
        public  class Handler : ReportHandler
        {
            /// <summary>
            /// Initializes a nwe instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="modelInspector">
            /// The (injected) <see cref="IModelInspector"/> that is used to generate the
            /// inspection report
            /// </param>
            /// <param name="versionChecker">
            /// The <see cref="IVersionChecker"/> used to check the github version
            /// </param>
            public Handler(IModelInspector modelInspector, IVersionChecker versionChecker)
                : base(modelInspector, versionChecker)
            {
            }
        }
    }
}
