// ------------------------------------------------------------------------------------------------
// <copyright file="IModelInspector.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Reporting.Generators
{
    /// <summary>
    /// The purpose of the <see cref="IModelInspector"/> is to iterate through the model and report on the various kinds of
    /// patters that exist in the ECore model that need to be taken into account for code-generation
    /// </summary>
    public interface IModelInspector : IReportGenerator
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
    }
}
