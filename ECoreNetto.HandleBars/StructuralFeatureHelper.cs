// ------------------------------------------------------------------------------------------------
// <copyright file="StructuralFeatureHelper.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars
{
    using System;
    using System.Linq;

    using ECoreNetto;
    using ECoreNetto.Extensions;

    using HandlebarsDotNet;

    /// <summary>
    /// A handlebars block helper for the <see cref="EStructuralFeature"/> class
    /// </summary>
    public static class StructuralFeatureHelper
    {
        /// <summary>
        /// Registers the <see cref="StructuralFeatureHelper"/>
        /// </summary>
        /// <param name="handlebars">
        /// The <see cref="IHandlebars"/> context with which the helper needs to be registered
        /// </param>
        public static void RegisterStructuralFeatureHelper(this IHandlebars handlebars)
        {
            handlebars.RegisterHelper("StructuralFeature.QueryIsEnumerable", (_, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryIsEnumerable}} helper must have exactly one argument");
                }

                var eStructuralFeature = (EStructuralFeature)arguments.Single()!;

                return eStructuralFeature.QueryIsEnumerable();
            });

            handlebars.RegisterHelper("StructuralFeature.IsEnumerable", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.IsEnumerable}} helper must have exactly one argument");
                }

                var eStructuralFeature = (EStructuralFeature)arguments.Single()!;

                var isEnumerable = eStructuralFeature.QueryIsEnumerable();

                if (isEnumerable)
                {
                    options.Template(output, context);
                }
            });

            handlebars.RegisterHelper("StructuralFeature.QueryIsAttribute", (_, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryIsAttribute}} helper must have exactly one argument");
                }

                var eStructuralFeature = (EStructuralFeature)arguments.Single()!;

                return eStructuralFeature.QueryIsAttribute();
            });

            handlebars.RegisterHelper("StructuralFeature.IsAttribute", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.IsAttribute}} helper must have exactly one argument");
                }

                var eStructuralFeature = (EStructuralFeature)arguments.Single()!;

                var isAttribute = eStructuralFeature.QueryIsAttribute();

                if (isAttribute)
                {
                    options.Template(output, context);
                }
            });

            handlebars.RegisterHelper("StructuralFeature.QueryIsReference", (_, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryIsReference}} helper must have exactly one argument");
                }

                var eStructuralFeature = (EStructuralFeature)arguments.Single()!;

                return eStructuralFeature.QueryIsReference();
            });

            handlebars.RegisterHelper("StructuralFeature.IsReference", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.IsReference}} helper must have exactly one argument");
                }

                var eStructuralFeature = (EStructuralFeature)arguments.Single()!;

                var isReference = eStructuralFeature.QueryIsReference();

                if (isReference)
                {
                    options.Template(output, context);
                }
            });

            handlebars.RegisterHelper("StructuralFeature.QueryStructuralFeatureNameEqualsEnclosingType", (_, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryStructuralFeatureNameEqualsEnclosingType}} helper must have exactly two arguments");
                }

                var eStructuralFeature = (EStructuralFeature)arguments[0]!;
                var eClass = (EClass)arguments[1]!;

                return eStructuralFeature.QueryStructuralFeatureNameEqualsEnclosingType(eClass);
            });

            handlebars.RegisterHelper("StructuralFeature.NameEqualsEnclosingType", (output, options, context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{#StructuralFeature.NameEqualsEnclosingType}} helper must have exactly two arguments");
                }

                var eStructuralFeature = (EStructuralFeature)arguments[0]!;
                var eClass = (EClass)arguments[1]!;

                var nameEqualsEnclosingType = eStructuralFeature.QueryStructuralFeatureNameEqualsEnclosingType(eClass);

                if (nameEqualsEnclosingType)
                {
                    options.Template(output, context);
                }
            });

            handlebars.RegisterHelper("StructuralFeature.QueryIsEnum", (_, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryIsEnum}} helper must have exactly one argument");
                }

                var eStructuralFeature = (EStructuralFeature)arguments.Single()!;

                return eStructuralFeature.QueryIsEnum();
            });

            handlebars.RegisterHelper("StructuralFeature.IsEnum", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.IsEnum}} helper must have exactly one argument");
                }

                var eStructuralFeature = (EStructuralFeature)arguments.Single()!;

                var isEnum = eStructuralFeature.QueryIsEnum();

                if (isEnum)
                {
                    options.Template(output, context);
                }
            });

            handlebars.RegisterHelper("StructuralFeature.QueryHasDefaultValue", (_, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryHasDefaultValue}} helper must have exactly one argument");
                }

                var eStructuralFeature = (EStructuralFeature)arguments.Single()!;

                return eStructuralFeature.QueryHasDefaultValue();
            });

            handlebars.RegisterHelper("StructuralFeature.QueryIsContainment", (_, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryIsContainment}} helper must have exactly one argument");
                }

                var eStructuralFeature = (EStructuralFeature)arguments.Single()!;

                return eStructuralFeature.QueryIsContainment();
            });

            handlebars.RegisterHelper("StructuralFeature.QueryTypeName", (writer, context, _) =>
            {
                if (!(context.Value is EStructuralFeature eStructuralFeature))
                {
                    throw new ArgumentException("supposed to be EStructuralFeature");
                }

                var typeName = eStructuralFeature.QueryTypeName();

                writer.WriteSafeString($"{typeName}");
            });
        }
    }
}
