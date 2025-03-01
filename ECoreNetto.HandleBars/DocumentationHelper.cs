﻿// -------------------------------------------------------------------------------------------------
// <copyright file="DocumentationHelper.cs" company="Starion Group S.A">
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

namespace ECoreNetto.HandleBars
{
    using System;

    using ECoreNetto;
    using ECoreNetto.Extensions;

    using HandlebarsDotNet;

    /// <summary>
    /// a block helper 
    /// </summary>
    public static class DocumentationHelper
    {
        /// <summary>
        /// Registers the <see cref="DocumentationHelper"/>
        /// </summary>
        /// <param name="handlebars">
        /// The <see cref="IHandlebars"/> context with which the helper needs to be registered
        /// </param>
        public static void RegisteredDocumentationHelper(this IHandlebars handlebars)
        {
            handlebars.RegisterHelper("RawDocumentation", (writer, context, _) =>
            {
                if (!(context.Value is EModelElement eModelElement))
                {
                    throw new ArgumentException("supposed to be EModelElement");
                }

                var rawDocumentation = eModelElement.QueryRawDocumentation();

                if (string.IsNullOrEmpty(rawDocumentation))
                {
                    rawDocumentation = "No Documentation Provided";
                }

                writer.WriteSafeString(rawDocumentation);
            });
        }
    }
}