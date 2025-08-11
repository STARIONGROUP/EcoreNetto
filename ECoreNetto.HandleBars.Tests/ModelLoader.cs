// -------------------------------------------------------------------------------------------------
// <copyright file="ModelLoader.cs" company="Starion Group S.A.">
// 
//   Copyright 2017-2025 Starion Group S.A.
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
