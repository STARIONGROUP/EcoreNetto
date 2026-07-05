// ------------------------------------------------------------------------------------------------
// <copyright file="ResourceLoader.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Reporting.Resources
{
    using System.IO;
    using System.Reflection;
    using System.Resources;

    /// <summary>
    /// Class responsible for loading embedded resources.
    /// </summary>
    public static class ResourceLoader
    {
        /// <summary>
        /// Load an embedded resource from the provided <see cref="Assembly"/>
        /// </summary>
        /// <param name="assembly">
        /// The <see cref="Assembly"/> whose manifest contains the embedded resource
        /// </param>
        /// <param name="path">
        /// The path of the embedded resource
        /// </param>
        /// <returns>
        /// a string containing the contents of the embedded resource
        /// </returns>
        public static string LoadEmbeddedResource(Assembly assembly, string path)
        {
            using var stream = assembly.GetManifestResourceStream(path);

            using var reader = new StreamReader(stream ?? throw new MissingManifestResourceException());

            return reader.ReadToEnd();
        }
    }
}
