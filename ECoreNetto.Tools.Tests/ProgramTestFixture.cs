// -------------------------------------------------------------------------------------------------
// <copyright file="ProgramTestFixture.cs" company="Starion Group S.A">
// 
//   Copyright 2017-2025 Starion Group S.A.
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

namespace ECoreNetto.Tools.Tests
{
    using System.IO;
    using System.Threading.Tasks;
    using NUnit.Framework;

    using ECoreNetto.Tools;

    [TestFixture]
    public class ProgramTestFixture
    {
        private string inputModel;

        [SetUp]
        public void Setup()
        {
            this.inputModel = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");
        }

        [Test]
        [TestCase("html-report")]
        [TestCase("excel-report")]
        [TestCase("inspect")]
        public async Task Verify_that_console_app_can_generate_reports(string reportKind)
        {
            var args = new[]
            {
                reportKind,
                "--no-logo",
                "--log-level", "Warning",
                "--input-model", this.inputModel,
            };

            var exitCode = await Program.Main(args);

            Assert.That(exitCode, Is.EqualTo(0));
        }
    }
}
