// ------------------------------------------------------------------------------------------------
// <copyright file="StringHelperTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars.Tests
{
    using System.Globalization;
    
    using HandlebarsDotNet;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="StringHelper"/> class
    /// </summary>
    [TestFixture]
    public class StringHelperTestFixture
    {
        private IHandlebars handlebarsContenxt = null!;

        [SetUp]
        public void SetUp()
        {
            this.handlebarsContenxt = Handlebars.Create();
            this.handlebarsContenxt.Configuration.FormatProvider = CultureInfo.InvariantCulture;

            StringHelper.RegisterStringHelper(this.handlebarsContenxt);
        }

        [Test]
        public void Verify_that_CapitalizeFirstLetter_returns_expected_result()
        {
            var template = "{{ #String.CapitalizeFirstLetter this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var result = action("uppercase");

            Assert.That(result,  Is.EqualTo("Uppercase"));
        }

        [Test]
        public void Verify_that_CapitalizeFirstLetter_throws_exception_when_not_exactly_one_parameter_is_provided()
        {
            var template = "{{ #String.CapitalizeFirstLetter this that }}";

            var action = this.handlebarsContenxt.Compile(template);

            Assert.Throws<HandlebarsException>(() => action("uppercase"));
        }

        [Test]
        public void Verify_that_LowerCaseFirstLetter_returns_expected_result()
        {
            var template = "{{ #String.LowerCaseFirstLetter this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var result = action("Lowercase");

            Assert.That(result, Is.EqualTo("lowercase"));
        }

        [Test]
        public void Verify_that_LowerCaseFirstLetter_throws_exception_when_not_exactly_one_parameter_is_provided()
        {
            var template = "{{ #String.LowerCaseFirstLetter this that }}";

            var action = this.handlebarsContenxt.Compile(template);

            Assert.Throws<HandlebarsException>(() => action("uppercase"));
        }

        [Test]
        public void Verify_that_CapitalizeFirstLetter_throws_exception_when_argument_is_not_a_string()
        {
            var template = "{{ #String.CapitalizeFirstLetter this }}";

            var action = this.handlebarsContenxt.Compile(template);

            Assert.Throws<HandlebarsException>(() => action(42));
        }

        [Test]
        public void Verify_that_LowerCaseFirstLetter_throws_exception_when_argument_is_not_a_string()
        {
            var template = "{{ #String.LowerCaseFirstLetter this }}";

            var action = this.handlebarsContenxt.Compile(template);

            Assert.Throws<HandlebarsException>(() => action(42));
        }
    }
}
