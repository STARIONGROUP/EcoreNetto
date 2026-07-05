// ------------------------------------------------------------------------------------------------
// <copyright file="XlReportCommandTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tools.Tests.Commands
{
    using System;
    using System.CommandLine;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using ECoreNetto.Reporting.Generators;
    using ECoreNetto.Tools.Commands;
    using ECoreNetto.Tools.Services;

    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="XlReportCommand"/> class.
    /// </summary>
    [TestFixture]
    public class XlReportCommandTestFixture
    {
        private RootCommand rootCommand = null!;

        private Mock<IXlReportGenerator> reportGenerator = null!;

        private Mock<IVersionChecker> versionChecker = null!;

        private XlReportCommand.Handler handler = null!;

        private CancellationTokenSource cts = null!;

        [SetUp]
        public void SetUp()
        {
            this.cts = new CancellationTokenSource();

            var xlReportCommand = new XlReportCommand();
            this.rootCommand = new RootCommand();
            this.rootCommand.Add(xlReportCommand);

            this.reportGenerator = new Mock<IXlReportGenerator>();
            this.versionChecker = new Mock<IVersionChecker>();

            this.reportGenerator.Setup(x => x.IsValidReportExtension(It.IsAny<FileInfo>()))
                .Returns(new Tuple<bool, string>(true, "valid extension"));

            this.handler = new XlReportCommand.Handler(this.reportGenerator.Object, this.versionChecker.Object);

            // disable the artificial status delay so the generation path runs without latency
            this.handler.StatusDelay = TimeSpan.Zero;
        }

        [Test]
        public void Verify_that_report_command_can_be_constructed()
        {
             Assert.That(() =>
             {
                 var reportCommand = new XlReportCommand();
             }, Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_InvokeAsync_returns_0()
        {
            var args = new[]
            {
                "excel-report",
                "--no-logo",
                "--input-model", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"),
                "--output-report", Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx")
            };

            var parseResult = this.rootCommand.Parse(args);

            var result = await this.handler.InvokeAsync(parseResult, this.cts.Token);

            this.reportGenerator.Verify(x => x.GenerateReport(It.IsAny<FileInfo>(), It.IsAny<FileInfo>()), Times.Once);

            this.versionChecker.Verify(x => x.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.That(result, Is.EqualTo(0), "InvokeAsync should return 0 upon success.");
        }

        [Test]
        public async Task Verify_that_when_the_input_ecore_model_does_not_exists_returns_not_0()
        {
            var args = new[]
            {
                "excel-report",
                "--no-logo",
                "--input-model", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "non-existent.ecore"),
                "--output-report", Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx")
            };

            var parseResult = this.rootCommand.Parse(args);

            var result = await this.handler.InvokeAsync(parseResult, this.cts.Token);

            Assert.That(result, Is.EqualTo(-1), "InvokeAsync should return -1 upon failure.");
        }

        [Test]
        public async Task Verify_that_when_the_output_extensions_is_not_supported_returns_not_0()
        {
            var args = new[]
            {
                "excel-report",
                "--no-logo",
                "--input-model", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"),
            };

            this.reportGenerator.Setup(x => x.IsValidReportExtension(It.IsAny<FileInfo>()))
                .Returns(new Tuple<bool, string>(false, "invalid extension"));

            var parseResult = this.rootCommand.Parse(args);

            var result = await this.handler.InvokeAsync(parseResult, this.cts.Token);

            Assert.That(result, Is.EqualTo(-1), "InvokeAsync should return -1 upon failure.");
        }

        [Test]
        public async Task Verify_that_InvokeAsync_without_no_logo_returns_0()
        {
            // omitting --no-logo exercises the logo-rendering branch
            var args = new[]
            {
                "excel-report",
                "--input-model", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"),
                "--output-report", Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx")
            };

            var parseResult = this.rootCommand.Parse(args);

            var result = await this.handler.InvokeAsync(parseResult, this.cts.Token);

            Assert.That(result, Is.EqualTo(0), "InvokeAsync should return 0 upon success.");
        }

        [Test]
        public async Task Verify_that_InvokeAsync_with_auto_open_report_returns_0()
        {
            // exercises the ExecuteAutoOpenAsync path; the generated file does not exist, so the
            // open attempt fails and is swallowed, but the report generation itself succeeds
            var args = new[]
            {
                "excel-report",
                "--no-logo",
                "--auto-open-report",
                "--input-model", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"),
                "--output-report", Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx")
            };

            var parseResult = this.rootCommand.Parse(args);

            var result = await this.handler.InvokeAsync(parseResult, this.cts.Token);

            this.reportGenerator.Verify(x => x.GenerateReport(It.IsAny<FileInfo>(), It.IsAny<FileInfo>()), Times.Once);

            Assert.That(result, Is.EqualTo(0), "InvokeAsync should return 0 upon success.");
        }

        [Test]
        public async Task Verify_that_when_GenerateReport_throws_IOException_it_is_handled()
        {
            this.reportGenerator.Setup(x => x.GenerateReport(It.IsAny<FileInfo>(), It.IsAny<FileInfo>()))
                .Throws(new IOException("the report file is locked"));

            var args = new[]
            {
                "excel-report",
                "--no-logo",
                "--input-model", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"),
                "--output-report", Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx")
            };

            var parseResult = this.rootCommand.Parse(args);

            // the IOException is caught and reported to the console; the call completes without throwing
            var result = await this.handler.InvokeAsync(parseResult, this.cts.Token);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task Verify_that_when_GenerateReport_throws_an_exception_returns_minus_1()
        {
            this.reportGenerator.Setup(x => x.GenerateReport(It.IsAny<FileInfo>(), It.IsAny<FileInfo>()))
                .Throws(new InvalidOperationException("something went wrong"));

            var args = new[]
            {
                "excel-report",
                "--no-logo",
                "--input-model", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"),
                "--output-report", Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx")
            };

            var parseResult = this.rootCommand.Parse(args);

            var result = await this.handler.InvokeAsync(parseResult, this.cts.Token);

            Assert.That(result, Is.EqualTo(-1), "InvokeAsync should return -1 when report generation fails.");
        }

        [Test]
        public void Verify_that_when_cancellation_is_requested_an_exception_is_thrown()
        {
            this.cts.Cancel();

            var args = new[]
            {
                "excel-report",
                "--no-logo",
                "--input-model", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"),
                "--output-report", Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx")
            };

            var parseResult = this.rootCommand.Parse(args);

            Assert.That(async () => await this.handler.InvokeAsync(parseResult, this.cts.Token),
                Throws.InstanceOf<OperationCanceledException>());
        }
    }
}
