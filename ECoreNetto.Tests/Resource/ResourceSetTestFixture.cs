﻿// -------------------------------------------------------------------------------------------------
// <copyright file="ResourceSetTestFixture.cs" company="Starion Group S.A.">
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

    using ECoreNetto.Resource;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Serilog;

    /// <summary>
    /// Suite of tests for the <see cref="ResourceSet"/> class.
    /// </summary>
    [TestFixture]
    public class ResourceSetTestFixture
    {
        /// <summary>
        /// the path to the file that is the resource
        /// </summary>
        private string filePath;

        /// <summary>
        /// the class that is being tested
        /// </summary>
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
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "ecore.ecore");
            this.filePath = Path.GetFullPath(path);

            this.resourceSet = new ResourceSet(this.loggerFactory);
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
    }
}
