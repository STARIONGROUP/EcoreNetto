// -------------------------------------------------------------------------------------------------
// <copyright file="ModelElementExtensionsTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2023 RHEA System S.A.
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

namespace ECoreNetto.Extensions.Tests
{
    using System.IO;
    using System.Linq;

    using ECoreNetto.Extensions;
    using ECoreNetto.Resource;

    using NUnit.Framework;
    
    /// <summary>
    /// Suite of tests for the <see cref="ModelElementExtensions"/> class
    /// </summary>
    [TestFixture]
    public class ModelElementExtensionsTestFixture
    {
        private EPackage rootPackage;
        
        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");
            var filePath = Path.GetFullPath(path);
            var uri = new System.Uri(filePath);

            var resourceSet = new ResourceSet();
            var resource = resourceSet.CreateResource(uri);

            this.rootPackage = resource.Load(null);
        }

        [Test]
        public void Verify_that_QueryDocumentation_returns_the_expected_result()
        {
            var documentedClass = this.rootPackage.EClassifiers.Single(x => x.Name == "Ingredient");

            var documentation = documentedClass.QueryDocumentation();

            Assert.That(documentation.First(), Is.EqualTo("Any of the foods or substances that are combined to make a particular dish."));
            
            var undocumentedClass = this.rootPackage.EClassifiers.Single(x => x.Name == "Recipe");

            documentation = undocumentedClass.QueryDocumentation();

            Assert.That(documentation, Is.Empty);
        }
    }
}
