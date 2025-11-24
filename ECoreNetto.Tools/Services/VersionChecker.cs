// -------------------------------------------------------------------------------------------------
// <copyright file="VersionChecker.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Tools.Services
{
    using System;
    using System.Net.Http;
    using System.Reflection;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Spectre.Console;

    /// <summary>
    /// The purpose of the <see cref="VersionChecker"/> is to check whether a newer version is available
    /// </summary>
    public class VersionChecker : IVersionChecker
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<VersionChecker> logger;

        /// <summary>
        /// The (injected) <see cref="IHttpClientFactory"/> used to create an <see cref="HttpClient"/>
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionChecker"/>
        /// </summary>
        /// <param name="httpClientFactory">
        /// The (injected) <see cref="IHttpClientFactory"/> used to create an <see cref="HttpClient"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public VersionChecker(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory = null)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = loggerFactory == null ? NullLogger<VersionChecker>.Instance : loggerFactory.CreateLogger<VersionChecker>();
        }

        /// <summary>
        /// Checks for the lastest release
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> used to cancel the operation
        /// </param>
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            try
            {
                var payload = await QueryLatestReleaseAsync(cancellationToken);

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
                        AnsiConsole.MarkupLine($"[Green] you are using the most recent version: {currentVersion}. [/]");
                        AnsiConsole.WriteLine("");
                    }
                }
            }
            catch (TaskCanceledException)
            {
                AnsiConsole.WriteLine("");
                AnsiConsole.MarkupLine($"[Red] Checking version at GitHub API timed out. [/]");
                AnsiConsole.WriteLine("");
            }
        }

        /// <summary>
        /// Queries the latest version from the GitHub API
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> used to cancel the operation
        /// </param>
        /// <returns>
        /// an instance of <see cref="GitHubRelease"/> or null if not found or a connection
        /// error occured
        /// </returns>
        public async Task<GitHubRelease> QueryLatestReleaseAsync(CancellationToken cancellationToken)
        {
            var httpClient = this.httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(2);

            const string requestUrl = "https://api.github.com/repos/STARIONGROUP/EcoreNetto/releases/latest";

            try
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ECoreNetto.Tools");

                var response = await httpClient.GetAsync(requestUrl, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                    var release = JsonSerializer.Deserialize<GitHubRelease>(jsonResponse);

                    return release;
                }
            }
            catch (TaskCanceledException taskCanceledException)
            {
                this.logger.LogWarning(taskCanceledException, "Contacting the GitGub API at {Url} timed out", requestUrl);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "");
            }

            return null;
        }
    }
}
