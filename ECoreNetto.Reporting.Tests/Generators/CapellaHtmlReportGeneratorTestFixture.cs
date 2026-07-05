// ------------------------------------------------------------------------------------------------
// <copyright file="CapellaHtmlReportGeneratorTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tools.Tests.Generators
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ECoreNetto.Reporting.Generators;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Serilog;

    /// <summary>
    /// Suite of tests that verify the <see cref="HtmlReportGenerator"/> produces an HTML report for the
    /// Eclipse Capella metamodel (21 cross-referencing <c>.ecore</c> files whose cross-file references are
    /// demand-loaded and resolved while a single entry file is rendered).
    /// </summary>
    [TestFixture]
    public class CapellaHtmlReportGeneratorTestFixture
    {
        private static readonly string CapellaDirectory = Path.Combine(
            Path.GetDirectoryName(typeof(CapellaHtmlReportGeneratorTestFixture).Assembly.Location)!,
            "Data",
            "capella");

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
        [TestCaseSource(nameof(CapellaModelFiles))]
        public void Verify_that_an_html_report_is_generated_for_a_capella_model(string fileName)
        {
            var modelFileInfo = new FileInfo(Path.Combine(CapellaDirectory, fileName));

            var reportFileInfo = new FileInfo(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                $"html-report.capella.{Path.GetFileNameWithoutExtension(fileName)}.html"));

            Assert.That(() => this.htmlReportGenerator.GenerateReport(modelFileInfo, reportFileInfo), Throws.Nothing);

            reportFileInfo.Refresh();
            Assert.That(reportFileInfo.Exists, Is.True);
        }

        [Test]
        public void Verify_that_the_generated_capella_html_contains_expected_content()
        {
            var modelFileInfo = new FileInfo(Path.Combine(CapellaDirectory, "CapellaCore.ecore"));

            var html = this.htmlReportGenerator.GenerateReport(modelFileInfo);

            Assert.Multiple(() =>
            {
                Assert.That(html, Does.Contain("capellacore"));
                Assert.That(html, Does.Contain("Classes"));
                Assert.That(html, Does.Contain("CapellaElement"));
            });
        }

        [Test]
        public void Verify_that_a_single_combined_html_report_is_generated_for_the_whole_capella_metamodel()
        {
            var inputDirectory = new DirectoryInfo(CapellaDirectory);

            var reportFileInfo = new FileInfo(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "html-report.capella.combined.html"));

            Assert.That(() => this.htmlReportGenerator.GenerateCombinedReport(inputDirectory, reportFileInfo), Throws.Nothing);

            reportFileInfo.Refresh();
            Assert.That(reportFileInfo.Exists, Is.True);

            var html = File.ReadAllText(reportFileInfo.FullName);

            Assert.Multiple(() =>
            {
                // a single document must contain classifiers that originate from several different .ecore files
                Assert.That(html, Does.Contain("CapellaElement"));       // CapellaCore.ecore
                Assert.That(html, Does.Contain("LogicalArchitecture"));  // LogicalArchitecture.ecore
                Assert.That(html, Does.Contain("SystemAnalysis"));       // ContextArchitecture.ecore
                Assert.That(html, Does.Contain("PhysicalArchitecture")); // PhysicalArchitecture.ecore
                Assert.That(html, Does.Contain("OperationalAnalysis"));  // OperationalAnalysis.ecore
            });
        }

        [Test]
        public void Verify_that_a_combined_report_from_an_entry_file_includes_reachable_models()
        {
            // LogicalArchitecture.ecore references classifiers that live in other files (e.g. its super type
            // ComponentArchitecture lives in CompositeStructure.ecore); the combined report must include them.
            var modelFileInfo = new FileInfo(Path.Combine(CapellaDirectory, "LogicalArchitecture.ecore"));

            var html = this.htmlReportGenerator.GenerateCombinedReport(modelFileInfo);

            Assert.Multiple(() =>
            {
                Assert.That(html, Does.Contain("LogicalArchitecture"));
                Assert.That(html, Does.Contain("ComponentArchitecture"));
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
