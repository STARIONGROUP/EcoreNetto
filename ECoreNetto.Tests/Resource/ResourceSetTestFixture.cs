// ------------------------------------------------------------------------------------------------
// <copyright file="ResourceSetTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tests.Resource
{
    using ECoreNetto.Resource;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serilog;
    using System;
    using System.IO;

    /// <summary>
    /// Suite of tests for the <see cref="ResourceSet"/> class.
    /// </summary>
    [TestFixture]
    public class ResourceSetTestFixture
    {
        /// <summary>
        /// the path to the file that is the resource
        /// </summary>
        private string filePath = null!;

        /// <summary>
        /// the class that is being tested
        /// </summary>
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
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "ecore.ecore");
            this.filePath = Path.GetFullPath(path);

            this.resourceSet = new ResourceSet(this.loggerFactory);
        }

        [Test]
        public void Verify_that_when_uri_is_null_exception_is_thrown()
        {
            Assert.That(() => this.resourceSet.CreateResource(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_can_be_initialized_with_null_logger()
        {
            var uri = new System.Uri(this.filePath);

            Assert.That(() => {
                this.resourceSet = new ResourceSet(null);

                resourceSet.CreateResource(uri);

            }, Throws.Nothing);
        }

        [Test]
        public void VerifyThatAResourceCanBeCreated()
        {
            var uri = new System.Uri(this.filePath);

            var resource = this.resourceSet.CreateResource(uri);

            Assert.That(this.resourceSet.Resources, Does.Contain(resource));

            Assert.That(uri, Is.EqualTo(resource.URI));
        }

        [Test]
        public void VerifyThatAResourceCanBeFoundByUri()
        {
            var uri = new System.Uri(this.filePath);

            var resource = new Resource(this.loggerFactory) { URI = uri };

            this.resourceSet.Resources.Add(resource);
            resource.ResourceSet = this.resourceSet;

            var result = this.resourceSet.Resource(uri, false);

            Assert.That(resource, Is.EqualTo(result));
        }

        [Test]
        public void VerifyThatNullIsReturnedWhenResourceCannotBeFound()
        {
            var uri = new System.Uri(this.filePath);

            var result = this.resourceSet.Resource(uri, false);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void VerifyThat_when_resource_is_called_with_null_uri_exception_is_thrown()
        {
            Assert.That(() => this.resourceSet.Resource(null!, false), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_with_loadOnDemand_true_a_missing_resource_is_created_and_loaded()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "ecore.ecore");
            var uri = new Uri(Path.GetFullPath(path));

            var result = this.resourceSet.Resource(uri, true);

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result!.IsLoaded(), Is.True);
                Assert.That(this.resourceSet.Resources, Does.Contain(result));
            });
        }

        [Test]
        public void Verify_that_with_loadOnDemand_true_and_a_nonexistent_file_returns_null()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "does-not-exist.ecore");
            var uri = new Uri(Path.GetFullPath(path));

            var result = this.resourceSet.Resource(uri, true);

            Assert.That(result, Is.Null);
            Assert.That(this.resourceSet.Resources, Is.Empty);
        }
    }
}
