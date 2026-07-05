// ------------------------------------------------------------------------------------------------
// <copyright file="PackageExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions.Tests
{
    using System.IO;
    using System.Linq;

    using ECoreNetto.Extensions;
    using ECoreNetto.Resource;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="ModelElementExtensions"/> class
    /// </summary>
    [TestFixture]
    public class PackageExtensionsTestFixture
    {
        private EPackage rootPackage = null!;

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
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");
            var filePath = Path.GetFullPath(path);
            var uri = new System.Uri(filePath);

            var resourceSet = new ResourceSet(this.loggerFactory);
            var resource = resourceSet.CreateResource(uri);

            this.rootPackage = resource.Load(null);
        }

        [Test]
        public void Verify_that_all_packages_are_returned()
        {
            var subPackage = new EPackage(this.rootPackage.EResource, this.loggerFactory);
            this.rootPackage.ESubPackages.Add(subPackage);

            var packages = this.rootPackage.QueryPackages();

            Assert.That(packages.Count, Is.EqualTo(2));
        }

        [Test]
        public void Verify_that_when_root_is_null_result_is_empty()
        {
            EPackage? package = null;

            var packages = PackageExtensions.QueryPackages(package!);

            Assert.That(packages, Is.Empty);
        }


    }
}