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
    using System.Buffers;
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
        /// The default GitHub API URL used to query the latest release
        /// </summary>
        public const string DefaultReleasesUrl = "https://api.github.com/repos/STARIONGROUP/EcoreNetto/releases/latest";

        /// <summary>
        /// The cached <see cref="SearchValues{T}"/> of the SemVer pre-release / build-metadata separators
        /// used when trimming a tag name before version parsing.
        /// </summary>
        private static readonly SearchValues<char> SemVerSuffixSeparators = SearchValues.Create("-+");

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionChecker"/>
        /// </summary>
        /// <param name="httpClientFactory">
        /// The (injected) <see cref="IHttpClientFactory"/> used to create an <see cref="HttpClient"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        /// <param name="releasesUrl">
        /// The GitHub API URL used to query the latest release. When null, <see cref="DefaultReleasesUrl"/> is used.
        /// </param>
        /// <param name="timeout">
        /// The timeout applied to the HTTP request. When null, a default of 2 seconds is used.
        /// </param>
        public VersionChecker(IHttpClientFactory httpClientFactory, ILoggerFactory? loggerFactory = null, string? releasesUrl = null, TimeSpan? timeout = null)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = loggerFactory == null ? NullLogger<VersionChecker>.Instance : loggerFactory.CreateLogger<VersionChecker>();
            this.ReleasesUrl = releasesUrl ?? DefaultReleasesUrl;
            this.Timeout = timeout ?? TimeSpan.FromSeconds(2);
        }

        /// <summary>
        /// Gets the GitHub API URL used to query the latest release
        /// </summary>
        public string ReleasesUrl { get; }

        /// <summary>
        /// Gets the timeout applied to the HTTP request
        /// </summary>
        public TimeSpan Timeout { get; }

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

                    if (!TryParseVersion(payload.TagName, out var publishedVersion))
                    {
                        this.logger.LogWarning("Unable to parse the published version '{TagName}' returned by the GitHub API", payload.TagName);
                        return;
                    }

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
        public async Task<GitHubRelease?> QueryLatestReleaseAsync(CancellationToken cancellationToken)
        {
            var httpClient = this.httpClientFactory.CreateClient();
            httpClient.Timeout = this.Timeout;

            try
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ECoreNetto.Tools");

                var response = await httpClient.GetAsync(this.ReleasesUrl, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                    var release = JsonSerializer.Deserialize<GitHubRelease>(jsonResponse);

                    return release;
                }
            }
            catch (TaskCanceledException taskCanceledException)
            {
                this.logger.LogWarning(taskCanceledException, "Contacting the GitHub API at {Url} timed out", this.ReleasesUrl);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "An error occurred while querying the latest release from the GitHub API at {Url}", this.ReleasesUrl);
            }

            return null;
        }

        /// <summary>
        /// Attempts to parse the version from a GitHub release tag name, tolerating a leading
        /// 'v'/'V' prefix and any SemVer pre-release or build-metadata suffix (e.g. '-beta', '+build').
        /// </summary>
        /// <param name="tagName">
        /// The raw tag name returned by the GitHub API
        /// </param>
        /// <param name="version">
        /// The parsed <see cref="Version"/> when parsing succeeds; otherwise null.
        /// </param>
        /// <returns>
        /// true when the tag name could be parsed into a <see cref="Version"/>, false otherwise.
        /// </returns>
        private static bool TryParseVersion(string? tagName, out Version? version)
        {
            version = null;

            if (string.IsNullOrWhiteSpace(tagName))
            {
                return false;
            }

            var candidate = tagName!.Trim().TrimStart('v', 'V');

            var suffixIndex = candidate.AsSpan().IndexOfAny(SemVerSuffixSeparators);
            if (suffixIndex >= 0)
            {
                candidate = candidate.Substring(0, suffixIndex);
            }

            return Version.TryParse(candidate, out version);
        }
    }
}
