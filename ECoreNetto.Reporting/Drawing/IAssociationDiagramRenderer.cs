// ------------------------------------------------------------------------------------------------
// <copyright file="IAssociationDiagramRenderer.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Reporting.Drawing
{
    using ECoreNetto.Reporting.Payload;

    /// <summary>
    /// Definition of a service that renders a per-class Ecore association diagram as SVG.
    /// </summary>
    public interface IAssociationDiagramRenderer
    {
        /// <summary>
        /// Renders a per-class association SVG diagram that shows the target class and all classes connected
        /// to it via typed references, with multiplicity labels and UML notation (composition diamonds and
        /// navigability arrows).
        /// </summary>
        /// <param name="targetClass">
        /// The <see cref="EClass"/> for which to render the association diagram.
        /// </param>
        /// <param name="payload">
        /// The <see cref="HandlebarsPayload"/> that contains the Ecore content.
        /// </param>
        /// <returns>
        /// a string that contains the per-class association diagram in SVG format, or an empty string when
        /// the class has no associations.
        /// </returns>
        string SvgRenderForClass(EClass targetClass, HandlebarsPayload payload);
    }
}
