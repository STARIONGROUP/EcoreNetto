// ------------------------------------------------------------------------------------------------
// <copyright file="IEnumerableHelperTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars.Tests
{
    using System.Collections.Generic;

    using HandlebarsDotNet;
    using HandlebarsDotNet.Helpers;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="IEnumerableHelper"/> class
    /// </summary>
    [TestFixture]
    public class IEnumerableHelperTestFixture
    {
        private IHandlebars handlebarsContext = null!;

        [SetUp]
        public void Setup()
        {
            this.handlebarsContext = Handlebars.Create();
            HandlebarsHelpers.Register(this.handlebarsContext);

            IEnumerableHelper.RegisterIEnumerableHelper(this.handlebarsContext);
        }

        [Test]
        public void Verify_that_IsEmpty_returns_true_for_an_empty_enumerable()
        {
            var template = "{{#if (IEnumerable.IsEmpty items)}}EMPTY{{else}}NOT{{/if}}";
            var action = this.handlebarsContext.Compile(template);

            var result = action(new { items = new List<string>() });

            Assert.That(result, Is.EqualTo("EMPTY"));
        }

        [Test]
        public void Verify_that_IsEmpty_returns_false_for_a_non_empty_enumerable()
        {
            var template = "{{#if (IEnumerable.IsEmpty items)}}EMPTY{{else}}NOT{{/if}}";
            var action = this.handlebarsContext.Compile(template);

            var result = action(new { items = new List<string> { "one" } });

            Assert.That(result, Is.EqualTo("NOT"));
        }

        [Test]
        public void Verify_that_IsEmpty_returns_true_when_the_argument_is_not_an_enumerable()
        {
            var template = "{{#if (IEnumerable.IsEmpty item)}}EMPTY{{else}}NOT{{/if}}";
            var action = this.handlebarsContext.Compile(template);

            var result = action(new { item = 42 });

            Assert.That(result, Is.EqualTo("EMPTY"));
        }

        [Test]
        public void Verify_that_IsEmpty_throws_when_not_exactly_one_argument()
        {
            var template = "{{#if (IEnumerable.IsEmpty a b)}}EMPTY{{else}}NOT{{/if}}";
            var action = this.handlebarsContext.Compile(template);

            Assert.Throws<HandlebarsException>(() => action(new { a = new List<string>(), b = new List<string>() }));
        }
    }
}
