// -------------------------------------------------------------------------------------------------
// <copyright file="ModelElementExtensions.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using HtmlAgilityPack;

    /// <summary>
    /// Extension methods for <see cref="ModelElementExtensions"/> class
    /// </summary>
    public static class ModelElementExtensions
    {
        /// <summary>
        /// Queries the documentation from the <see cref="EModelElement"/> and
        /// returns it as a string
        /// </summary>
        /// <param name="eModelElement"></param>
        /// <returns></returns>
        public static IEnumerable<string> QueryDocumentation(this EModelElement eModelElement)
        {
            var annotation = eModelElement.EAnnotations.FirstOrDefault(x => x.Details.ContainsKey("documentation"));
            if (annotation == null)
            {
                return Enumerable.Empty<string>();
            }

            if (annotation.Details.TryGetValue("documentation", out var documentation))
            {
                var unwantedTags = new List<string> { "p", "code", "em", "tt" };

                var result = documentation.RemoveUnwantedHtmlTags(unwantedTags).Replace(Environment.NewLine, "");

                var splitLines = result.SplitToLines(100);

                return splitLines;
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Queries the documentation from the <see cref="EModelElement"/> and
        /// returns it as a string
        /// </summary>
        /// <param name="eModelElement"></param>
        /// <returns></returns>
        public static string QueryRawDocumentation(this EModelElement eModelElement)
        {
            var annotation = eModelElement.EAnnotations.FirstOrDefault(x => x.Details.ContainsKey("documentation"));
            if (annotation == null)
            {
                return string.Empty;
            }

            if (annotation.Details.TryGetValue("documentation", out var documentation))
            {
                var unwantedTags = new List<string> { "p", "code", "em", "tt" };

                var result = documentation.RemoveUnwantedHtmlTags(unwantedTags).Replace(Environment.NewLine, "");

                return result;
            }

            return string.Empty;
        }

        /// <summary>
        /// removes the specified html tags from the <paramref name="html"/>
        /// </summary>
        /// <param name="html">
        /// the string from which the unwanted html tags are to be removed
        /// </param>
        /// <param name="unwantedTags">
        /// list of unwanted html tags
        /// </param>
        /// <returns>
        /// a cleaned up string
        /// </returns>
        public static string RemoveUnwantedHtmlTags(this string html, List<string> unwantedTags)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            var document = new HtmlDocument();
            document.LoadHtml(html);

            HtmlNodeCollection tryGetNodes = document.DocumentNode.SelectNodes("./*|./text()");

            if (tryGetNodes == null || !tryGetNodes.Any())
            {
                return html;
            }

            var nodes = new Queue<HtmlNode>(tryGetNodes);

            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                var parentNode = node.ParentNode;

                var childNodes = node.SelectNodes("./*|./text()");

                if (childNodes != null)
                {
                    foreach (var child in childNodes)
                    {
                        nodes.Enqueue(child);
                    }
                }

                if (unwantedTags.Any(tag => tag == node.Name))
                {
                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                        {
                            parentNode.InsertBefore(child, node);
                        }
                    }

                    parentNode.RemoveChild(node);

                }
            }

            return document.DocumentNode.InnerHtml;
        }
    }
}
