// ------------------------------------------------------------------------------------------------
// <copyright file="AnchorExtensions.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extension methods that produce stable, collision-free HTML anchors for <see cref="EObject"/>s.
    /// </summary>
    public static class AnchorExtensions
    {
        /// <summary>
        /// Queries a stable, unique HTML anchor identifier for the provided <see cref="EObject"/>, derived
        /// from its (unique) <see cref="EObject.Identifier"/> by replacing every run of characters that are
        /// not valid in an HTML fragment with a single dash.
        /// </summary>
        /// <param name="eObject">
        /// The <see cref="EObject"/> for which the anchor is computed.
        /// </param>
        /// <returns>
        /// A sanitized anchor slug (e.g. <c>recipe-ecore-Recipe</c>), suitable for use as an <c>id</c>
        /// attribute and as an <c>href="#..."</c> target.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="eObject"/> is null.
        /// </exception>
        public static string QueryAnchorId(this EObject eObject)
        {
            if (eObject == null)
            {
                throw new ArgumentNullException(nameof(eObject));
            }

            // A classifier that is referenced across resources (e.g. a super-type, an inherited feature's
            // containing class or a feature type defined in another .ecore) may be an unattached proxy with
            // no EContainer. Computing its Identifier would dereference a null EPackage, so fall back to the
            // simple name for such detached classifiers.
            var identifier = eObject is EClassifier { EContainer: null } detachedClassifier
                ? detachedClassifier.Name ?? string.Empty
                : eObject.Identifier ?? string.Empty;

            var sanitized = Regex.Replace(identifier, "[^A-Za-z0-9]+", "-").Trim('-');

            return string.IsNullOrEmpty(sanitized) ? "anchor" : sanitized;
        }
    }
}
