// -------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Tools
{
    using System.CommandLine.Builder;
    using System.CommandLine.Help;
    using System.CommandLine.Hosting;
    using System.CommandLine.Parsing;
    using System.Linq;

    using ECoreNetto.Processor.Resources;
    using ECoreNetto.Tools.Commands;
    using Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;
    using Microsoft.Extensions.Hosting;

    using Spectre.Console;

    /// <summary>
    /// Main entry point for the command line application
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point for the command line application
        /// </summary>
        /// <param name="args">
        /// command line arguments
        /// </param>
        public static int Main(string[] args)
        {
            var commandLineBuilder = BuildCommandLine()
                .UseHost(_ => Host.CreateDefaultBuilder(args)
                        .ConfigureLogging(loggingBuilder =>
                            loggingBuilder.AddFilter<ConsoleLoggerProvider>(level =>
                                level == LogLevel.None))
                    , builder => builder
                        .ConfigureServices((hostContext, services) =>
                        {
                            services.AddSingleton<IReportGenerator, ReportGenerator>();
                        })
                        .UseCommandHandler<ReportCommand, ReportCommand.Handler>())
                .UseDefaults()

                .Build();

            return commandLineBuilder.Invoke(args);
        }

        /// <summary>
        /// builds the root command
        /// </summary>
        /// <returns>
        /// The <see cref="CommandLineBuilder"/> with the root command set
        /// </returns>
        private static CommandLineBuilder BuildCommandLine()
        {
            var root = new ReportCommand();

            return new CommandLineBuilder(root)
                .UseHelp(ctx =>
                {
                    ctx.HelpBuilder.CustomizeLayout(_ =>
                        HelpBuilder.Default
                            .GetLayout()
                            .Skip(1) // Skip the default command description section.
                            .Prepend(
                                _ =>
                                {
                                    AnsiConsole.Markup($"[blue]{ResourceLoader.QueryLogo()}[/]");
                                }
                            ));
                });
        }
    }
}