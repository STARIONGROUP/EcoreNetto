// ------------------------------------------------------------------------------------------------
// <copyright file="IEnumerableHelper.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars
{
    using System.Collections;
    using System.Linq;

    using HandlebarsDotNet;

    /// <summary>
    /// A handlebars helper that reports whether an <see cref="IEnumerable"/> is empty, used to guard the
    /// empty-state messages of the report sections.
    /// </summary>
    public static class IEnumerableHelper
    {
        /// <summary>
        /// Registers the <see cref="IEnumerableHelper"/>
        /// </summary>
        /// <param name="handlebars">
        /// The <see cref="IHandlebars"/> context with which the helper needs to be registered
        /// </param>
        public static void RegisterIEnumerableHelper(this IHandlebars handlebars)
        {
            handlebars.RegisterHelper("IEnumerable.IsEmpty", (_, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#IEnumerable.IsEmpty}} helper must have exactly one argument");
                }

                return !(arguments[0] is IEnumerable enumerable) || !enumerable.Cast<object>().Any();
            });
        }
    }
}
