// ------------------------------------------------------------------------------------------------
// <copyright file="ClassExtensions.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="EClass"/> class
    /// </summary>
    public static class ClassExtensions
    {
        /// <summary>
        /// Queries for the subject <see cref="EClass"/> which are specializations (sub classes). 
        /// </summary>
        /// <param name="class">
        /// The <see cref="EClass"/> for which the specializations need to be computed
        /// </param>
        /// <param name="allClasses">
        /// The <see cref="IEnumerable{EClass}"/> from which the generalization / specializations are computed
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{EClass}"/> that contains the specializations (sub classes) if the subject <see cref="EClass"/>
        /// </returns>
        public static IEnumerable<EClass> QuerySpecializations(this EClass @class, IEnumerable<EClass> allClasses)
        {
            if (@class == null)
            {
                throw new ArgumentNullException(nameof(@class));
            }

            var result = allClasses
                .Where(x => x.ESuperTypes.Contains(@class));

            return result;
        }

        /// <summary>
        /// Queries the type hierarchy (chain of super classes) of the provided <see cref="EClass"/>
        /// </summary>
        /// <param name="class">
        /// The subject <see cref="EClass"/> for which the type hierarchy (chain of super classes) needs to be computed
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{EClass}"/> that contains the chain of superclasses
        /// </returns>
        public static IEnumerable<EClass> QueryTypeHierarchy(this EClass @class)
        {
            if (@class == null)
            {
                throw new ArgumentNullException(nameof(@class));
            }

            var result = new List<EClass>();

            foreach (var superType in @class.ESuperTypes)
            {
                result.Add(superType);

                result.AddRange(superType.QueryTypeHierarchy());
            }

            return result;
        }
    }
}
