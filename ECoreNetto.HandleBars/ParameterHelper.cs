// ------------------------------------------------------------------------------------------------
// <copyright file="ParameterHelper.cs" company="Starion Group S.A.">
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
    /// Handlebars helpers that render <see cref="EParameter"/>s and <see cref="ETypedElement"/> return
    /// types (type link, name and multiplicity).
    /// </summary>
    public static class ParameterHelper
    {
        /// <summary>
        /// Registers the <see cref="ParameterHelper"/>
        /// </summary>
        /// <param name="handlebars">
        /// The <see cref="IHandlebars"/> context with which the helper needs to be registered
        /// </param>
        public static void RegisterParameterHelper(this IHandlebars handlebars)
        {
            handlebars.RegisterHelper("Parameter.WriteTypeAndName", (writer, _, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{Parameter.WriteTypeAndName}} helper must have exactly one argument");
                }

                var parameter = (EParameter)arguments[0]!;

                if (parameter.EType == null)
                {
                    writer.WriteSafeString("void");
                }
                else
                {
                    writer.WriteSafeString($"<a href=\"#{parameter.EType.QueryAnchorId()}\">{parameter.EType.Name}</a>");
                }

                writer.WriteSafeString($" {parameter.Name}");
                writer.WriteSafeString($" {FormatMultiplicity(parameter.LowerBound, parameter.UpperBound)}");
            });

            handlebars.RegisterHelper("TypedElement.WriteReturnType", (writer, _, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{TypedElement.WriteReturnType}} helper must have exactly one argument");
                }

                var typedElement = (ETypedElement)arguments[0]!;

                if (typedElement.EType == null)
                {
                    writer.WriteSafeString("void");
                    return;
                }

                writer.WriteSafeString($"<a href=\"#{typedElement.EType.QueryAnchorId()}\">{typedElement.EType.Name}</a>");
                writer.WriteSafeString($" {FormatMultiplicity(typedElement.LowerBound, typedElement.UpperBound)}");
            });
        }

        /// <summary>
        /// Formats the multiplicity of a typed element as <c>[lower..upper]</c>, using <c>*</c> for the
        /// unbounded upper bound (-1).
        /// </summary>
        /// <param name="lowerBound">
        /// The lower bound.
        /// </param>
        /// <param name="upperBound">
        /// The upper bound (-1 means unbounded).
        /// </param>
        /// <returns>
        /// The formatted multiplicity string.
        /// </returns>
        private static string FormatMultiplicity(int lowerBound, int upperBound)
        {
            var upper = upperBound == -1 ? "*" : upperBound.ToString();

            return $"[{lowerBound}..{upper}]";
        }
    }
}
