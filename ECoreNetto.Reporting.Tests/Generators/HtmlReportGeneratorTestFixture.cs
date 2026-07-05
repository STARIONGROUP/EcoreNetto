// ------------------------------------------------------------------------------------------------
// <copyright file="HtmlReportGeneratorTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="HtmlReportGenerator"/> class.
    /// </summary>
    [TestFixture]
    public class HtmlReportGeneratorTestFixture
    {
        private HtmlReportGenerator htmlReportGenerator = null!;

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

        [Test]
        public void Verify_that_the_generated_html_contains_the_expected_content()
        {
            var modelFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"));

            var html = this.htmlReportGenerator.GenerateReport(modelFileInfo);

            Assert.Multiple(() =>
            {
                // package name, the three top-level sections and a representative class and enum
                // (and one of its literals) from the recipe model must all be rendered
                Assert.That(html, Does.Contain("recipe"));
                Assert.That(html, Does.Contain("Enumeration Types"));
                Assert.That(html, Does.Contain("Data Types"));
                Assert.That(html, Does.Contain("Classes"));
                Assert.That(html, Does.Contain("Container"));
                Assert.That(html, Does.Contain("Unit"));
                Assert.That(html, Does.Contain("PIECE"));
            });
        }

        [Test]
        public void Verify_that_when_modelpath_is_null_exception_is_thrown()
        {
            FileInfo? modelFileInfo = null;

            Assert.That(() => this.htmlReportGenerator.GenerateReport(modelFileInfo!), Throws.ArgumentNullException);

            FileInfo? reportFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "html-report.ecore.html"));

            Assert.That(() => this.htmlReportGenerator.GenerateReport(modelFileInfo!, reportFileInfo!), Throws.ArgumentNullException);

            modelFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"));
            reportFileInfo = null;

            Assert.That(() => this.htmlReportGenerator.GenerateReport(modelFileInfo!, reportFileInfo!), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_That_when_outputpath_is_null_exception_is_thrown()
        {
            Assert.That(() => this.htmlReportGenerator.IsValidReportExtension(null!), Throws.ArgumentNullException);
        }
    }
}
