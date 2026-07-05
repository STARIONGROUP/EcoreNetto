// ------------------------------------------------------------------------------------------------
// <copyright file="IInheritanceDiagramRenderer.cs" company="Starion Group S.A.">
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
    /// Definition of a service that renders an Ecore class inheritance diagram as SVG.
    /// </summary>
    public interface IInheritanceDiagramRenderer
    {
        /// <summary>
        /// Renders a model-wide inheritance diagram for the classes in the provided <see cref="HandlebarsPayload"/>.
        /// </summary>
        /// <param name="payload">
        /// The <see cref="HandlebarsPayload"/> that contains the Ecore content.
        /// </param>
        /// <returns>
        /// a string that contains the diagram in SVG format.
        /// </returns>
        string SvgRender(HandlebarsPayload payload);

        /// <summary>
        /// Renders a per-class inheritance tree SVG diagram that highlights the target class and shows all
        /// its ancestors and descendants.
        /// </summary>
        /// <param name="targetClass">
        /// The <see cref="EClass"/> for which to render the inheritance tree.
        /// </param>
        /// <param name="payload">
        /// The <see cref="HandlebarsPayload"/> that contains the Ecore content.
        /// </param>
        /// <returns>
        /// a string that contains the per-class inheritance diagram in SVG format.
        /// </returns>
        string SvgRenderForClass(EClass targetClass, HandlebarsPayload payload);
    }
}
