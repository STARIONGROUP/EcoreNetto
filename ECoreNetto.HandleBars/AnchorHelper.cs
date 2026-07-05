// ------------------------------------------------------------------------------------------------
// <copyright file="AnchorHelper.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars
{
    using ECoreNetto;
    using ECoreNetto.Extensions;

    using HandlebarsDotNet;

    /// <summary>
    /// A handlebars helper that writes a stable, unique HTML anchor for an <see cref="EObject"/>.
    /// </summary>
    public static class AnchorHelper
    {
        /// <summary>
        /// Registers the <see cref="AnchorHelper"/>
        /// </summary>
        /// <param name="handlebars">
        /// The <see cref="IHandlebars"/> context with which the helper needs to be registered
        /// </param>
        public static void RegisterAnchorHelper(this IHandlebars handlebars)
        {
            handlebars.RegisterHelper("Anchor", (writer, _, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{Anchor}} helper must have exactly one argument");
                }

                var eObject = (EObject)arguments[0]!;

                writer.WriteSafeString(eObject.QueryAnchorId());
            });
        }
    }
}
