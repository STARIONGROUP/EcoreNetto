// -------------------------------------------------------------------------------------------------
// <copyright file="ContainmentUpdater.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2024 RHEA System S.A.
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

namespace ECoreNetto.HandleBars
{
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
            handlebars.RegisterHelper("StructuralFeature.QueryIsEnumerable", (context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryIsEnumerable}} helper must have exactly one argument");
                }

                var eStructuralFeature = arguments.Single() as EStructuralFeature;

                return eStructuralFeature.QueryIsEnumerable();
            });

            handlebars.RegisterHelper("StructuralFeature.IsEnumerable", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.IsEnumerable}} helper must have exactly one argument");
                }

                var eStructuralFeature = arguments.Single() as EStructuralFeature;

                var isEnumerable = eStructuralFeature.QueryIsEnumerable();

                if (isEnumerable)
                {
                    options.Template(output, context);
                }
            });

            handlebars.RegisterHelper("StructuralFeature.QueryIsAttribute", (context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryIsAttribute}} helper must have exactly one argument");
                }

                var eStructuralFeature = arguments.Single() as EStructuralFeature;

                return eStructuralFeature.QueryIsAttribute();
            });

            handlebars.RegisterHelper("StructuralFeature.IsAttribute", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.IsAttribute}} helper must have exactly one argument");
                }

                var eStructuralFeature = arguments.Single() as EStructuralFeature;

                var isAttribute = eStructuralFeature.QueryIsAttribute();

                if (isAttribute)
                {
                    options.Template(output, context);
                }
            });

            handlebars.RegisterHelper("StructuralFeature.QueryIsReference", (context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryIsReference}} helper must have exactly one argument");
                }

                var eStructuralFeature = arguments.Single() as EStructuralFeature;

                return eStructuralFeature.QueryIsReference();
            });

            handlebars.RegisterHelper("StructuralFeature.IsReference", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.IsReference}} helper must have exactly one argument");
                }

                var eStructuralFeature = arguments.Single() as EStructuralFeature;

                var isReference = eStructuralFeature.QueryIsReference();

                if (isReference)
                {
                    options.Template(output, context);
                }
            });

            handlebars.RegisterHelper("StructuralFeature.QueryStructuralFeatureNameEqualsEnclosingType", (context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryStructuralFeatureNameEqualsEnclosingType}} helper must have exactly two arguments");
                }

                var eStructuralFeature = arguments[0] as EStructuralFeature;
                var eClass = arguments[1] as EClass;

                return eStructuralFeature.QueryStructuralFeatureNameEqualsEnclosingType(eClass);
            });

            handlebars.RegisterHelper("StructuralFeature.NameEqualsEnclosingType", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.NameEqualsEnclosingType}} helper must have exactly two arguments");
                }

                var eStructuralFeature = arguments.Single() as EStructuralFeature;
                var eClass = arguments.Last() as EClass;

                var nameEqualsEnclosingType = eStructuralFeature.QueryStructuralFeatureNameEqualsEnclosingType(eClass);

                if (nameEqualsEnclosingType)
                {
                    options.Template(output, context);
                }
            });

            handlebars.RegisterHelper("StructuralFeature.QueryIsEnum", (context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryIsEnum}} helper must have exactly one argument");
                }

                var eStructuralFeature = arguments.Single() as EStructuralFeature;

                return eStructuralFeature.QueryIsEnum();
            });

            handlebars.RegisterHelper("StructuralFeature.IsEnum", (output, options, context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.IsEnum}} helper must have exactly one argument");
                }

                var eStructuralFeature = arguments.Single() as EStructuralFeature;

                var isEnum = eStructuralFeature.QueryIsEnum();

                if (isEnum)
                {
                    options.Template(output, context);
                }
            });

            handlebars.RegisterHelper("StructuralFeature.QueryHasDefaultValue", (context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryHasDefaultValue}} helper must have exactly one argument");
                }

                var eStructuralFeature = arguments.Single() as EStructuralFeature;

                return eStructuralFeature.QueryHasDefaultValue();
            });

            handlebars.RegisterHelper("StructuralFeature.QueryIsContainment", (context, arguments) =>
            {
                if (arguments.Length != 1)
                {
                    throw new HandlebarsException("{{#StructuralFeature.QueryIsContainment}} helper must have exactly one argument");
                }

                var eStructuralFeature = arguments.Single() as EStructuralFeature;

                return eStructuralFeature.QueryIsContainment();
            });
        }
    }
}
