// ------------------------------------------------------------------------------------------------
// <copyright file="ReportCommand.cs" company="Starion Group S.A.">
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

    using Serilog.Events;

    /// <summary>
    /// Abstract super class from which all report commands shall inherit
    /// </summary>
    public abstract class ReportCommand : Command
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportCommand"/>
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">The description of the command, shown in help.</param>
        protected ReportCommand(string name, string? description = null) : base(name, description)
        {
            var noLogoOption = new Option<bool>(name: "--no-logo")
            {
                Description = "Suppress the logo",
                DefaultValueFactory = parseResult => false,
            };

            this.Add(noLogoOption);

            var logLevelOption = new Option<LogEventLevel>("--log-level")
            {
                Description = "Sets the logging level (Trace, Debug, Information, Warning, Error, Critical)",
                Required = false,
                DefaultValueFactory = parseResult => LogEventLevel.Information
            };

            this.Options.Add(logLevelOption);

            var inputModelFileOption = new Option<FileInfo>(name: "--input-model")
            {
                Description = "The path to the ecore file",
                DefaultValueFactory = parseResult => new FileInfo("model.ecore"),
                Required = false
            };

            inputModelFileOption.Aliases.Add("-i");
            this.Add(inputModelFileOption);

            var inputDirectoryOption = new Option<DirectoryInfo?>(name: "--input-directory")
            {
                Description = "The path to a directory of .ecore files; produces a single combined report for every model in the directory",
                DefaultValueFactory = parseResult => null,
                Required = false
            };

            inputDirectoryOption.Aliases.Add("-d");
            this.Add(inputDirectoryOption);

            var autoOpenReportOption = new Option<bool>(name: "--auto-open-report")
            {
                Description = "Open the generated report with its default application",
                DefaultValueFactory = parseResult => false,
                Required = false
            };

            autoOpenReportOption.Aliases.Add("-a");
            this.Add(autoOpenReportOption);

            var includeReferencedModelsOption = new Option<bool>(name: "--include-referenced-models")
            {
                Description = "Produce a single combined report that also includes every cross-referenced .ecore model reachable from the input model",
                DefaultValueFactory = parseResult => false,
                Required = false
            };

            includeReferencedModelsOption.Aliases.Add("-r");
            this.Add(includeReferencedModelsOption);
        }
    }
}
