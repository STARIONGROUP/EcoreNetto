// ------------------------------------------------------------------------------------------------
// <copyright file="GeneralizationHelper.cs" company="Starion Group S.A.">
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

    using HandlebarsDotNet;

    /// <summary>
    /// A block helper that prints the generalization (super-type) information of an <see cref="EClass"/>
    /// </summary>
    public static class GeneralizationHelper
    {
        /// <summary>
        /// Registers the <see cref="GeneralizationHelper"/>
        /// </summary>
        /// <param name="handlebars">
        /// The <see cref="IHandlebars"/> context with which the helper needs to be registered
        /// </param>
        public static void RegisterGeneralizationHelper(this IHandlebars handlebars)
        {
            handlebars.RegisterHelper("Generalization.Interfaces", (writer, context, _) =>
            {
                if (!(context.Value is EClass eClass))
                {
                    throw new ArgumentException("The context shall be an EClass");
                }

                if (eClass.ESuperTypes.Any())
                {
                    var result = $": {string.Join(", ", eClass.ESuperTypes.Select(x => $"I{x.Name}"))}";

                    writer.WriteSafeString(result);
                }
            });

            handlebars.RegisterHelper("Generalization.Classes", (writer, context, _) =>
            {
                if (!(context.Value is EClass eClass))
                {
                    throw new ArgumentException("The context shall be an EClass");
                }

                if (!eClass.ESuperTypes.Any())
                {
                    writer.WriteSafeString($": I{eClass.Name}");
                    return;
                }

                writer.WriteSafeString($": {eClass.ESuperTypes[0].Name}, I{eClass.Name}");
            });
        }
    }
}
