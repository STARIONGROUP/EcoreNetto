// -------------------------------------------------------------------------------------------------
// <copyright file="BooleanHelperTestFixture.cs" company="Starion Group S.A">
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
    using System.Globalization;

    using HandlebarsDotNet;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="BooleanHelper"/> class
    /// </summary>
    [TestFixture]
    public class BooleanHelperTestFixture
    {
        private IHandlebars handlebarsContenxt;

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
    }
}
