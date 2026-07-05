// ------------------------------------------------------------------------------------------------
// <copyright file="SvgDrawingHelper.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Reporting.Drawing
{
    using System.Drawing;

    /// <summary>
    /// static class that provides helper methods for SVG drawing
    /// </summary>
    public static class SvgDrawingHelper
    {
        /// <summary>
        /// Estimates the size of a box based on the text that is used as its label
        /// </summary>
        /// <param name="label">
        /// the content of the label
        /// </param>
        /// <param name="fontSize">
        /// The size of the used font
        /// </param>
        /// <param name="padding">
        /// the padding surrounding the text
        /// </param>
        /// <returns>
        /// the estimated height and width of the box
        /// </returns>
        /// <remarks>
        /// The average character width is around 60% of the font size
        /// </remarks>
        public static (double Height, double Width) EstimateBoxSize(string label, double fontSize = 12, double padding = 10)
        {
            var avgCharWidth = 0.6 * fontSize;

            var width = (label.Length * avgCharWidth) + 2 * padding;

            var height = fontSize + 2 * padding;

            return (height, width);
        }

        /// <summary>
        /// Converts a <see cref="Microsoft.Msagl.Core.Geometry.Point"/> to a <see cref="System.Drawing.PointF"/>.
        /// </summary>
        /// <param name="pt">
        /// The <see cref="Microsoft.Msagl.Core.Geometry.Point"/> to convert.
        /// </param>
        /// <returns>
        /// A <see cref="System.Drawing.PointF"/> with the same X and Y coordinates as the input point,
        /// cast to <see cref="float"/>.
        /// </returns>
        public static PointF ToPointF(Microsoft.Msagl.Core.Geometry.Point pt)
        {
            return new PointF((float)pt.X, (float)pt.Y);
        }
    }
}
