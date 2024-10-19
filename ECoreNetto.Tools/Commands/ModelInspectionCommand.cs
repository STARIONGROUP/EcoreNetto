﻿// -------------------------------------------------------------------------------------------------
// <copyright file="ModelInspectionCommand.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Tools.Commands
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;

    using ECoreNetto.Reporting.Generators;
    
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
            var reportFileOption = new Option<FileInfo>(
                name: "--output-report",
                description: "The path to the text report file. Supported extensions is '.txt'",
                getDefaultValue: () => new FileInfo("inspection-report.txt"));
            reportFileOption.AddAlias("-o");
            reportFileOption.IsRequired = true;
            this.AddOption(reportFileOption);
        }

        /// <summary>
        /// The Command Handler of the <see cref="XlReportCommand"/>
        /// </summary>
        public new class Handler : ReportHandler, ICommandHandler
        {
            /// <summary>
            /// Initializes a nwe instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="modelInspector">
            /// The (injected) <see cref="IModelInspector"/> that is used to generate the
            /// inspection report
            /// </param>
            public Handler(IModelInspector modelInspector) : base(modelInspector)
            {
            }
        }
    }
}