﻿// -------------------------------------------------------------------------------------------------
// <copyright file="IModelInspector.cs" company="Starion Group S.A.">
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
    using System.IO;

    using System;

    /// <summary>
    /// The purpose of the <see cref="ModelInspector"/> is to iterate through the model and report on the various kinds of
    /// patters that exist in the ECore model that need to be taken into account for code-generation
    /// </summary>
    public interface IModelInspector
    {
        /// <summary>
        /// Inspect the content of the provided <see cref="EPackage"/> and returns the variation 
        /// of data-types, enums and multiplicity as an Analysis report
        /// </summary>
        /// <param name="package">
        /// The <see cref="EPackage"/> that needs to be inspected
        /// </param>
        /// <param name="recursive">
        /// Assertion whether the sub packages should be inspected or not
        /// </param>
        /// <returns>
        /// Returns a report of the classes of interest in the provided package 
        /// </returns>
        public string Inspect(EPackage package, bool recursive = false);

        /// <summary>
        /// Inspect the provided <see cref="EClass"/> (by name) that is contained in the <see cref="EPackage"/>
        /// and returns the variation of data-types, enums and multiplicity as an Analysis report
        /// </summary>
        /// <param name="package">
        /// The <see cref="EPackage"/> that contains the <see cref="EClass"/> that
        /// is to be inspected
        /// </param>
        /// <param name="className">
        /// the name of the class that is to be inspected
        /// </param>
        /// <returns>
        /// returns a report detailing the various combinations of properties of the provided class
        /// </returns>
        public string Inspect(EPackage package, string className);

        /// <summary>
        /// Recursively analyzes the documentation of the model and prints the names of all classes 
        /// and features that do not have any documentation in an analysis report
        /// </summary>
        /// <param name="package">
        /// The <see cref="EPackage"/> which needs to be inspected
        /// </param>
        /// <param name="recursive">
        /// A value indicating whether the sub <see cref="EPackage"/>s need to be Analyzed as well
        /// </param>
        /// <returns>
        /// returns a report of the classes and properties that do not contain any documentation
        /// </returns>
        public string AnalyzeDocumentation(EPackage package, bool recursive = false);

        /// <summary>
        /// Verifies whether the extension of the <paramref name="outputPath"/> is valid or not
        /// </summary>
        /// <param name="outputPath">
        /// The subject <see cref="FileInfo"/> to check
        /// </param>
        /// <returns>
        /// A Tuple of bool and string, where the string contains a description of the verification.
        /// Either stating that the extension is valid or not.
        /// </returns>
        public Tuple<bool, string> IsValidReportExtension(FileInfo outputPath);

        /// <summary>
        /// Generates a text file that contains an inspection report.
        /// </summary>
        /// <param name="modelPath">
        /// the path to the Ecore model of which the report is to be generated.
        /// </param>
        /// <param name="outputPath">
        /// the path, including filename, where the output is to be generated.
        /// </param>
        public void GenerateReport(FileInfo modelPath, FileInfo outputPath);
    }
}
