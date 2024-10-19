// -------------------------------------------------------------------------------------------------
// <copyright file="GitHubRelease.cs" company="Starion Group S.A">
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
    using System.Text.Json.Serialization;

    /// <summary>
    /// The <see cref="GitHubRelease"/> class represents a response from the GitHb API
    /// </summary>
    public class GitHubRelease
    {
        /// <summary>
        /// Gets or sets the url of the release page
        /// </summary>
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the tag
        /// </summary>
        [JsonPropertyName("tag_name")]
        public string TagName{ get; set; }

        /// <summary>
        /// Gets or sets the description of the release
        /// </summary>
        [JsonPropertyName("body")]
        public string Body { get; set; }
    }
}