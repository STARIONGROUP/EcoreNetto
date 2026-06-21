// -------------------------------------------------------------------------------------------------
// <copyright file="XlReportGeneratorTestFixture.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Reporting.Tests.Generators
{
    using System.IO;
    using System.Linq;

    using ClosedXML.Excel;

    using ECoreNetto.Reporting.Generators;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="XlReportGenerator"/> class.
    /// </summary>
    [TestFixture]
    public class XlReportGeneratorTestFixture
    {
        private XlReportGenerator generator = null!;

        private FileInfo modelFileInfo = null!;

        private FileInfo reportFileInfo = null!;

        [SetUp]
        public void SetUp()
        {
            this.generator = new XlReportGenerator();

            this.modelFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"));
            this.reportFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "xl-report.xlsx"));
        }

        [Test]
        public void Verify_that_the_info_sheet_contains_distinct_ns_prefix_and_ns_uri_rows()
        {
            this.generator.GenerateReport(this.modelFileInfo, this.reportFileInfo);

            using var workbook = new XLWorkbook(this.reportFileInfo.FullName);
            var info = workbook.Worksheet("Model Info");

            Assert.Multiple(() =>
            {
                Assert.That(info.Cell(4, 1).GetString(), Is.EqualTo("Root Package - ns prefix"));
                Assert.That(info.Cell(4, 2).GetString(), Is.EqualTo("recipe"));
                Assert.That(info.Cell(5, 1).GetString(), Is.EqualTo("Root Package - ns uri"));
                Assert.That(info.Cell(5, 2).GetString(), Is.EqualTo("hu.bme.mit.mdsd.recipe"));
            });
        }

        [Test]
        public void Verify_that_the_class_enum_and_datatype_sheets_contain_the_expected_content()
        {
            this.generator.GenerateReport(this.modelFileInfo, this.reportFileInfo);

            using var workbook = new XLWorkbook(this.reportFileInfo.FullName);

            var eClassSheet = workbook.Worksheet("EClass");
            var eEnumSheet = workbook.Worksheet("EEnum");
            var eDataTypeSheet = workbook.Worksheet("EDataType");

            Assert.Multiple(() =>
            {
                // the EClass sheet must list representative classes and a feature of the recipe model
                Assert.That(eClassSheet.CellsUsed().Any(c => c.GetString() == "Container"), Is.True);
                Assert.That(eClassSheet.CellsUsed().Any(c => c.GetString() == "Recipe"), Is.True);
                Assert.That(eClassSheet.CellsUsed().Any(c => c.GetString() == "capacity"), Is.True);

                // the EEnum sheet must list the Unit enumeration and one of its literals
                Assert.That(eEnumSheet.CellsUsed().Any(c => c.GetString() == "Unit"), Is.True);
                Assert.That(eEnumSheet.CellsUsed().Any(c => c.GetString() == "PIECE"), Is.True);

                // the EDataType sheet must carry its column header
                Assert.That(eDataTypeSheet.CellsUsed().Any(c => c.GetString() == "DataType"), Is.True);
            });
        }

        [Test]
        public void Verify_that_IsValidReportExtension_returns_expected_results()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.generator.IsValidReportExtension(new FileInfo("report.xlsx")).Item1, Is.True);
                Assert.That(this.generator.IsValidReportExtension(new FileInfo("report.txt")).Item1, Is.False);
            });
        }
    }
}
