﻿// -------------------------------------------------------------------------------------------------
// <copyright file="IMarkdownReportGenerator.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Reporting.Generators
{
    using System.IO;

    /// <summary>
    /// The purpose of the <see cref="IMarkdownReportGenerator"/> is to generate a Markdown report of an
    /// Ecore Model
    /// </summary>
    public interface IMarkdownReportGenerator : IReportGenerator
    {
        /// <summary>
        /// Generates a Markdown document with a table that contains all classes, attributes and their documentation
        /// </summary>
        /// <param name="modelPath">
        /// the path to the Ecore model of which the report is to be generated.
        /// </param>
        /// <returns>
        /// the content of a Markdown report in a string
        /// </returns>
        public string GenerateReport(FileInfo modelPath);
    }
}
