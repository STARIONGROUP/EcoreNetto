// ------------------------------------------------------------------------------------------------
// <copyright file="PackageExtensions.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Extension methods for <see cref="PackageExtensions"/> class
    /// </summary>
    public static class PackageExtensions
    {
        /// <summary>
        /// Queries all the packages and subpackages recursively that are contained
        /// by the root package
        /// </summary>
        /// <param name="root">
        /// The root <see cref="EPackage"/>
        /// </param>
        /// <returns>
        /// A  ReadOnlyCollection of all the <see cref="EPackage"/>s that are contained
        /// recursively by the root <see cref="EPackage"/>
        /// </returns>
        public static ReadOnlyCollection<EPackage> QueryPackages(this EPackage root)
        {
            var result = new List<EPackage>();

            if (root == null)
            {
                return result.AsReadOnly();
            }

            result.Add(root);

            if (root.ESubPackages != null)
            {
                foreach (var subPackage in root.ESubPackages)
                {
                    result.AddRange(subPackage.QueryPackages());
                }
            }

            return result.AsReadOnly();
        }
    }
}
