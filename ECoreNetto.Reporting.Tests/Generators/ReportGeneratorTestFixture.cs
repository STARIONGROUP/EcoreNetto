// ------------------------------------------------------------------------------------------------
// <copyright file="ReportGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tools.Tests.Generators
{
    using System.IO;

    using ECoreNetto.Reporting.Generators;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="XlReportGenerator"/> class
    /// </summary>
    [TestFixture]
    public class XlReportGeneratorTestFixture
    {
        private FileInfo modelFileInfo = null!;

        private FileInfo reportFileInfo = null!;

        private XlReportGenerator xlXlReportGenerator = null!;

        private ILoggerFactory loggerFactory = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
        }

        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");
            
            var modelPath = Path.GetFullPath(path);
            this.modelFileInfo = new FileInfo(modelPath);

            var outputPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx");
            this.reportFileInfo = new FileInfo(outputPath);

            this.xlXlReportGenerator = new XlReportGenerator(this.loggerFactory);
        }

        [Test]
        public void Verify_that_the_report_generator_generates_a_report()
        {
            Assert.That(() => this.xlXlReportGenerator.GenerateReport(modelFileInfo, reportFileInfo), Throws.Nothing);
        }

        [Test]
        public void Verify_that_IsValidExcelReportExtension_returns_false_when_invalid()
        {
            var inValidFileName = new FileInfo("output-report.invalid");
            var invalidResult = this.xlXlReportGenerator.IsValidReportExtension(inValidFileName);

            Assert.Multiple(() =>
            {
                Assert.That(invalidResult.Item1, Is.False);
                Assert.That(invalidResult.Item2,
                    Is.EqualTo("The Extension of the output file '.invalid' is not supported. Supported extensions are '.xlsx', '.xlsm', '.xltx' and '.xltm'"));
            });
        }

        [Test]
        [TestCase("xlsx")]
        [TestCase("xltx")]
        [TestCase("xlsm")]
        [TestCase("xltm")]
        public void Verify_that_IsValidExcelReportExtension_returns_true_when_valid(string extension)
        {
            var validFileName = new FileInfo($"output-report.{extension}");
            var validResult = this.xlXlReportGenerator.IsValidReportExtension(validFileName);
            Assert.That(validResult.Item1, Is.True);
            Assert.That(validResult.Item2, Is.EqualTo($".{extension} is a supported report extension"));
        }
    }
}
