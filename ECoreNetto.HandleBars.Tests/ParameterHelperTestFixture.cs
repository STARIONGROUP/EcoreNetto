// ------------------------------------------------------------------------------------------------
// <copyright file="ParameterHelperTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars.Tests
{
    using System.IO;
    using System.Linq;

    using ECoreNetto;

    using HandlebarsDotNet;
    using HandlebarsDotNet.Helpers;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ParameterHelper"/> class
    /// </summary>
    [TestFixture]
    public class ParameterHelperTestFixture
    {
        private IHandlebars handlebarsContext = null!;

        private EPackage root = null!;

        [SetUp]
        public void Setup()
        {
            this.handlebarsContext = Handlebars.Create();
            HandlebarsHelpers.Register(this.handlebarsContext);

            ParameterHelper.RegisterParameterHelper(this.handlebarsContext);

            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "report-features.ecore");
            this.root = ModelLoader.Load(path);
        }

        private EOperation Operation(string className, string operationName)
        {
            return this.root.EClassifiers.OfType<EClass>().Single(x => x.Name == className)
                .EOperations.Single(x => x.Name == operationName);
        }

        [Test]
        public void Verify_that_WriteTypeAndName_renders_type_name_and_multiplicity()
        {
            var template = "{{ Parameter.WriteTypeAndName this }}";
            var action = this.handlebarsContext.Compile(template);

            var greeting = this.Operation("Person", "greet").EParameters.Single(x => x.Name == "greeting");

            var result = action(greeting);

            Assert.Multiple(() =>
            {
                Assert.That(result, Does.Contain("EString"));
                Assert.That(result, Does.Contain("greeting"));
                Assert.That(result, Does.Contain("[0..*]"));
            });
        }

        [Test]
        public void Verify_that_WriteTypeAndName_renders_void_for_a_typeless_parameter()
        {
            var template = "{{ Parameter.WriteTypeAndName this }}";
            var action = this.handlebarsContext.Compile(template);

            var target = this.Operation("Company", "clear").EParameters.Single(x => x.Name == "target");

            var result = action(target);

            Assert.Multiple(() =>
            {
                Assert.That(result, Does.Contain("void"));
                Assert.That(result, Does.Contain("target"));
            });
        }

        [Test]
        public void Verify_that_WriteReturnType_renders_type_name_and_multiplicity()
        {
            var template = "{{ TypedElement.WriteReturnType this }}";
            var action = this.handlebarsContext.Compile(template);

            var greet = this.Operation("Person", "greet");

            var result = action(greet);

            Assert.Multiple(() =>
            {
                Assert.That(result, Does.Contain("EString"));
                Assert.That(result, Does.Contain("[1..1]"));
            });
        }

        [Test]
        public void Verify_that_WriteReturnType_renders_void_when_there_is_no_return_type()
        {
            var template = "{{ TypedElement.WriteReturnType this }}";
            var action = this.handlebarsContext.Compile(template);

            var clear = this.Operation("Company", "clear");

            var result = action(clear);

            Assert.That(result, Is.EqualTo("void"));
        }

        [Test]
        public void Verify_that_WriteTypeAndName_throws_when_not_exactly_one_argument()
        {
            var template = "{{ Parameter.WriteTypeAndName this that }}";
            var action = this.handlebarsContext.Compile(template);

            var greeting = this.Operation("Person", "greet").EParameters.Single(x => x.Name == "greeting");

            Assert.Throws<HandlebarsException>(() => action(new { that = greeting }));
        }

        [Test]
        public void Verify_that_WriteReturnType_throws_when_not_exactly_one_argument()
        {
            var template = "{{ TypedElement.WriteReturnType this that }}";
            var action = this.handlebarsContext.Compile(template);

            var greet = this.Operation("Person", "greet");

            Assert.Throws<HandlebarsException>(() => action(new { that = greet }));
        }
    }
}
