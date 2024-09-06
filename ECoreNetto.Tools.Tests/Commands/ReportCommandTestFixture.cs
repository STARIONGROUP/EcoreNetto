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
    using System.CommandLine.Invocation;
    using System.IO;
    using System.Threading.Tasks;

    using ECoreNetto.Extensions;
    using ECoreNetto.Tools.Commands;
    
    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ReportCommand"/> class.
    /// </summary>
    [TestFixture]
    public class ReportCommandTestFixture
    {
        private Mock<IReportGenerator> reportGenerator;

        private ReportCommand.Handler handler;

        [SetUp]
        public void SetUp()
        {
            this.reportGenerator = new Mock<IReportGenerator>();

            this.handler = new ReportCommand.Handler(
                this.reportGenerator.Object);
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
    }
}