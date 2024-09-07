// -------------------------------------------------------------------------------------------------
// <copyright file="PackageExtensionsTestFixture.cs" company="Starion Group S.A">
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
        private EPackage rootPackage;

        private ILoggerFactory loggerFactory;

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
            EPackage package = null;

            var packages = PackageExtensions.QueryPackages(package);

            Assert.That(packages, Is.Empty);
        }


    }
}