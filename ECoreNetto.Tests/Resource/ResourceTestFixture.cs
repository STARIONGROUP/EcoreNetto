// -------------------------------------------------------------------------------------------------
// <copyright file="ResourceTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
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
        private string filePath;
        private Resource resource;
        private ResourceSet resourceSet;
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

            Assert.That(() => this.resource.GetEObject(null), Throws.ArgumentException);
        }

        [Test]
        public void Verify_that_can_be_initialized_with_null_logger()
        {
            var uri = new System.Uri(this.filePath);

            Assert.That(() => {
                this.resource = new Resource() { URI = uri, ResourceSet = this.resourceSet };

            }, Throws.Nothing);
        }
    }
}
