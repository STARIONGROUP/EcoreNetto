// -------------------------------------------------------------------------------------------------
// <copyright file="ModelInspectorTestFixture.cs" company="Starion Group S.A.">
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
    using System;
    using System.IO;
    using ECoreNetto.Extensions;
    using ECoreNetto.Resource;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serilog;

    [TestFixture]
    public class ModelInspectorTestFixture
    {
        private EPackage rootPackage;

        private ModelInspector modelInspector;

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

            this.modelInspector = new ModelInspector(this.loggerFactory);
        }

        [Test]
        public void Verify_that_inspects_non_recursive_executes_as_expected()
        {
            var report = this.modelInspector.Inspect(this.rootPackage, false);

            Assert.That(report, Is.Not.Empty);

            Console.Write(report);
        }

        [Test]
        public void Verify_that_inspects_recursive_executes_as_expected()
        {
            var report = this.modelInspector.Inspect(this.rootPackage, true);

            Assert.That(report, Is.Not.Empty);

            Console.Write(report);
        }

        [Test]
        public void Verify_that_inspect_class_executes_as_expected()
        {
            var report = this.modelInspector.Inspect(this.rootPackage, "Container");

            Assert.That(report, Is.Not.Empty);

            Console.Write(report);
        }

        [Test]
        public void Verify_that_analyze_docs_non_recursive_executes_as_expected()
        {
            var report = this.modelInspector.AnalyzeDocumentation(this.rootPackage, false);

            Assert.That(report, Is.Not.Empty);

            Console.Write(report);
        }

        [Test]
        public void Verify_that_analyze_docs_recursive_executes_as_expected()
        {
            var report = this.modelInspector.AnalyzeDocumentation(this.rootPackage, true);

            Assert.That(report, Is.Not.Empty);

            Console.Write(report);
        }
    }
}
