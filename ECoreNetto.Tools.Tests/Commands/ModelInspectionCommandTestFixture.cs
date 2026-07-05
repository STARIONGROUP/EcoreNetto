// ------------------------------------------------------------------------------------------------
// <copyright file="ModelInspectionCommandTestFixture.cs" company="Starion Group S.A.">
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
    public class ModelInspectionCommandTestFixture
    {
        private RootCommand rootCommand = null!;

        private Mock<IModelInspector> modelInspector = null!;

        private Mock<IVersionChecker> versionChecker = null!;

        private ModelInspectionCommand.Handler handler = null!;

        private CancellationTokenSource cts = null!;

        [SetUp]
        public void SetUp()
        {
            this.cts = new CancellationTokenSource();

            var modelInspectionCommand = new ModelInspectionCommand();
            this.rootCommand = new RootCommand();
            this.rootCommand.Add(modelInspectionCommand);

            this.modelInspector = new Mock<IModelInspector>();
            this.versionChecker = new Mock<IVersionChecker>();

            this.modelInspector.Setup(x => x.IsValidReportExtension(It.IsAny<FileInfo>()))
                .Returns(new Tuple<bool, string>(true, "valid extension"));

            this.handler = new ModelInspectionCommand.Handler(this.modelInspector.Object, this.versionChecker.Object);

            // disable the artificial status delay so the generation path runs without latency
            this.handler.StatusDelay = TimeSpan.Zero;
        }

        [Test]
        public void Verify_that_inspect_command_can_be_constructed()
        {
            Assert.That(() =>
            {
                var reportCommand = new ModelInspectionCommand();
            }, Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_InvokeAsync_returns_0()
        {
            var args = new[]
            {
                "inspect",
                "--no-logo",
                "--input-model", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"),
                "--output-report", Path.Combine(TestContext.CurrentContext.TestDirectory, "inspection-report.txt")
            };

            var parseResult = this.rootCommand.Parse(args);

            var result = await this.handler.InvokeAsync(parseResult, this.cts.Token);

            this.modelInspector.Verify(x => x.GenerateReport(It.IsAny<FileInfo>(), It.IsAny<FileInfo>()), Times.Once);

            this.versionChecker.Verify(x => x.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);

            Assert.That(result, Is.EqualTo(0), "InvokeAsync should return 0 upon success.");
        }

        [Test]
        public async Task Verify_that_when_the_input_ecore_model_does_not_exists_returns_not_0()
        {
            var args = new[]
            {
                "inspect",
                "--no-logo",
                "--input-model", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "non-existent.ecore"),
                "--output-report", Path.Combine(TestContext.CurrentContext.TestDirectory, "inspection-report.txt")
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
                "inspect",
                "--no-logo",
                "--input-model", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"),
            };

            this.modelInspector.Setup(x => x.IsValidReportExtension(It.IsAny<FileInfo>()))
                .Returns(new Tuple<bool, string>(false, "invalid extension"));

            var parseResult = this.rootCommand.Parse(args);

            var result = await this.handler.InvokeAsync(parseResult, this.cts.Token);

            Assert.That(result, Is.EqualTo(-1), "InvokeAsync should return -1 upon failure.");
        }
    }
}
