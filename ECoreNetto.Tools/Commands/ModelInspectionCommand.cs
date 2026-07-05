// ------------------------------------------------------------------------------------------------
// <copyright file="ModelInspectionCommand.cs" company="Starion Group S.A.">
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
        /// The Command Handler of the <see cref="ModelInspectionCommand"/>
        /// </summary>
        public  class Handler : ReportHandler
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Handler"/> class.
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
