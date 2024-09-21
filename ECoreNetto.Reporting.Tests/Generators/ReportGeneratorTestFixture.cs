// -------------------------------------------------------------------------------------------------
// <copyright file="XlReportGeneratorTestFixture.cs" company="Starion Group S.A">
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
        private FileInfo modelFileInfo;

        private FileInfo reportFileInfo;

        private XlReportGenerator xlXlReportGenerator;

        private ILoggerFactory loggerFactory;

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