// -------------------------------------------------------------------------------------------------
// <copyright file="ResourceLoader.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Tools.Resources
{
    using System.Reflection;

    using SharedResourceLoader = ECoreNetto.Reporting.Resources.ResourceLoader;

    /// <summary>
    /// Class responsible for loading embedded resources from this assembly and for the
    /// Tools-specific version and logo queries.
    /// </summary>
    public static class ResourceLoader
    {
        /// <summary>
        /// Load an embedded resource from the executing (ECoreNetto.Tools) assembly
        /// </summary>
        /// <param name="path">
        /// The path of the embedded resource
        /// </param>
        /// <returns>
        /// a string containing the contents of the embedded resource
        /// </returns>
        public static string LoadEmbeddedResource(string path)
        {
            return SharedResourceLoader.LoadEmbeddedResource(Assembly.GetExecutingAssembly(), path);
        }

        /// <summary>
        /// queries the version number from the executing assembly
        /// </summary>
        /// <returns>
        /// a string representation of the version of the application
        /// </returns>
        public static string? QueryVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetName().Version?.ToString();
        }

        /// <summary>
        /// Queries the logo with version info from the embedded resources
        /// </summary>
        /// <returns>
        /// the logo
        /// </returns>
        public static string QueryLogo()
        {
            var version = QueryVersion();

            var logo = LoadEmbeddedResource("ECoreNetto.Tools.Resources.ascii-art.txt")
                .Replace("EcoreToolsVersion", version);

            return logo;
        }
    }
}
