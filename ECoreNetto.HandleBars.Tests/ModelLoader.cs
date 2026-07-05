// ------------------------------------------------------------------------------------------------
// <copyright file="ModelLoader.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars.Tests
{
    using System.IO;
    using System;

    using ECoreNetto.Resource;

    /// <summary>
    /// helper class to load a model from a path
    /// </summary>
    public static class ModelLoader
    {
        /// <summary>
        /// load the model at the specified path
        /// </summary>
        /// <param name="path">
        /// the model path
        /// </param>
        /// <returns>
        /// the root <see cref="EPackage"/>
        /// </returns>
        public static EPackage Load(string path)
        {
            var filePath = Path.GetFullPath(path);
            var uri = new Uri(filePath);

            var resourceSet = new ResourceSet();
            var resource = resourceSet.CreateResource(uri);

            var root = resource.Load(null);

            return root;
        }
    }
}
