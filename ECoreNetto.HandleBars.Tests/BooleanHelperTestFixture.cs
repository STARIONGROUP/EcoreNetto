// ------------------------------------------------------------------------------------------------
// <copyright file="BooleanHelperTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="BooleanHelper"/> class
    /// </summary>
    [TestFixture]
    public class BooleanHelperTestFixture
    {
        private IHandlebars handlebarsContenxt = null!;

        [SetUp]
        public void SetUp()
        {
            this.handlebarsContenxt = Handlebars.Create();
            this.handlebarsContenxt.Configuration.FormatProvider = CultureInfo.InvariantCulture;

            BooleanHelper.RegisterBooleanHelper(this.handlebarsContenxt);
        }

        [Test]
        public void Verify_that_ToLowerCase_returns_expected_result()
        {
            var template = "{{ #Boolean.ToLowerCase this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var trueResult = action(true);

            Assert.That(trueResult, Is.EqualTo("true"));

            var falseResult = action(false);

            Assert.That(falseResult, Is.EqualTo("false"));
        }

        [Test]
        public void Verify_that_ToLowerCase_throws_when_not_exactly_one_argument()
        {
            var template = "{{ #Boolean.ToLowerCase this that }}";

            var action = this.handlebarsContenxt.Compile(template);

            Assert.Throws<HandlebarsException>(() => action(true));
        }
    }
}
