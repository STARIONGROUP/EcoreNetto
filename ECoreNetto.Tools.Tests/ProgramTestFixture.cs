// ------------------------------------------------------------------------------------------------
// <copyright file="ProgramTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
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
        private string inputModel = null!;

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
