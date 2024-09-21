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
    using System.CommandLine.Invocation;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using ECoreNetto.Processor.Resources;
    using ECoreNetto.Tools.Generators;

    using Spectre.Console;

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