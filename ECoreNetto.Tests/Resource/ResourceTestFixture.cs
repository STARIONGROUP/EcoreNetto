// ------------------------------------------------------------------------------------------------
// <copyright file="ResourceTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tests.Resource
{
    using System.IO;
    using System.Linq;

    using ECoreNetto.Resource;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="Resource"/> class.
    /// </summary>
    [TestFixture]
    public class ResourceTestFixture
    {
        private string filePath = null!;
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
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "ecore.ecore");
            this.filePath = Path.GetFullPath(path);
            var uri = new System.Uri(this.filePath);

            this.resourceSet = new ResourceSet(this.loggerFactory);
            this.resource = new Resource(this.loggerFactory) { URI = uri, ResourceSet = this.resourceSet};
            this.resourceSet.Resources.Add(this.resource);
        }

        [Test]
        public void VerifyThatAResourceCanBeLoaded()
        {
            var root = this.resource.Load(null);

            Assert.That(root.EClassifiers.OfType<EClass>().Count(), Is.EqualTo(20));
        }

        [Test]
        public void Verify_that_when_urifragment_is_null_or_empty_exception_is_thrown()
        {
            Assert.That(() => this.resource.GetEObject(""), Throws.ArgumentException);

            Assert.That(() => this.resource.GetEObject(null!), Throws.ArgumentException);
        }

        [Test]
        public void Verify_that_can_be_initialized_with_null_logger()
        {
            var uri = new System.Uri(this.filePath);

            Assert.That(() => {
                this.resource = new Resource() { URI = uri, ResourceSet = this.resourceSet };

            }, Throws.Nothing);
        }

        [Test]
        public void Verify_that_resource_load_state_and_unload_work_as_expected()
        {
            this.resource.Load(null);

            Assert.That(this.resource.IsLoaded(), Is.True);
            Assert.That(this.resource.AllContents().Any(), Is.True);

            this.resource.UnLoad();

            Assert.That(this.resource.IsLoaded(), Is.False);
            Assert.That(this.resource.Contents, Is.Empty);
            Assert.That(this.resource.Errors, Is.Empty);
            Assert.That(this.resource.Warnings, Is.Empty);
        }
    }
}
