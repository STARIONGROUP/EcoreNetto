// ------------------------------------------------------------------------------------------------
// <copyright file="ReportHandler.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tools.Commands
{
    using System;
    using System.CommandLine;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using ECoreNetto.Reporting.Generators;
    using ECoreNetto.Tools.Resources;
    using ECoreNetto.Tools.Services;

    using Spectre.Console;

    /// <summary>
    /// Abstract super class from which all Report <see cref="ReportHandler"/>s need to derive
    /// </summary>
    public abstract class ReportHandler
    {
        /// <summary>
        /// Gets or sets the delay between Spectre status updates. This keeps the themed status
        /// messages briefly visible on the console during a real run. Tests set this to
        /// <see cref="TimeSpan.Zero"/> so the generation path runs without artificial latency.
        /// </summary>
        public TimeSpan StatusDelay { get; internal set; } = TimeSpan.FromMilliseconds(1500);

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportHandler"/>
        /// </summary>
        /// <param name="reportGenerator">
        /// The <see cref="IReportGenerator"/> used to generate an ECore report
        /// </param>
        /// <param name="versionChecker">
        /// The <see cref="IVersionChecker"/> used to check the github version
        /// </param>
        protected ReportHandler(IReportGenerator reportGenerator, IVersionChecker versionChecker)
        {
            this.ReportGenerator = reportGenerator ?? throw new ArgumentNullException(nameof(reportGenerator));
            this.VersionChecker = versionChecker;
        }

        /// <summary>
        /// Gets or sets the <see cref="IReportGenerator"/> used to generate an ECore report
        /// </summary>
        public IReportGenerator ReportGenerator { get; private set; }

        /// <summary>
        /// The <see cref="IVersionChecker"/> used to check the github version
        /// </summary>
        public IVersionChecker VersionChecker { get; private set; }

        /// <summary>
        /// The value indicating whether the logo should be shown or not
        /// </summary>
        private bool noLogo;

        /// <summary>
        /// The <see cref="FileInfo"/> where the ecore model is located that is to be read
        /// </summary>
        private FileInfo inputModel = null!;

        /// <summary>
        /// The optional <see cref="DirectoryInfo"/> of a directory of <c>.ecore</c> models; when set a single
        /// combined report is generated for every model in the directory.
        /// </summary>
        private DirectoryInfo? inputDirectory;

        /// <summary>
        /// The <see cref="FileInfo"/> where the inspection report is to be generated
        /// </summary>
        private FileInfo outputReport = null!;

        /// <summary>
        /// The value indicating whether the generated report needs to be automatically be
        /// opened once generated.
        /// </summary>
        private bool autoOpenReport;

        /// <summary>
        /// The value indicating whether the report should also include every cross-referenced model that is
        /// reachable from the input model (a single combined report).
        /// </summary>
        private bool includeReferencedModels;

        /// <summary>
        /// Asynchronously invokes the <see cref="ReportHandler"/>
        /// </summary>
        /// <param name="parseResult">
        /// The <see cref="ParseResult"/> that carries the parsed command line arguments
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> used to cancel the operation
        /// </param>
        /// <returns>
        /// 0 when successful, another if not
        /// </returns>
        public async Task<int> InvokeAsync(ParseResult parseResult, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            await this.VersionChecker.ExecuteAsync(cancellationToken);

            this.ProcessParseResult(parseResult);

            if (!this.InputValidation())
            {
                return -1;
            }

            var isValidExtension = this.ReportGenerator.IsValidReportExtension(this.outputReport);
            if (!isValidExtension.Item1)
            {
                AnsiConsole.WriteLine("");
                AnsiConsole.MarkupLine($"[red] {isValidExtension.Item2} [/]");
                AnsiConsole.MarkupLine($"[purple]{this.inputModel.FullName}[/]");
                AnsiConsole.WriteLine("");
                return -1;
            }

            try
            {
                await AnsiConsole.Status()
                    .AutoRefresh(true)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Preparing Warp Engines for {this.ReportGenerator.QueryReportType()} reporting...", async ctx =>
                    {
                        await Task.Delay(this.StatusDelay, cancellationToken);

                        ctx.Status($"Generating Ecore Model report at Warp 11, Captain..., SLOW DOWN!");

                        await Task.Delay(this.StatusDelay, cancellationToken);

                        if (this.inputDirectory != null)
                        {
                            this.ReportGenerator.GenerateCombinedReport(this.inputDirectory, this.outputReport);
                        }
                        else if (this.includeReferencedModels)
                        {
                            this.ReportGenerator.GenerateCombinedReport(this.inputModel, this.outputReport);
                        }
                        else
                        {
                            this.ReportGenerator.GenerateReport(this.inputModel, this.outputReport);
                        }

                        AnsiConsole.MarkupLine(
                            $"[grey]LOG:[/] Ecore {this.ReportGenerator.QueryReportType()} report generated at [bold]{this.outputReport.FullName}[/]");
                        await Task.Delay(this.StatusDelay, cancellationToken);

                        await this.ExecuteAutoOpenAsync(ctx, cancellationToken);

                        ctx.Status("[green]Dropping to impulse speed[/]");
                        await Task.Delay(this.StatusDelay, cancellationToken);
                    });
            }
            catch (IOException ex)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[red]The report file could not be generated or opened. Make sure the file is not open and try again.[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[green]Dropping to impulse speed[/]");
                AnsiConsole.WriteLine();
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[red]An exception occurred[/]");
                AnsiConsole.MarkupLine("[green]Dropping to impulse speed[/]");
                AnsiConsole.MarkupLine("[red]please report an issue at[/]");
                AnsiConsole.MarkupLine("[link] https://github.com/STARIONGROUP/EcoreNetto/issues [/]");
                AnsiConsole.WriteLine();
                AnsiConsole.WriteException(ex);

                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Process the <see cref="ParseResult"/> and set to the associated properties
        /// </summary>
        /// <param name="parseResult">
        /// The instance of <see cref="ParseResult"/> that contains the parsed commandline arguments
        /// </param>
        private void ProcessParseResult(ParseResult parseResult)
        {
            this.noLogo = parseResult.GetValue<bool>("--no-logo");
            this.inputModel = parseResult.GetValue<FileInfo>("--input-model")!;
            this.autoOpenReport = parseResult.GetValue<bool>("--auto-open-report");
            this.outputReport = parseResult.GetValue<FileInfo>("--output-report")!;
            this.includeReferencedModels = parseResult.GetValue<bool>("--include-referenced-models");
            this.inputDirectory = parseResult.GetValue<DirectoryInfo?>("--input-directory");
        }

        /// <summary>
        /// validates the options
        /// </summary>
        /// <returns>
        /// 0 when successful, -1 when not
        /// </returns>
        protected bool InputValidation()
        {
            if (!this.noLogo)
            {
                AnsiConsole.Markup($"[blue]{ResourceLoader.QueryLogo()}[/]");
            }

            if (this.inputDirectory != null)
            {
                if (!this.inputDirectory.Exists)
                {
                    AnsiConsole.WriteLine("");
                    AnsiConsole.MarkupLine($"[red]The specified input directory does not exist[/]");
                    AnsiConsole.MarkupLine($"[purple]{this.inputDirectory.FullName}[/]");
                    AnsiConsole.WriteLine("");
                    return false;
                }

                return true;
            }

            if (!this.inputModel.Exists)
            {
                AnsiConsole.WriteLine("");
                AnsiConsole.MarkupLine($"[red]The specified input ecore model does not exist[/]");
                AnsiConsole.MarkupLine($"[purple]{this.inputModel.FullName}[/]");
                AnsiConsole.WriteLine("");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Automatically opens the generated report
        /// </summary>
        /// <param name="ctx">
        /// Spectre Console <see cref="StatusContext"/>
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> used to cancel the operation
        /// </param>
        protected async Task ExecuteAutoOpenAsync(StatusContext ctx, CancellationToken cancellationToken)
        {
            if (this.autoOpenReport)
            {
                ctx.Status($"Opening generated report");
                await Task.Delay(this.StatusDelay, cancellationToken);

                try
                {
                    Process.Start(new ProcessStartInfo(this.outputReport.FullName)
                        { UseShellExecute = true });
                    ctx.Status($"Generated report opened");
                }
                catch
                {
                    ctx.Status($"Opening of generated report failed, please open manually");
                    await Task.Delay(this.StatusDelay, cancellationToken);
                }
            }
        }
    }
}
