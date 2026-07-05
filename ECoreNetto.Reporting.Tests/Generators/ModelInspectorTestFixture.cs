// ------------------------------------------------------------------------------------------------
// <copyright file="ModelInspectorTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tools.Tests.Generators
{
    using System.IO;

    using ECoreNetto.Resource;
    using ECoreNetto.Reporting.Generators;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Serilog;

    [TestFixture]
    public class ModelInspectorTestFixture
    {
        private FileInfo modelFileInfo = null!;

        private FileInfo reportFileInfo = null!;

        private EPackage rootPackage = null!;

        private ModelInspector modelInspector = null!;

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
            var filePath = Path.GetFullPath(path);
            var uri = new System.Uri(filePath);

            var resourceSet = new ResourceSet(this.loggerFactory);
            var resource = resourceSet.CreateResource(uri);

            this.rootPackage = resource.Load(null);

            var modelPath = Path.GetFullPath(path);
            this.modelFileInfo = new FileInfo(modelPath);

            var outputPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx");
            this.reportFileInfo = new FileInfo(outputPath);

            this.modelInspector = new ModelInspector(this.loggerFactory);
        }

        [Test]
        public void Verify_that_inspects_non_recursive_executes_as_expected()
        {
            var report = this.modelInspector.Inspect(this.rootPackage, false);

            Assert.Multiple(() =>
            {
                Assert.That(report, Does.Contain("ANALYSIS"));
                Assert.That(report, Does.Contain("MULTIPLICITY RESULTS"));
                Assert.That(report, Does.Contain("INTERESTING CLASSES"));
            });
        }

        [Test]
        public void Verify_that_inspects_recursive_executes_as_expected()
        {
            var report = this.modelInspector.Inspect(this.rootPackage, true);

            Assert.Multiple(() =>
            {
                Assert.That(report, Does.Contain("MULTIPLICITY RESULTS"));
                // recursion descends into the recipe package, so its name must appear in the analysis
                Assert.That(report, Does.Contain("recipe"));
            });
        }

        [Test]
        public void Verify_that_inspect_class_executes_as_expected()
        {
            var report = this.modelInspector.Inspect(this.rootPackage, "Container");

            // the per-class inspection must be headed by the inspected class
            Assert.That(report, Does.Contain("Container"));
        }

        [Test]
        public void Verify_that_analyze_docs_non_recursive_executes_as_expected()
        {
            var report = this.modelInspector.AnalyzeDocumentation(this.rootPackage, false);

            Assert.That(report, Does.Contain("MISSING DOCUMENTATION"));
        }

        [Test]
        public void Verify_that_analyze_docs_recursive_executes_as_expected()
        {
            var report = this.modelInspector.AnalyzeDocumentation(this.rootPackage, true);

            Assert.That(report, Does.Contain("MISSING DOCUMENTATION"));
        }

        [Test]
        public void Verify_that_the_report_generator_generates_a_report()
        {
            Assert.That(() => this.modelInspector.GenerateReport(modelFileInfo, reportFileInfo), Throws.Nothing);
        }

        [Test]
        public void Verify_that_IsValidExcelReportExtension_returns_false_when_invalid()
        {
            var inValidFileName = new FileInfo("output-report.invalid");
            var invalidResult = this.modelInspector.IsValidReportExtension(inValidFileName);

            Assert.Multiple(() =>
            {
                Assert.That(invalidResult.Item1, Is.False);
                Assert.That(invalidResult.Item2,
                    Is.EqualTo("The Extension of the output file '.invalid' is not supported. Supported extensions is '.txt'"));
            });
        }
    }
}
