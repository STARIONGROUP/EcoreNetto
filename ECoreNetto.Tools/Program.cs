// -------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Tools
{
    using System.CommandLine;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Autofac.Extensions.DependencyInjection;

    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    
    using ECoreNetto.Tools.Commands;
    using ECoreNetto.Reporting.Generators;
    
    using ECoreNetto.Tools.Services;

    /// <summary>
    /// Main entry point for the command line application
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        /// <summary>
        /// Runtime-adjustable Serilog minimum level.
        /// </summary>
        private static readonly LoggingLevelSwitch LoggingLevelSwitch = new();

        /// <summary>
        /// Main entry point for the command line application
        /// </summary>
        /// <param name="args">
        /// command line arguments
        /// </param>
        public static async Task<int> Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            var rootCommand = CreateCommandChain(host);

            var parseResult = rootCommand.Parse(args);

            var result = await parseResult.InvokeAsync();

            return result;
        }

        /// <summary>
        /// Creates the <see cref="IHostBuilder"/>
        /// </summary>
        /// <param name="args">
        /// the command line arguments
        /// </param>
        /// <returns>
        /// a configured instance of <see cref="IHostBuilder"/>
        /// </returns>
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    var template = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] ({SourceContext}) {Message:lj}{NewLine}{Exception}";
                    var loggerConfig = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .MinimumLevel.ControlledBy(LoggingLevelSwitch)
                        .WriteTo.File("uml4net.logs",
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: template);

                    var serilogLogger = loggerConfig.CreateLogger();
                    loggingBuilder.AddSerilog(serilogLogger, dispose: true);

                    loggingBuilder.SetMinimumLevel(LogLevel.Information);
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureServices(services =>
                {
                    services.AddHttpClient();

                    services.AddSingleton<IVersionChecker, VersionChecker>();
                    services.AddSingleton<IXlReportGenerator, XlReportGenerator>();
                    services.AddSingleton<IModelInspector, ModelInspector>();
                    services.AddSingleton<IHtmlReportGenerator, HtmlReportGenerator>();
                    services.AddSingleton<IMarkdownReportGenerator, MarkdownReportGenerator>();
                });

            return host;
        }

        /// <summary>
        /// Creates the root and sub commands
        /// </summary>
        /// <returns>
        /// returns an instance of <see cref="RootCommand"/>
        /// </returns>
        private static RootCommand CreateCommandChain(IHost host)
        {
            var root = new RootCommand("ECoreNetto Tools");

            var reportCommand = new XlReportCommand();
            reportCommand.SetAction((parseResult, cancellationToken) =>
            {
                ApplyLogLevel(parseResult);

                using var scope = host.Services.CreateScope();
                var generator = scope.ServiceProvider.GetService<IXlReportGenerator>();
                var versionChecker = scope.ServiceProvider.GetService<IVersionChecker>();
                var handler = new XlReportCommand.Handler(generator, versionChecker);
                return handler.InvokeAsync(parseResult, cancellationToken);
            });
            root.Add(reportCommand);
            

            var modelInspectionCommand = new ModelInspectionCommand();
            modelInspectionCommand.SetAction((parseResult, cancellationToken) =>
            {
                ApplyLogLevel(parseResult);

                using var scope = host.Services.CreateScope();
                var generator = scope.ServiceProvider.GetService<IModelInspector>();
                var versionChecker = scope.ServiceProvider.GetService<IVersionChecker>();
                var handler = new ModelInspectionCommand.Handler(generator, versionChecker);
                return handler.InvokeAsync(parseResult, cancellationToken);
            });
            root.Add(modelInspectionCommand);

            var htmlReportCommand = new HtmlReportCommand();
            htmlReportCommand.SetAction((parseResult, cancellationToken) =>
            {
                ApplyLogLevel(parseResult);

                using var scope = host.Services.CreateScope();
                var generator = scope.ServiceProvider.GetService<IHtmlReportGenerator>();
                var versionChecker = scope.ServiceProvider.GetService<IVersionChecker>();
                var handler = new HtmlReportCommand.Handler(generator, versionChecker);
                return handler.InvokeAsync(parseResult, cancellationToken);
            });
            root.Add(htmlReportCommand);

            var markdownReportCommand = new MarkdownReportCommand();
            markdownReportCommand.SetAction((parseResult, cancellationToken) =>
            {
                ApplyLogLevel(parseResult);

                using var scope = host.Services.CreateScope();
                var generator = scope.ServiceProvider.GetService<IMarkdownReportGenerator>();
                var versionChecker = scope.ServiceProvider.GetService<IVersionChecker>();
                var handler = new MarkdownReportCommand.Handler(generator, versionChecker);
                return handler.InvokeAsync(parseResult, cancellationToken);
            });
            root.Add(markdownReportCommand);

            return root;
        }

        /// <summary>
        /// Reads the log level from the parse result and updates the Serilog <see cref="LoggingLevelSwitch"/>>.
        /// </summary>
        private static void ApplyLogLevel(ParseResult parseResult)
        {
            LoggingLevelSwitch.MinimumLevel = parseResult.GetValue<LogEventLevel>("--log-level");
        }
    }
}
