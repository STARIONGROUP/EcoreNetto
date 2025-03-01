// -------------------------------------------------------------------------------------------------
// <copyright file="VersionCheckerTestFixture.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Tools.Tests.Services
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using ECoreNetto.Tools.Services;

    using Microsoft.Extensions.Logging;

    using NUnit.Framework;

    using Serilog;

    [TestFixture]
    public class VersionCheckerTestFixture
    {
        private VersionChecker versionChecker;

        private ILoggerFactory? loggerFactory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();

            this.loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSerilog();
            });
        }

        [SetUp]
        public void SetUp()
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            this.versionChecker = new VersionChecker(httpClient, this.loggerFactory);
        }

        [Test]
        public async Task Verify_that_Query_version_returns_result()
        {
            var result = await this.versionChecker.QueryLatestRelease();

            Assert.That(result, Is.Not.Null);

            Log.Logger.Information(result.TagName);
            Log.Logger.Information(result.Body);
            Log.Logger.Information(result.HtmlUrl);

        }
    }
}
