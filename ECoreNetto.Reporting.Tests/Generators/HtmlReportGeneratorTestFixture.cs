// -------------------------------------------------------------------------------------------------
// <copyright file="HtmlReportGeneratorTestFixture.cs" company="Starion Group S.A">
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
    /// Suite of tests for the <see cref="HtmlReportGenerator"/> class.
    /// </summary>
    [TestFixture]
    public class HtmlReportGeneratorTestFixture
    {
        private HtmlReportGenerator htmlReportGenerator;

        private ILoggerFactory? loggerFactory;

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
            this.htmlReportGenerator = new HtmlReportGenerator(this.loggerFactory);
        }

        [Test]
        [TestCase("ecore")]
        [TestCase("recipe")]
        [TestCase("wizardEcore")]
        public void Verify_that_the_report_generator_generates_a_report(string model)
        {
            var modelFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", $"{model}.ecore"));

            var reportFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, $"html-report.{model}.html"));

            Assert.That(() => this.htmlReportGenerator.GenerateReport(modelFileInfo, reportFileInfo), Throws.Nothing);
        }
    }
}