// ------------------------------------------------------------------------------------------------
// <copyright file="CapellaMarkdownReportGeneratorTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests that verify the <see cref="MarkdownReportGenerator"/> produces a Markdown report for the
    /// Eclipse Capella metamodel (21 cross-referencing <c>.ecore</c> files), both per file and as a single
    /// combined report of the whole metamodel.
    /// </summary>
    [TestFixture]
    public class CapellaMarkdownReportGeneratorTestFixture
    {
        private static readonly string CapellaDirectory = Path.Combine(
            Path.GetDirectoryName(typeof(CapellaMarkdownReportGeneratorTestFixture).Assembly.Location)!,
            "Data",
            "capella");

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
        [TestCaseSource(nameof(CapellaModelFiles))]
        public void Verify_that_a_markdown_report_is_generated_for_a_capella_model(string fileName)
        {
            var modelFileInfo = new FileInfo(Path.Combine(CapellaDirectory, fileName));

            var reportFileInfo = new FileInfo(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                $"markdown-report.capella.{Path.GetFileNameWithoutExtension(fileName)}.md"));

            Assert.That(() => this.markdownReportGenerator.GenerateReport(modelFileInfo, reportFileInfo), Throws.Nothing);

            reportFileInfo.Refresh();
            Assert.That(reportFileInfo.Exists, Is.True);
        }

        [Test]
        public void Verify_that_a_single_combined_markdown_report_is_generated_for_the_whole_capella_metamodel()
        {
            var inputDirectory = new DirectoryInfo(CapellaDirectory);

            var reportFileInfo = new FileInfo(Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "markdown-report.capella.combined.md"));

            Assert.That(() => this.markdownReportGenerator.GenerateCombinedReport(inputDirectory, reportFileInfo), Throws.Nothing);

            reportFileInfo.Refresh();
            Assert.That(reportFileInfo.Exists, Is.True);

            var markdown = File.ReadAllText(reportFileInfo.FullName);

            Assert.Multiple(() =>
            {
                // a single document must contain classifiers that originate from several different .ecore files
                Assert.That(markdown, Does.Contain("CapellaElement"));       // CapellaCore.ecore
                Assert.That(markdown, Does.Contain("LogicalArchitecture"));  // LogicalArchitecture.ecore
                Assert.That(markdown, Does.Contain("SystemAnalysis"));       // ContextArchitecture.ecore
                Assert.That(markdown, Does.Contain("PhysicalArchitecture")); // PhysicalArchitecture.ecore
                Assert.That(markdown, Does.Contain("OperationalAnalysis"));  // OperationalAnalysis.ecore
            });
        }

        [Test]
        public void Verify_that_a_combined_markdown_report_from_an_entry_file_includes_reachable_models()
        {
            var modelFileInfo = new FileInfo(Path.Combine(CapellaDirectory, "LogicalArchitecture.ecore"));

            var markdown = this.markdownReportGenerator.GenerateCombinedReport(modelFileInfo);

            Assert.Multiple(() =>
            {
                Assert.That(markdown, Does.Contain("LogicalArchitecture"));
                Assert.That(markdown, Does.Contain("ComponentArchitecture"));
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
