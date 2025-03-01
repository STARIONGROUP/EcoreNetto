// -------------------------------------------------------------------------------------------------
// <copyright file="PackageExtensions.cs" company="Starion Group S.A">
// 
//   Copyright 2017-2024 Starion Group S.A.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
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
