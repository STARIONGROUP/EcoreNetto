// -------------------------------------------------------------------------------------------------
// <copyright file="ModelElementExtensions.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2023 RHEA System S.A.
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
    using System.Collections.Generic;
    using System.Linq;

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
            var annotation = eModelElement.EAnnotations.FirstOrDefault();
            if (annotation == null)
            {
                return Enumerable.Empty<string>();
            }

            if (annotation.Details.TryGetValue("documentation", out var documentation))
            {
                var splitLines = documentation.SplitToLines(100);

                return splitLines;
            }

            return Enumerable.Empty<string>();
        }
    }
}
