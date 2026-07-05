// ------------------------------------------------------------------------------------------------
// <copyright file="MarkdownReportGeneratorTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="MarkdownReportGenerator"/> class.
    /// </summary>
    [TestFixture]
    public class MarkdownReportGeneratorTestFixture
    {
        private MarkdownReportGenerator markdownReportGenerator = null!;

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
            this.markdownReportGenerator = new MarkdownReportGenerator(this.loggerFactory);
        }

        [Test]
        [TestCase("ecore")]
        [TestCase("recipe")]
        [TestCase("wizardEcore")]
        public void Verify_that_the_report_generator_generates_a_report(string model)
        {
            var modelFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", $"{model}.ecore"));

            var reportFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, $"markdown-report.{model}.md"));

            Assert.That(() => this.markdownReportGenerator.GenerateReport(modelFileInfo, reportFileInfo), Throws.Nothing);
        }

        [Test]
        public void Verify_that_combined_markdown_report_methods_throw_when_arguments_are_null()
        {
            var validModel = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"));
            var validOutput = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "markdown-report.null.md"));

            Assert.Multiple(() =>
            {
                Assert.That(() => this.markdownReportGenerator.GenerateCombinedReport((FileInfo)null!), Throws.ArgumentNullException);
                Assert.That(() => this.markdownReportGenerator.GenerateCombinedReport((DirectoryInfo)null!), Throws.ArgumentNullException);
                Assert.That(() => this.markdownReportGenerator.GenerateCombinedReport((FileInfo)null!, validOutput), Throws.ArgumentNullException);
                Assert.That(() => this.markdownReportGenerator.GenerateCombinedReport(validModel, null!), Throws.ArgumentNullException);
                Assert.That(() => this.markdownReportGenerator.GenerateCombinedReport(new DirectoryInfo(TestContext.CurrentContext.TestDirectory), null!), Throws.ArgumentNullException);
            });
        }

        [Test]
        public void Verify_that_the_generated_markdown_contains_the_expected_content()
        {
            var modelFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"));

            var markdown = this.markdownReportGenerator.GenerateReport(modelFileInfo);

            Assert.Multiple(() =>
            {
                // the model-information block (with the namespace uri), the section headers and a
                // representative class and enum from the recipe model must all be rendered
                Assert.That(markdown, Does.Contain("## Model Information"));
                Assert.That(markdown, Does.Contain("hu.bme.mit.mdsd.recipe"));
                Assert.That(markdown, Does.Contain("## Data Types"));
                Assert.That(markdown, Does.Contain("## Enumeration Types"));
                Assert.That(markdown, Does.Contain("## Classes"));
                Assert.That(markdown, Does.Contain("Container"));
                Assert.That(markdown, Does.Contain("Unit"));
            });
        }
    }
}
