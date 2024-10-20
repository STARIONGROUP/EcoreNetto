// -------------------------------------------------------------------------------------------------
// <copyright file="VersionChecker.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Tools.Services
{
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="VersionChecker"/> is to check whether a newer version is available
    /// </summary>
    public class VersionChecker
    {
        /// <summary>
        /// The (injected) <see cref="HttpClient"/> used to check the latest version that is available
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<VersionChecker> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionChecker"/>
        /// </summary>
        /// <param name="httpClient">
        /// The (injected) <see cref="HttpClient"/> used to check the latest version that is available
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public VersionChecker(HttpClient httpClient, ILoggerFactory loggerFactory = null)
        {
            this.httpClient = httpClient;
            this.logger = loggerFactory == null ? NullLogger<VersionChecker>.Instance : loggerFactory.CreateLogger<VersionChecker>();
        }

        /// <summary>
        /// Queries the latest version from the GitHub API
        /// </summary>
        /// <returns>
        /// an instance of <see cref="GitHubRelease"/> or null if not found or a connection
        /// error occured
        /// </returns>
        public async Task<GitHubRelease> QueryLatestRelease()
        {
            var requestUrl = "https://api.github.com/repos/STARIONGROUP/EcoreNetto/releases/latest";

            try
            {
                this.httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ECoreNetto.Tools");

                var response = await this.httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var release = JsonSerializer.Deserialize<GitHubRelease>(jsonResponse);

                    return release;
                }
            }
            catch (TaskCanceledException)
            {
                this.logger.LogWarning("Contacting the GitGub API at {url} timed out", requestUrl);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "");
            }

            return null;
        }
    }
}