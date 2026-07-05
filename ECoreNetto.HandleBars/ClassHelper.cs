// ------------------------------------------------------------------------------------------------
// <copyright file="ClassHelper.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars
{
    using System.Collections.Generic;
    using System.Linq;

    using ECoreNetto;
    using ECoreNetto.Extensions;

    using HandlebarsDotNet;

    /// <summary>
    /// Handlebars helpers for the <see cref="EClass"/> class: specialization/container/constraint queries
    /// and per-class diagram rendering.
    /// </summary>
    public static class ClassHelper
    {
        /// <summary>
        /// Registers the <see cref="ClassHelper"/>
        /// </summary>
        /// <param name="handlebars">
        /// The <see cref="IHandlebars"/> context with which the helper needs to be registered
        /// </param>
        public static void RegisterClassHelper(this IHandlebars handlebars)
        {
            handlebars.RegisterHelper("Class.QuerySpecializations", (_, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{Class.QuerySpecializations}} helper must have exactly two arguments");
                }

                var @class = (EClass)arguments[0]!;
                var allClasses = ((IEnumerable<EClass>)arguments[1]!).ToList();

                return @class.QuerySpecializations(allClasses).OrderBy(x => x.Name);
            });

            handlebars.RegisterHelper("Class.QueryContainers", (_, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{Class.QueryContainers}} helper must have exactly two arguments");
                }

                var @class = (EClass)arguments[0]!;
                var allClasses = ((IEnumerable<EClass>)arguments[1]!).ToList();

                return @class.QueryContainers(allClasses).OrderBy(x => x.Name);
            });

            handlebars.RegisterHelper("Class.QueryConstraints", (_, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{Class.QueryConstraints}} helper must have exactly one argument");
                }

                var @class = (EClass)arguments[0]!;

                return @class.QueryConstraints();
            });

            handlebars.RegisterHelper("Class.RenderInheritanceDiagram", (output, options, _, arguments) =>
            {
                if (!TryGetDiagram(arguments, out var svg))
                {
                    return;
                }

                options.Template(output, svg);
            });

            handlebars.RegisterHelper("Class.RenderAssociationDiagram", (output, options, _, arguments) =>
            {
                if (!TryGetDiagram(arguments, out var svg))
                {
                    return;
                }

                options.Template(output, svg);
            });
        }

        /// <summary>
        /// Attempts to resolve the pre-rendered diagram SVG for the class passed as the first argument, from
        /// the anchor-keyed dictionary passed as the second argument.
        /// </summary>
        /// <param name="arguments">
        /// The block-helper arguments: an <see cref="EClass"/> and an anchor-keyed diagram dictionary.
        /// </param>
        /// <param name="svg">
        /// The resolved SVG string when present.
        /// </param>
        /// <returns>
        /// True when a diagram exists for the class; otherwise false.
        /// </returns>
        private static bool TryGetDiagram(Arguments arguments, out string? svg)
        {
            svg = null;

            if (arguments.Length < 2)
            {
                return false;
            }

            if (!(arguments[0] is EClass @class) || !(arguments[1] is IDictionary<string, string> diagrams))
            {
                return false;
            }

            return diagrams.TryGetValue(@class.QueryAnchorId(), out svg);
        }
    }
}
