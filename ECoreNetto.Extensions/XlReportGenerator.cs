﻿// -------------------------------------------------------------------------------------------------
// <copyright file="XlReportGenerator.cs" company="Starion Group S.A">
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
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using ClosedXML.Excel;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The purpose of the <see cref="XlReportGenerator"/> is to generate reports of an
    /// Ecore Model
    /// </summary>
    public class XlReportGenerator : ReportGenerator, IXlReportGenerator
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<XlReportGenerator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="XlReportGenerator"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public XlReportGenerator(ILoggerFactory loggerFactory = null) : base(loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<XlReportGenerator>.Instance : loggerFactory.CreateLogger<XlReportGenerator>();
        }

        /// <summary>
        /// Generates a table that contains all classes, attributes and their documentation
        /// </summary>
        /// <param name="modelPath">
        /// the path to the Ecore model of which the report is to be generated.
        /// </param>
        /// <param name="outputPath">
        /// the path, including filename, where the output is to be generated.
        /// </param>
        public void GenerateReport(FileInfo modelPath, FileInfo outputPath)
        {
            if (modelPath == null)
            {
                throw new ArgumentNullException(nameof(modelPath));
            }

            if (outputPath == null)
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            var sw = Stopwatch.StartNew();

            this.logger.LogInformation("Start Generating Excel report tables");

            var rootPackage = this.LoadRootPackage(modelPath);

            var packages = rootPackage.QueryPackages();

            using (var workbook = new XLWorkbook())
            {
                this.AddClassSheet(workbook, packages);

                this.AddEnumSheet(workbook, packages);

                this.AddEDataTypeSheet(workbook, packages);

                this.logger.LogInformation("Saving report file to {0}", outputPath.FullName);

                workbook.SaveAs(outputPath.FullName);
            }

            this.logger.LogInformation("Generated Excel report tables in {0} [ms]", sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Adds a worksheet to the workbook with EClass data
        /// </summary>
        /// <param name="workbook">
        /// The target <see cref="XLWorkbook"/> to which the EClass worksheet is added
        /// </param>
        /// <param name="packages">
        /// The <see cref="EPackage"/>s that contain the EClass instances to report on
        /// </param>
        private void AddClassSheet(XLWorkbook workbook, IEnumerable<EPackage> packages)
        {
            this.logger.LogDebug("Add EClass reports");

            var classWorksheet = workbook.Worksheets.Add("EClass");

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

            this.FormatSheet(classWorksheet);
        }

        /// <summary>
        /// Adds a worksheet to the workbook with EEnum data
        /// </summary>
        /// <param name="workbook">
        /// The target <see cref="XLWorkbook"/> to which the EEnum worksheet is added
        /// </param>
        /// <param name="packages">
        /// The <see cref="EPackage"/>s that contain the EEnum instances to report on
        /// </param>
        private void AddEnumSheet(XLWorkbook workbook, IEnumerable<EPackage> packages)
        {
            var enumWorksheet = workbook.Worksheets.Add("EEnum");

            this.logger.LogDebug("Add EEnum reports");

            var dataTable = new DataTable();

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

            this.FormatSheet(enumWorksheet);
        }

        /// <summary>
        /// Adds a worksheet to the workbook with EDataType data
        /// </summary>
        /// <param name="workbook">
        /// The target <see cref="XLWorkbook"/> to which the EDataType worksheet is added
        /// </param>
        /// <param name="packages">
        /// The <see cref="EPackage"/>s that contain the EDataType instances to report on
        /// </param>
        private void AddEDataTypeSheet(XLWorkbook workbook, IEnumerable<EPackage> packages)
        {
            var dataTypeWorksheet = workbook.Worksheets.Add("EDataType");

            this.logger.LogDebug("Add EDataType reports");

            var dataTable = new DataTable();

            dataTable.Columns.Add("DataType", typeof(string));
            dataTable.Columns.Add("Documentation", typeof(string));

            foreach (var package in packages)
            {
                foreach (var eDataType in package.EClassifiers
                             .OfType<EDataType>()
                             .Where(x => !(x is EEnum))
                             .OrderBy(x => x.Name))
                {
                    var dataTypeRow = dataTable.NewRow();
                    dataTypeRow["DataType"] = eDataType.Name;
                    dataTypeRow["Documentation"] = eDataType.QueryRawDocumentation();
                    dataTable.Rows.Add(dataTypeRow);
                }
            }

            dataTypeWorksheet.Cell(1, 1).InsertTable(dataTable, "DataTypes", true);

            this.FormatSheet(dataTypeWorksheet);
        }

        /// <summary>
        /// Format the provided sheet
        /// </summary>
        /// <param name="worksheet">
        /// The <see cref="IXLWorksheet"/> that is to be formatted
        /// </param>
        private void FormatSheet(IXLWorksheet worksheet)
        {
            try
            {
                worksheet.Rows().AdjustToContents();
                worksheet.Columns().AdjustToContents();
            }
            catch (Exception e)
            {
                this.logger.LogWarning("Problem loading fonts when adjusting to contents {0}", e);
            }
        }

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
        public override Tuple<bool, string>  IsValidReportExtension(FileInfo outputPath)
        {
            switch (outputPath.Extension)
            {
                case ".xlsm":
                    return new Tuple<bool, string>(true, ".xlsm is a supported report extension");
                case ".xltm":
                    return new Tuple<bool, string>(true, ".xltm is a supported report extension");
                case ".xlsx":
                    return new Tuple<bool, string>(true, ".xlsx is a supported report extension");
                case ".xltx":
                    return new Tuple<bool, string>(true, ".xltx is a supported report extension");
                default:
                    return new Tuple<bool, string>(false, $"The Extension of the output file '{outputPath.Extension}' is not supported. Supported extensions are '.xlsx', '.xlsm', '.xltx' and '.xltm'");
            }
        }
    }
}