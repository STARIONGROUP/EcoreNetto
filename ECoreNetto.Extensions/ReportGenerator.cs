// -------------------------------------------------------------------------------------------------
// <copyright file="ReportGenerator.cs" company="Starion Group S.A">
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
    using System;
    using System.Data;
    using System.IO;
    using System.Linq;

    using ClosedXML.Excel;

    using ECoreNetto.Resource;

    /// <summary>
    /// The purpose of the <see cref="ReportGenerator"/> is to generate reports of an
    /// Ecore Model
    /// </summary>
    public class ReportGenerator : IReportGenerator
    {
        /// <summary>
        /// Generates a table that contains all classes, attributes and their documentation
        /// </summary>
        /// <param name="modelPath">
        /// the path to the Ecore model of which the report is to be generated.
        /// </param>
        /// <param name="outputPath">
        /// the path, including filename, where the output is to be generated.
        /// </param>
        public void GenerateTable(FileInfo modelPath, FileInfo outputPath)
        {
            if (modelPath == null)
            {
                throw new ArgumentNullException(nameof(modelPath));
            }

            if (outputPath == null)
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            var rootPackage = this.LoadRootPackage(modelPath);

            var packages = rootPackage.QueryPackages();

            using (var workbook = new XLWorkbook())
            {
                var classWorksheet = workbook.Worksheets.Add("Classes");

                var dataTable = new DataTable();

                dataTable.Columns.Add("Class", typeof(string));
                dataTable.Columns.Add("Feature", typeof(string));
                dataTable.Columns.Add("EType", typeof(string));
                dataTable.Columns.Add("Multiplicity", typeof(string));
                dataTable.Columns.Add("IsContainment", typeof(string));
                dataTable.Columns.Add("Documentation", typeof(string));

                foreach (var package in packages)
                {
                    foreach (var eClass in package.EClassifiers.OfType<EClass>().OrderBy(x => x.Name))
                    {
                        var classDataRow = dataTable.NewRow();
                        classDataRow["Class"] = eClass.Name;
                        classDataRow["Feature"] = "--";
                        classDataRow["EType"] = "--";
                        classDataRow["Multiplicity"] = "--";
                        classDataRow["IsContainment"] = "--";
                        classDataRow["Documentation"] = eClass.QueryRawDocumentation();
                        dataTable.Rows.Add(classDataRow);

                        foreach (var structuralFeature in eClass.EStructuralFeatures)
                        {
                            if (structuralFeature.Derived || structuralFeature.Transient)
                            {
                                continue;
                            }

                            var structuralFeatureDataRow = dataTable.NewRow();
                            structuralFeatureDataRow["Class"] = eClass.Name;
                            structuralFeatureDataRow["Feature"] = structuralFeature.Name;
                            structuralFeatureDataRow["EType"] = structuralFeature.EType.Name;
                            structuralFeatureDataRow["Multiplicity"] = $"{structuralFeature.LowerBound}:{structuralFeature.UpperBound}";
                            structuralFeatureDataRow["IsContainment"] = structuralFeature.QueryIsContainment();
                            structuralFeatureDataRow["Documentation"] = structuralFeature.QueryRawDocumentation();

                            dataTable.Rows.Add(structuralFeatureDataRow);
                        }
                    }
                }

                classWorksheet.Cell(1, 1).InsertTable(dataTable, "Classes", true);

                try {
                    classWorksheet.Rows().AdjustToContents();
                    classWorksheet.Columns().AdjustToContents();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Problem loading fonts {0}", e);
                }

                var enumWorksheet = workbook.Worksheets.Add("Enums");

                dataTable = new DataTable();

                dataTable.Columns.Add("Enum", typeof(string));
                dataTable.Columns.Add("Literal", typeof(string));
                dataTable.Columns.Add("Documentation", typeof(string));

                foreach (var package in packages)
                {
                    foreach (var eEnum in package.EClassifiers.OfType<EEnum>().OrderBy(x => x.Name))
                    {
                        var enumDataRow = dataTable.NewRow();
                        enumDataRow["Enum"] = eEnum.Name;
                        enumDataRow["Literal"] = "--";
                        enumDataRow["Documentation"] = eEnum.QueryRawDocumentation();
                        dataTable.Rows.Add(enumDataRow);

                        foreach (var eEnumLiteral in eEnum.ELiterals)
                        {
                            var eEnumLiteralRow = dataTable.NewRow();
                            eEnumLiteralRow["Enum"] = eEnum.Name;
                            eEnumLiteralRow["Literal"] = eEnumLiteral.Name;
                            eEnumLiteralRow["Documentation"] = eEnumLiteral.QueryRawDocumentation();
                            dataTable.Rows.Add(eEnumLiteralRow);
                        }
                    }
                }

                enumWorksheet.Cell(1, 1).InsertTable(dataTable, "Enums", true);

                try
                {
                    enumWorksheet.Rows().AdjustToContents();
                    enumWorksheet.Columns().AdjustToContents();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Problem loading fonts {0}", e);
                }

                workbook.SaveAs(outputPath.FullName);
            }
        }

        /// <summary>
        /// Loads the root Ecore package from the provided model
        /// </summary>
        /// <param name="modelPath">
        /// the path to the Ecore model that is to be loaded
        /// </param>
        /// <returns>
        /// an instance of <see cref="EPackage"/>
        /// </returns>
        private EPackage LoadRootPackage(FileInfo modelPath)
        {
            var uri = new System.Uri(modelPath.FullName);

            var resourceSet = new ResourceSet();
            var resource = resourceSet.CreateResource(uri);

            var rootPackage = resource.Load(null);

            return rootPackage;
        }
    }
}