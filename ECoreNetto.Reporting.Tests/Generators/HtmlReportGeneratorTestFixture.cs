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
        public void Verify_that_the_generated_html_renders_the_rich_feature_set()
        {
            var modelFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "report-features.ecore"));

            var html = this.htmlReportGenerator.GenerateReport(modelFileInfo);

            Assert.Multiple(() =>
            {
                // sections and type classification
                Assert.That(html, Does.Contain("[Primitive Type]"));
                Assert.That(html, Does.Contain("[Data Type]"));
                Assert.That(html, Does.Contain("[Interface]"));
                Assert.That(html, Does.Contain("[Enumeration]"));

                // enum literal value + literal columns
                Assert.That(html, Does.Contain("active"));

                // feature flag chips
                Assert.That(html, Does.Contain("{ordered}"));
                Assert.That(html, Does.Contain("{unique}"));
                Assert.That(html, Does.Contain("{id}"));
                Assert.That(html, Does.Contain("{readonly}"));
                Assert.That(html, Does.Contain("{transient}"));
                Assert.That(html, Does.Contain("{volatile}"));
                Assert.That(html, Does.Contain("{unsettable}"));
                Assert.That(html, Does.Contain("{derived}"));
                Assert.That(html, Does.Contain("{composite}"));
                Assert.That(html, Does.Contain("{opposite:"));

                // specializations / containers
                Assert.That(html, Does.Contain("Specializations"));
                Assert.That(html, Does.Contain("Containers"));

                // operations table with a rendered parameter and return type
                Assert.That(html, Does.Contain("Operations"));
                Assert.That(html, Does.Contain("greet"));
                Assert.That(html, Does.Contain("greeting"));

                // OCL constraint (rule)
                Assert.That(html, Does.Contain("hasFullName"));
                Assert.That(html, Does.Contain("self.fullName"));

                // collapsible UX, diagrams, custom-HTML injection point and Open Graph metadata
                Assert.That(html, Does.Contain("collapsible-section"));
                Assert.That(html, Does.Contain("expand-all"));
                Assert.That(html, Does.Contain("inheritance-diagram"));
                Assert.That(html, Does.Contain("inheritance-tree-"));
                Assert.That(html, Does.Contain("association-diagram-"));
                Assert.That(html, Does.Contain("download-svg"));
                Assert.That(html, Does.Contain("og:title"));
            });
        }

        [Test]
        public void Verify_that_an_existing_report_file_is_overwritten()
        {
            var modelFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"));

            var reportFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "html-report.overwrite.html"));

            this.htmlReportGenerator.GenerateReport(modelFileInfo, reportFileInfo);

            // a second generation to the same path must overwrite the existing file without throwing
            Assert.That(() => this.htmlReportGenerator.GenerateReport(modelFileInfo, reportFileInfo), Throws.Nothing);
            Assert.That(reportFileInfo.Exists, Is.True);
        }

        [Test]
        public void Verify_that_the_custom_html_is_injected_into_the_report()
        {
            var modelFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"));

            const string customHtml = "<div id=\"my-injected-block\">custom-content-marker</div>";

            var html = this.htmlReportGenerator.GenerateReport(modelFileInfo, customHtml);

            Assert.That(html, Does.Contain("custom-content-marker"));
        }

        [Test]
        public void Verify_that_the_default_constructor_generates_a_report()
        {
            var generator = new HtmlReportGenerator();

            var modelFileInfo = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"));

            Assert.That(() => generator.GenerateReport(modelFileInfo), Throws.Nothing);
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

        [Test]
        public void Verify_that_combined_report_methods_throw_when_arguments_are_null()
        {
            FileInfo? modelFileInfo = null;
            DirectoryInfo? directory = null;
            var validModel = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"));
            var validOutput = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "html-report.combined-null.html"));

            Assert.Multiple(() =>
            {
                Assert.That(() => this.htmlReportGenerator.GenerateCombinedReport(modelFileInfo!), Throws.ArgumentNullException);
                Assert.That(() => this.htmlReportGenerator.GenerateCombinedReport(directory!), Throws.ArgumentNullException);
                Assert.That(() => this.htmlReportGenerator.GenerateCombinedReport(modelFileInfo!, validOutput), Throws.ArgumentNullException);
                Assert.That(() => this.htmlReportGenerator.GenerateCombinedReport(validModel, (FileInfo)null!), Throws.ArgumentNullException);
                Assert.That(() => this.htmlReportGenerator.GenerateCombinedReport(directory!, validOutput), Throws.ArgumentNullException);
                Assert.That(() => this.htmlReportGenerator.GenerateCombinedReport(new DirectoryInfo(TestContext.CurrentContext.TestDirectory), (FileInfo)null!), Throws.ArgumentNullException);
            });
        }
    }
}
