// -------------------------------------------------------------------------------------------------
// <copyright file="DocumentationHelperTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
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
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using ECoreNetto;

    using HandlebarsDotNet;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="DocumentationHelper"/> class
    /// </summary>
    [TestFixture]
    public class DocumentationHelperTestFixture
    {
        private IHandlebars handlebarsContenxt = null!;

        private EPackage root = null!;

        [SetUp]
        public void SetUp()
        {
            this.handlebarsContenxt = Handlebars.Create();
            this.handlebarsContenxt.Configuration.FormatProvider = CultureInfo.InvariantCulture;

            DocumentationHelper.RegisteredDocumentationHelper(this.handlebarsContenxt);

            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");

            this.root = ModelLoader.Load(path);
        }

        [Test]
        public void Verify_that_RawDocumentation_returns_the_documentation_when_present()
        {
            var template = "{{ #RawDocumentation this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Ingredient");

            var result = action(eClass);

            Assert.That(result, Does.Contain("Any of the foods or substances"));
        }

        [Test]
        public void Verify_that_RawDocumentation_returns_placeholder_when_no_documentation_is_present()
        {
            var template = "{{ #RawDocumentation this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");

            var result = action(eClass);

            Assert.That(result, Is.EqualTo("No Documentation Provided"));
        }

        [Test]
        public void Verify_that_RawDocumentation_throws_when_context_is_not_an_EModelElement()
        {
            var template = "{{ #RawDocumentation this }}";

            var action = this.handlebarsContenxt.Compile(template);

            Assert.Throws<ArgumentException>(() => action("not-an-emodelelement"));
        }
    }
}
