// -------------------------------------------------------------------------------------------------
// <copyright file="StringExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
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
        public void Verify_that_CapitalizeFirstLetter_returns_expected_result()
        {
            Assert.Throws<ArgumentException>(() => StringExtensions.CapitalizeFirstLetter(null));

            Assert.Throws<ArgumentException>(() => "".CapitalizeFirstLetter());
            
            Assert.That( "john Doe".CapitalizeFirstLetter(), Is.EqualTo("John Doe"));
        }

        [Test]
        public void Verify_that_LowerCaseFirstLetter_returns_expected_result()
        {
            Assert.Throws<ArgumentException>(() => StringExtensions.LowerCaseFirstLetter(null));

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
