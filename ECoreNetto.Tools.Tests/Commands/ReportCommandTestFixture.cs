// -------------------------------------------------------------------------------------------------
// <copyright file="ReportCommandTestFixture.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Tools.Tests.Commands
{
    using System;
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Threading.Tasks;

    using ECoreNetto.Extensions;
    using ECoreNetto.Tools.Commands;
    
    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="XlReportCommand"/> class.
    /// </summary>
    [TestFixture]
    public class ReportCommandTestFixture
    {
        private Mock<IXlReportGenerator> reportGenerator;

        private XlReportCommand.Handler handler;

        [SetUp]
        public void SetUp()
        {
            this.reportGenerator = new Mock<IXlReportGenerator>();

            this.reportGenerator.Setup(x => x.IsValidExcelReportExtension(It.IsAny<FileInfo>()))
                .Returns(new Tuple<bool, string>(true, "valid extension"));

            this.handler = new XlReportCommand.Handler(
                this.reportGenerator.Object);
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
            var invocationContext = new InvocationContext(null!);

            this.handler.InputModel = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"));
            this.handler.OutputReport = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx"));

            var result = await this.handler.InvokeAsync(invocationContext);

            this.reportGenerator.Verify(x => x.GenerateTable(It.IsAny<FileInfo>(), It.IsAny<FileInfo>()), Times.Once);

            Assert.That(result, Is.EqualTo(0), "InvokeAsync should return 0 upon success.");
        }

        [Test]
        public async Task Verify_that_when_the_input_ecore_model_does_not_exists_returns_not_0()
        {
            var invocationContext = new InvocationContext(null!);

            this.handler.InputModel = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "non-existent.ecore"));
            this.handler.OutputReport = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx"));

            var result = await this.handler.InvokeAsync(invocationContext);

            Assert.That(result, Is.EqualTo(-1), "InvokeAsync should return -1 upon failure.");
        }

        [Test]
        public async Task Verify_that_when_the_output_extensions_is_not_supported_returns_not_0()
        {
            var invocationContext = new InvocationContext(null!);

            this.reportGenerator.Setup(x => x.IsValidExcelReportExtension(It.IsAny<FileInfo>()))
                .Returns(new Tuple<bool, string>(false, "invalid extension"));

            this.handler.InputModel = new FileInfo(Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore"));
            
            var result = await this.handler.InvokeAsync(invocationContext);

            Assert.That(result, Is.EqualTo(-1), "InvokeAsync should return -1 upon failure.");
        }
    }
}