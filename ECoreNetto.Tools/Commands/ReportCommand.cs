// -------------------------------------------------------------------------------------------------
// <copyright file="ReportCommand.cs" company="Starion Group S.A">
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
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using ECoreNetto.Extensions;
    using ECoreNetto.Processor.Resources;
    using Spectre.Console;

    /// <summary>
    /// The <see cref="RootCommand"/> that generates ecore reports
    /// </summary>
    public class ReportCommand : RootCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportCommand"/>
        /// </summary>
        public ReportCommand() : base("Ecore Tools")
        {
            var noLogoOption = new Option<bool>(
                name: "--no-logo",
                description: "Suppress the logo",
                getDefaultValue: () => false);
            this.AddOption(noLogoOption);

            var inputModelFileOption = new Option<FileInfo>(
                name: "--input-model",
                description: "The path to the ecore file",
                getDefaultValue: () => new FileInfo("model.ecore"));
            inputModelFileOption.AddAlias("-i");
            inputModelFileOption.IsRequired = true;
            this.AddOption(inputModelFileOption);

            var reportFileOption = new Option<FileInfo>(
                name: "--output-report",
                description: "The path to the tabular report file. Supported extensions are '.xlsx', '.xlsm', '.xltx' and '.xltm'",
                getDefaultValue: () => new FileInfo("tabular-report.xlsx"));
            reportFileOption.AddAlias("-o");
            reportFileOption.IsRequired = true;
            this.AddOption(reportFileOption);
        }

        /// <summary>
        /// The Command Handler of the <see cref="ReportCommand"/>
        /// </summary>
        public new class Handler : ICommandHandler
        {
            /// <summary>
            /// The (injected) <see cref="IReportGenerator"/> that is used to generate the
            /// excel report
            /// </summary>
            private readonly IReportGenerator reportGenerator;

            /// <summary>
            /// Initializes a nwe instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="reportGenerator">
            /// The (injected) <see cref="IReportGenerator"/> that is used to generate the
            /// excel report
            /// </param>
            public Handler(IReportGenerator reportGenerator)
            {
                this.reportGenerator = reportGenerator ?? throw new ArgumentNullException(nameof(reportGenerator));
            }

            /// <summary>
            /// Gets or sets the value indicating whether the logo should be shown or not
            /// </summary>
            public bool NoLogo { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="FileInfo"/> where the ecore model is located that is to be read
            /// </summary>
            public FileInfo InputModel { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="FileInfo"/> where the SRF report is to be generated
            /// </summary>
            public FileInfo OutputReport { get; set; }

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
                    return -1;
                }

                var isValidExtension = this.reportGenerator.IsValidExcelReportExtension(this.OutputReport);
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
                        .Start("Preparing Warp Engines for reporting...", ctx =>
                        {
                            Thread.Sleep(1500);

                            ctx.Status($"Generating Ecore Model report at Warp 11, Captain..., SLOW DOWN!");
                            
                            Thread.Sleep(1500);

                            this.reportGenerator.GenerateTable(this.InputModel, this.OutputReport);

                            AnsiConsole.MarkupLine($"[grey]LOG:[/] Ecore report generated at [bold]{this.OutputReport.FullName}[/]");
                            Thread.Sleep(1500);
                            ctx.Status("[green]Dropping to impulse speed[/]");
                            Thread.Sleep(1500);

                            return Task.FromResult(0);

                        });
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
        }
    }
}