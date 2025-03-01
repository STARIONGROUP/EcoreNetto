// -------------------------------------------------------------------------------------------------
// <copyright file="VersionCheckerMiddleware.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Tools.Middlewares
{
    using System;
    using System.CommandLine.Builder;
    using System.CommandLine.Invocation;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;

    using ECoreNetto.Tools.Services;

    using Spectre.Console;

    /// <summary>
    /// A middleware that checks whether a newer version is available
    /// </summary>
    internal static class VersionCheckerMiddleware
    {
        /// <summary>
        /// Configures the application to show check for a new version
        /// </summary>
        /// <param name="builder">
        /// A command line builder.
        /// </param>
        /// <returns>
        /// The same instance of <see cref="CommandLineBuilder"/>.
        /// </returns>
        public static CommandLineBuilder UseVersionChecker(this CommandLineBuilder builder)
        {
            return builder.AddMiddleware(async (context, next) =>
            {
                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(2);
                var versionChecker = new VersionChecker(httpClient);

                try
                {
                    var payload = await versionChecker.QueryLatestRelease();

                    if (payload != null)
                    {
                        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                        var publishedVersion = new Version(payload.TagName);

                        if (currentVersion < publishedVersion)
                        {
                            AnsiConsole.WriteLine("");
                            AnsiConsole.MarkupLine($"[Green] a newer version is available at {payload.HtmlUrl} [/]");
                            AnsiConsole.MarkupLine($"[Green] {payload.Body.EscapeMarkup()} [/]");
                            AnsiConsole.WriteLine("");
                        }
                        else
                        {
                            AnsiConsole.WriteLine("");
                            AnsiConsole.MarkupLine($"[Green] you are using the most recent version. [/]");
                            AnsiConsole.WriteLine("");
                        }
                    }

                    await next(context);
                }
                catch (TaskCanceledException)
                {
                    AnsiConsole.WriteLine("");
                    AnsiConsole.MarkupLine($"[Red] Checking version at GitHub API timed out. [/]");
                    AnsiConsole.WriteLine("");
                }
            }, MiddlewareOrder.ExceptionHandler);
        }
    }
}
