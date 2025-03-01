// -------------------------------------------------------------------------------------------------
// <copyright file="ClassExtensionsTestFixture.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
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
    public class ClassExtensionsTestFixture
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
        public void Verify_that_QuerySpecializations_returns_the_expected_result()
        {
            var tool = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Tool");

            var specializations =
                ClassExtensions.QuerySpecializations(tool, this.rootPackage.EClassifiers.OfType<EClass>());

            var expected = new List<EClass>();
            
            expected.Add(this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Container"));
            expected.Add(this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Transformer"));
            
            Assert.That(specializations, Is.EquivalentTo(expected));
        }

        [Test]
        public void Verify_that_QuerySpecializations_throws_exception_when_argument_is_null()
        {
            EClass @class = null;

            Assert.That(() =>ClassExtensions.QuerySpecializations(@class, new List<EClass>()), Throws.ArgumentNullException);

        }

        [Test]
        public void Verify_that_QueryTypeHierarchy_returns_the_expected_result()
        {
            var standardAction = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "StandardAction");

            var expectedNames = new List<string> { "Action", "NamedElement" };

            var typeHierarchyNames = standardAction.QueryTypeHierarchy().Select(x => x.Name);

            Assert.That(typeHierarchyNames, Is.EquivalentTo(expectedNames));
        }

        [Test]
        public void Verify_that_QueryTypeHierarchy_throws_exception_when_argument_is_null()
        {
            EClass @class = null;

            Assert.That(() => ClassExtensions.QueryTypeHierarchy(@class), Throws.ArgumentNullException);

        }
    }
}
