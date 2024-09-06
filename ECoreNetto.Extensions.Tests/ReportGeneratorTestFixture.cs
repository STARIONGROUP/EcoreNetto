// -------------------------------------------------------------------------------------------------
// <copyright file="ReportGeneratorTestFixture.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Extensions.Tests
{
    using System.IO;
    
    using ECoreNetto.Extensions;
    
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ReportGeneratorTestFixture"/> class
    /// </summary>
    [TestFixture]
    public class ReportGeneratorTestFixture
    {
        private FileInfo modelFileInfo;

        private FileInfo reportFileInfo;

        private ReportGenerator reportGenerator;

        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");
            
            var modelPath = Path.GetFullPath(path);
            this.modelFileInfo = new FileInfo(modelPath);

            var outputPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "tabular-report.xlsx");
            this.reportFileInfo = new FileInfo(outputPath);

            this.reportGenerator = new ReportGenerator();
        }

        [Test]
        public void Verify_that_the_report_generator_generates_a_report()
        {
            Assert.That(() => this.reportGenerator.GenerateTable(modelFileInfo, reportFileInfo), Throws.Nothing);
        }
    }
}