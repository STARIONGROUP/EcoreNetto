// -------------------------------------------------------------------------------------------------
// <copyright file="ReportHandler.cs" company="Starion Group S.A">
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
    using System;
    using System.CommandLine.Invocation;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using ECoreNetto.Tools.Resources;
    using ECoreNetto.Reporting.Generators;

    using Spectre.Console;
    using System.Threading.Tasks;
    using Resources;

    /// <summary>
    /// Abstract super class from which all Report <see cref="ICommandHandler"/>s need to derive
    /// </summary>
    public abstract class ReportHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportHandler"/>
        /// </summary>
        /// <param name="reportGenerator">
        /// The <see cref="IReportGenerator"/> used to generate an ECore report
        /// </param>
        protected ReportHandler(IReportGenerator reportGenerator)
        {
            this.ReportGenerator = reportGenerator ?? throw new ArgumentNullException(nameof(reportGenerator));
        }

        /// <summary>
        /// Gets or sets the <see cref="IReportGenerator"/> used to generate an ECore report
        /// </summary>
        public IReportGenerator ReportGenerator { get; private set; }

        /// <summary>
        /// Gets or sets the value indicating whether the logo should be shown or not
        /// </summary>
        public bool NoLogo { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FileInfo"/> where the ecore model is located that is to be read
        /// </summary>
        public FileInfo InputModel { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FileInfo"/> where the inspection report is to be generated
        /// </summary>
        public FileInfo OutputReport { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the generated report needs to be automatically be
        /// opened once generated.
        /// </summary>
        public bool AutoOpenReport { get; set; }

        /// <summary>
        /// Invokes the <see cref="ICommandHandler"/>
        /// </summary>
        /// <param name="context">
        /// The <see cref="InvocationContext"/> 
        /// </param>
        /// <returns>
        /// 0 when successful, another if not
        /// </returns>
        public int Invoke(InvocationContext context)
        {
            throw new NotSupportedException("Please use InvokeAsync");
        }

        /// <summary>
        /// Asynchronously invokes the <see cref="ICommandHandler"/>
        /// </summary>
        /// <param name="context">
        /// The <see cref="InvocationContext"/> 
        /// </param>
        /// <returns>
        /// 0 when successful, another if not
        /// </returns>
        public async Task<int> InvokeAsync(InvocationContext context)
        {
            if (!this.InputValidation())
            {
                return -1;
            }

            var isValidExtension = this.ReportGenerator.IsValidReportExtension(this.OutputReport);
            if (!isValidExtension.Item1)
            {
                AnsiConsole.WriteLine("");
                AnsiConsole.MarkupLine($"[red] {isValidExtension.Item2} [/]");
                AnsiConsole.MarkupLine($"[purple]{this.InputModel.FullName}[/]");
                AnsiConsole.WriteLine("");
                return -1;
            }

            try
            {
                await AnsiConsole.Status()
                    .AutoRefresh(true)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .Start($"Preparing Warp Engines for {this.ReportGenerator.QueryReportType()} reporting...", ctx =>
                    {
                        Thread.Sleep(1500);

                        ctx.Status($"Generating Ecore Model report at Warp 11, Captain..., SLOW DOWN!");

                        Thread.Sleep(1500);

                        this.ReportGenerator.GenerateReport(this.InputModel, this.OutputReport);

                        AnsiConsole.MarkupLine(
                            $"[grey]LOG:[/] Ecore {this.ReportGenerator.QueryReportType()} report generated at [bold]{this.OutputReport.FullName}[/]");
                        Thread.Sleep(1500);

                        this.ExecuteAutoOpen(ctx);

                        ctx.Status("[green]Dropping to impulse speed[/]");
                        Thread.Sleep(1500);

                        return Task.FromResult(0);

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
        /// validates the options
        /// </summary>
        /// <returns>
        /// 0 when successful, -1 when not
        /// </returns>
        protected bool InputValidation()
        {
            if (!this.NoLogo)
            {
                AnsiConsole.Markup($"[blue]{ResourceLoader.QueryLogo()}[/]");
            }

            if (!this.InputModel.Exists)
            {
                AnsiConsole.WriteLine("");
                AnsiConsole.MarkupLine($"[red]The specified input ecore model does not exist[/]");
                AnsiConsole.MarkupLine($"[purple]{this.InputModel.FullName}[/]");
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
        protected void ExecuteAutoOpen(StatusContext ctx)
        {
            if (this.AutoOpenReport)
            {
                ctx.Status($"Opening generated report");
                Thread.Sleep(1500);

                try
                {
                    Process.Start(new ProcessStartInfo(this.OutputReport.FullName)
                        { UseShellExecute = true });
                    ctx.Status($"Generated report opened");
                }
                catch
                {
                    ctx.Status($"Opening of generated report failed, please open manually");
                    Thread.Sleep(1500);
                }
            }
        }
    }
}