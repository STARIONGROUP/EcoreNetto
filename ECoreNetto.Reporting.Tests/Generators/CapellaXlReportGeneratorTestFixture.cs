// ------------------------------------------------------------------------------------------------
// <copyright file="CapellaXlReportGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Reporting.Tests.Generators
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ClosedXML.Excel;

    using ECoreNetto.Reporting.Generators;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests that verify the <see cref="XlReportGenerator"/> produces an Excel report for the Eclipse
    /// Capella metamodel (21 cross-referencing <c>.ecore</c> files), both per file and as a single combined
    /// workbook of the whole metamodel.
    /// </summary>
    [TestFixture]
    public class CapellaXlReportGeneratorTestFixture
    {
        private static readonly string CapellaDirectory = Path.Combine(
            Path.GetDirectoryName(typeof(CapellaXlReportGeneratorTestFixture).Assembly.Location)!,
            "Data",
            "capella");

        private XlReportGenerator generator = null!;

        [SetUp]
        public void SetUp()
        {
            this.generator = new XlReportGenerator();
        }

        [Test]
        [TestCaseSource(nameof(CapellaModelFiles))]
        public void Verify_that_an_excel_report_is_generated_for_a_capella_model(string fileName)
        {
            var modelFileInfo = new FileInfo(Path.Combine(CapellaDirectory, fileName));

            var reportFileInfo = new FileInfo(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                $"xl-report.capella.{Path.GetFileNameWithoutExtension(fileName)}.xlsx"));

            Assert.That(() => this.generator.GenerateReport(modelFileInfo, reportFileInfo), Throws.Nothing);

            reportFileInfo.Refresh();
            Assert.That(reportFileInfo.Exists, Is.True);
        }

        [Test]
        public void Verify_that_a_single_combined_excel_report_is_generated_for_the_whole_capella_metamodel()
        {
            var inputDirectory = new DirectoryInfo(CapellaDirectory);

            var reportFileInfo = new FileInfo(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "xl-report.capella.combined.xlsx"));

            Assert.That(() => this.generator.GenerateCombinedReport(inputDirectory, reportFileInfo), Throws.Nothing);

            reportFileInfo.Refresh();
            Assert.That(reportFileInfo.Exists, Is.True);

            using var workbook = new XLWorkbook(reportFileInfo.FullName);

            var info = workbook.Worksheet("Model Info");
            var eClassSheet = workbook.Worksheet("EClass");

            Assert.Multiple(() =>
            {
                // the info sheet lists all the included root packages
                Assert.That(info.Cell(6, 1).GetString(), Is.EqualTo("Included Packages"));

                // a single workbook must list classes that originate from several different .ecore files
                Assert.That(eClassSheet.CellsUsed().Any(c => c.GetString() == "CapellaElement"), Is.True);
                Assert.That(eClassSheet.CellsUsed().Any(c => c.GetString() == "LogicalArchitecture"), Is.True);
                Assert.That(eClassSheet.CellsUsed().Any(c => c.GetString() == "SystemAnalysis"), Is.True);
                Assert.That(eClassSheet.CellsUsed().Any(c => c.GetString() == "PhysicalArchitecture"), Is.True);
            });
        }

        [Test]
        public void Verify_that_a_combined_excel_report_from_an_entry_file_includes_reachable_models()
        {
            var modelFileInfo = new FileInfo(Path.Combine(CapellaDirectory, "LogicalArchitecture.ecore"));
            var reportFileInfo = new FileInfo(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "xl-report.capella.reachable.xlsx"));

            Assert.That(() => this.generator.GenerateCombinedReport(modelFileInfo, reportFileInfo), Throws.Nothing);

            reportFileInfo.Refresh();
            Assert.That(reportFileInfo.Exists, Is.True);

            using var workbook = new XLWorkbook(reportFileInfo.FullName);
            var eClassSheet = workbook.Worksheet("EClass");

            Assert.Multiple(() =>
            {
                Assert.That(eClassSheet.CellsUsed().Any(c => c.GetString() == "LogicalArchitecture"), Is.True);
                Assert.That(eClassSheet.CellsUsed().Any(c => c.GetString() == "ComponentArchitecture"), Is.True);
            });
        }

        /// <summary>
        /// Enumerates the Capella <c>.ecore</c> file names available in the test output directory.
        /// </summary>
        private static IEnumerable<string> CapellaModelFiles()
        {
            return Directory.EnumerateFiles(CapellaDirectory, "*.ecore").Select(file => Path.GetFileName(file)!);
        }
    }
}
