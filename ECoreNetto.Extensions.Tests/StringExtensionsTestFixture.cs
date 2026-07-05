// ------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions.Tests
{
    using System;
    using System.Collections.Generic;
    
    using ECoreNetto.Extensions;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="StringExtensions"/> class
    /// </summary>
    [TestFixture]
    public class StringExtensionsTestFixture
    {
        [Test]
        public void Verify_that_SplitToLines_returns_expected_results()
        {
            var input = "ab c def";

            var lines =  input.SplitToLines(1);

            Assert.That(lines, Is.EquivalentTo(new List<string> {"ab", "c", "def"}));
        }

        [Test]
        public void Verify_that_SplitToLines_throws_for_null_or_empty_input()
        {
            // validation is eager: the exception is thrown on the call, not on enumeration
            Assert.Throws<ArgumentException>(() => StringExtensions.SplitToLines(null!, 10));

            Assert.Throws<ArgumentException>(() => "".SplitToLines(10));
        }

        [Test]
        public void Verify_that_CapitalizeFirstLetter_returns_expected_result()
        {
            Assert.Throws<ArgumentException>(() => StringExtensions.CapitalizeFirstLetter(null!));

            Assert.Throws<ArgumentException>(() => "".CapitalizeFirstLetter());
            
            Assert.That( "john Doe".CapitalizeFirstLetter(), Is.EqualTo("John Doe"));
        }

        [Test]
        public void Verify_that_LowerCaseFirstLetter_returns_expected_result()
        {
            Assert.Throws<ArgumentException>(() => StringExtensions.LowerCaseFirstLetter(null!));

            Assert.Throws<ArgumentException>(() => "".LowerCaseFirstLetter());

            Assert.That("John Doe".LowerCaseFirstLetter(), Is.EqualTo("john Doe"));
        }

        [Test]
        public void Verify_that_Prefix_returns_expected_result()
        {
            var input = "gram";
            var prefix = "kilo";

            Assert.That(input.Prefix(prefix), Is.EqualTo("kilogram"));
        }
    }
}
