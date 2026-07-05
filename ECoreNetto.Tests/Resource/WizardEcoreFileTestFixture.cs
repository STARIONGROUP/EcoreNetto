// ------------------------------------------------------------------------------------------------
// <copyright file="WizardEcoreFileTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tests.Resource
{
    using System;
    using System.IO;

    using ECoreNetto.Resource;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serilog;

    /// <summary>
    /// Suite of tests to verify that the wizardEcore.ecore file can be loaded
    /// </summary>
    [TestFixture]
    public class WizardEcoreFileTestFixture
    {
        private string filePath = null!;
        private Uri uri = null!;
        private Resource resource = null!;
        private ResourceSet resourceSet = null!;
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
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "wizardEcore.ecore");
            this.filePath = Path.GetFullPath(path);
            this.uri = new System.Uri(this.filePath);
        }

        [Test]
        public void Verify_that_the_ecore_file_can_be_loaded_as_a_resource()
        {
            this.resourceSet = new ResourceSet(this.loggerFactory);
            this.resource = this.resourceSet.CreateResource(this.uri);
            
            Assert.DoesNotThrow(() => this.resource.Load(null));
        }
    }
}
