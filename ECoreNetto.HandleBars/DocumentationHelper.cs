// ------------------------------------------------------------------------------------------------
// <copyright file="DocumentationHelper.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
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
