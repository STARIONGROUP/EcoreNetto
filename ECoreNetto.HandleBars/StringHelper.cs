// ------------------------------------------------------------------------------------------------
// <copyright file="StringHelper.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars
{
    using ECoreNetto.Extensions;

    using HandlebarsDotNet;

    /// <summary>
    /// A block helper that prints the name of the type of a <see cref="EStructuralFeature"/>
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Registers the <see cref="StructuralFeatureHelper"/>
        /// </summary>
        /// <param name="handlebars">
        /// The <see cref="IHandlebars"/> context with which the helper needs to be registered
        /// </param>
        public static void RegisterStringHelper(this IHandlebars handlebars)
        {
            handlebars.RegisterHelper("String.CapitalizeFirstLetter", (writer, _, parameters) =>
            {
                if (parameters.Length != 1)
                {
                    throw new HandlebarsException("{{#String.CapitalizeFirstLetter}} helper must have exactly one argument");
                }

                if (!(parameters[0] is string value))
                {
                    throw new HandlebarsException("{{#String.CapitalizeFirstLetter}} helper requires a single string argument");
                }

                writer.WriteSafeString(value.CapitalizeFirstLetter());
            });

            handlebars.RegisterHelper("String.LowerCaseFirstLetter", (writer, _, parameters) =>
            {
                if (parameters.Length != 1)
                {
                    throw new HandlebarsException("{{#String.LowerCaseFirstLetter}} helper must have exactly one argument");
                }

                if (!(parameters[0] is string value))
                {
                    throw new HandlebarsException("{{#String.LowerCaseFirstLetter}} helper requires a single string argument");
                }

                writer.WriteSafeString(value.LowerCaseFirstLetter());
            });
        }
    }
}
