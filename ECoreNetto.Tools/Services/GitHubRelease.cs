// ------------------------------------------------------------------------------------------------
// <copyright file="GitHubRelease.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
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
        public string HtmlUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of the tag
        /// </summary>
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the description of the release
        /// </summary>
        [JsonPropertyName("body")]
        public string Body { get; set; } = null!;
    }
}
