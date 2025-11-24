// -------------------------------------------------------------------------------------------------
// <copyright file="VersionCheckerTestFixture.cs" company="Starion Group S.A">
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

namespace ECoreNetto.Tools.Tests.Services
{
    using ECoreNetto.Tools.Services;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Serilog;
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture]
    public class VersionCheckerTestFixture
    {
        private VersionChecker versionChecker;

        private ILoggerFactory? loggerFactory;

        private TestHttpClientFactory httpClientFactory;

        private TestTimeOutHttpClientFactory timeOutHttpClientFactory;

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

            this.httpClientFactory = new TestHttpClientFactory();
            this.timeOutHttpClientFactory = new TestTimeOutHttpClientFactory();
        }

        [SetUp]
        public void SetUp()
        {
            this.versionChecker = new VersionChecker(this.httpClientFactory, this.loggerFactory);
        }

        [Test]
        public async Task Verify_that_ExecuteAsync_does_not_throw()
        {
            var cts = new CancellationTokenSource();

            await Assert.ThatAsync(() => this.versionChecker.ExecuteAsync(cts.Token), Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_ExecuteAsync_does_not_throw_on_http_timeout()
        {
            var cts = new CancellationTokenSource();

            var checker = new VersionChecker(this.timeOutHttpClientFactory, this.loggerFactory);

            await Assert.ThatAsync(() => checker.ExecuteAsync(cts.Token), Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_when_cancelled_exception_is_thrown()
        {
            var cts = new CancellationTokenSource();

            await cts.CancelAsync();

            var checker = new VersionChecker(this.timeOutHttpClientFactory, this.loggerFactory);

            await Assert.ThatAsync(() => checker.ExecuteAsync(cts.Token), Throws.TypeOf<OperationCanceledException>());
        }

        /// <summary>
        /// Very simple IHttpClientFactory used just for tests.
        /// It always returns the HttpClient passed in the constructor.
        /// </summary>
        private sealed class TestHttpClientFactory : IHttpClientFactory
        {
            private readonly HttpClient client;

            public TestHttpClientFactory()
            {
                this.client = new HttpClient(new SuccessHandler());
            }

            public HttpClient CreateClient(string name)
            {
                return this.client;
            }
        }

        private class SuccessHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var json = "{\"tag_name\":\"1.2.3\",\"body\":\"notes\",\"html_url\":\"https://example.com\"}";
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                });
            }
        }

        /// <summary>
        /// Very simple IHttpClientFactory used just for tests.
        /// It always returns a HttpClient.
        /// </summary>
        private sealed class TestTimeOutHttpClientFactory : IHttpClientFactory
        {
            private readonly HttpClient client;
            public TestTimeOutHttpClientFactory()
            {
                this.client = new HttpClient(new TimeoutHandler()) { Timeout = TimeSpan.FromSeconds(1) };
            }

            public HttpClient CreateClient(string name)
            {
                return this.client;
            }
        }

        /// <summary>
        /// Very simple IHttpClientFactory used just for tests.
        /// It always returns a HttpClient that times out
        /// </summary>
        private class TimeoutHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw new TaskCanceledException();
            }
        }
    }
}
