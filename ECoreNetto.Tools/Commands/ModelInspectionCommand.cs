// -------------------------------------------------------------------------------------------------
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
            /// The (injected) <see cref="IXlReportGenerator"/> that is used to generate the
            /// excel report
            /// </summary>
            private readonly IModelInspector modelInspector;

            /// <summary>
            /// Initializes a nwe instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="modelInspector">
            /// The (injected) <see cref="IModelInspector"/> that is used to generate the
            /// inspection report
            /// </param>
            public Handler(IModelInspector modelInspector)
            {
                this.modelInspector = modelInspector ?? throw new ArgumentNullException(nameof(modelInspector));
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

                var isValidExtension = this.modelInspector.IsValidReportExtension(this.OutputReport);
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

                            this.modelInspector.GenerateReport(this.InputModel, this.OutputReport);

                            AnsiConsole.MarkupLine($"[grey]LOG:[/] Ecore inspection report generated at [bold]{this.OutputReport.FullName}[/]");
                            Thread.Sleep(1500);

                            this.ExecuteAutOpen(ctx);

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