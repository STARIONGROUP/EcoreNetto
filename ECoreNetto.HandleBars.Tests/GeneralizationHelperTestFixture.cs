// -------------------------------------------------------------------------------------------------
// <copyright file="GeneralizationHelperTestFixture.cs" company="Starion Group S.A.">
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

namespace ECoreNetto.HandleBars.Tests
{
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using ECoreNetto;
    
    using HandlebarsDotNet;
    
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="GeneralizationHelper"/> class
    /// </summary>
    [TestFixture]
    public class GeneralizationHelperTestFixture
    {
        private IHandlebars handlebarsContenxt;

        private EPackage root;

        [SetUp]
        public void Setup()
        {
            this.handlebarsContenxt = Handlebars.Create();
            this.handlebarsContenxt.Configuration.FormatProvider = CultureInfo.InvariantCulture;

            GeneralizationHelper.RegisterGeneralizationHelper(this.handlebarsContenxt);
            
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");

            this.root = ModelLoader.Load(path);
        }
        
        [Test]
        public void Verify_that_GeneralizationInterfaces_returns_expected_results()
        {
            var template = "{{ #Generalization.Interfaces this }}";
            
            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.Single(x => x.Name == "ContainmentRelation");

            var result = action(eClass);

            Assert.That(result, Is.EqualTo(": IRelation, IAction"));
        }

        [Test]
        public void Verify_that_GeneralizationClasses_returns_expected_results()
        {
            var template = "{{ #Generalization.Classes this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.Single(x => x.Name == "ContainmentRelation");

            var result = action(eClass);

            Assert.That(result, Is.EqualTo(": Relation, IContainmentRelation"));
        }
    }
}
