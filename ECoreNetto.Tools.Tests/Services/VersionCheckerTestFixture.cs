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
        private VersionChecker versionChecker = null!;

        private ILoggerFactory? loggerFactory;

        private TestHttpClientFactory httpClientFactory = null!;

        private TestTimeOutHttpClientFactory timeOutHttpClientFactory = null!;

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

        [Test]
        public void Verify_that_default_url_and_timeout_are_used_when_not_configured()
        {
            Assert.Multiple(() =>
            {
                Assert.That(this.versionChecker.ReleasesUrl, Is.EqualTo(VersionChecker.DefaultReleasesUrl));
                Assert.That(this.versionChecker.Timeout, Is.EqualTo(TimeSpan.FromSeconds(2)));
            });
        }

        [Test]
        public void Verify_that_url_and_timeout_are_configurable()
        {
            var checker = new VersionChecker(this.httpClientFactory, this.loggerFactory,
                "https://example.test/releases/latest", TimeSpan.FromSeconds(5));

            Assert.Multiple(() =>
            {
                Assert.That(checker.ReleasesUrl, Is.EqualTo("https://example.test/releases/latest"));
                Assert.That(checker.Timeout, Is.EqualTo(TimeSpan.FromSeconds(5)));
            });
        }

        [Test]
        public async Task Verify_that_QueryLatestReleaseAsync_uses_the_configured_url()
        {
            var recordingHandler = new RecordingHandler();
            var factory = new StubHttpClientFactory(new HttpClient(recordingHandler));

            var checker = new VersionChecker(factory, this.loggerFactory, "https://example.test/releases/latest");

            await checker.QueryLatestReleaseAsync(new CancellationTokenSource().Token);

            Assert.That(recordingHandler.LastRequestUri?.ToString(), Is.EqualTo("https://example.test/releases/latest"));
        }

        [Test]
        public async Task Verify_that_ExecuteAsync_handles_a_malformed_tag_name_gracefully()
        {
            var factory = new StubHttpClientFactory(new HttpClient(new TagNameHandler("not-a-version")));

            var checker = new VersionChecker(factory, this.loggerFactory);

            await Assert.ThatAsync(() => checker.ExecuteAsync(new CancellationTokenSource().Token), Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_ExecuteAsync_handles_an_empty_tag_name()
        {
            var factory = new StubHttpClientFactory(new HttpClient(new TagNameHandler("")));

            var checker = new VersionChecker(factory, this.loggerFactory);

            await Assert.ThatAsync(() => checker.ExecuteAsync(new CancellationTokenSource().Token), Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_ExecuteAsync_handles_a_v_prefixed_tag_name()
        {
            var factory = new StubHttpClientFactory(new HttpClient(new TagNameHandler("v9.9.9")));

            var checker = new VersionChecker(factory, this.loggerFactory);

            await Assert.ThatAsync(() => checker.ExecuteAsync(new CancellationTokenSource().Token), Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_QueryLatestReleaseAsync_returns_a_populated_release_on_success()
        {
            var factory = new StubHttpClientFactory(new HttpClient(new SuccessHandler()));

            var checker = new VersionChecker(factory, this.loggerFactory);

            var release = await checker.QueryLatestReleaseAsync(new CancellationTokenSource().Token);

            Assert.That(release, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(release!.TagName, Is.EqualTo("1.2.3"));
                Assert.That(release.Body, Is.EqualTo("notes"));
                Assert.That(release.HtmlUrl, Is.EqualTo("https://example.com"));
            });
        }

        [Test]
        public async Task Verify_that_QueryLatestReleaseAsync_returns_null_on_a_non_success_status()
        {
            var factory = new StubHttpClientFactory(new HttpClient(new NotFoundHandler()));

            var checker = new VersionChecker(factory, this.loggerFactory);

            var release = await checker.QueryLatestReleaseAsync(new CancellationTokenSource().Token);

            Assert.That(release, Is.Null);
        }

        [Test]
        public async Task Verify_that_ExecuteAsync_does_not_throw_on_a_non_success_status()
        {
            var factory = new StubHttpClientFactory(new HttpClient(new NotFoundHandler()));

            var checker = new VersionChecker(factory, this.loggerFactory);

            await Assert.ThatAsync(() => checker.ExecuteAsync(new CancellationTokenSource().Token), Throws.Nothing);
        }

        [Test]
        public async Task Verify_that_QueryLatestReleaseAsync_returns_null_on_malformed_json()
        {
            var factory = new StubHttpClientFactory(new HttpClient(new MalformedJsonHandler()));

            var checker = new VersionChecker(factory, this.loggerFactory);

            var release = await checker.QueryLatestReleaseAsync(new CancellationTokenSource().Token);

            Assert.That(release, Is.Null);
        }

        [Test]
        public async Task Verify_that_ExecuteAsync_does_not_throw_on_malformed_json()
        {
            var factory = new StubHttpClientFactory(new HttpClient(new MalformedJsonHandler()));

            var checker = new VersionChecker(factory, this.loggerFactory);

            await Assert.ThatAsync(() => checker.ExecuteAsync(new CancellationTokenSource().Token), Throws.Nothing);
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

        /// <summary>
        /// An <see cref="IHttpClientFactory"/> that always returns the provided <see cref="HttpClient"/>.
        /// </summary>
        private sealed class StubHttpClientFactory : IHttpClientFactory
        {
            private readonly HttpClient client;

            public StubHttpClientFactory(HttpClient client)
            {
                this.client = client;
            }

            public HttpClient CreateClient(string name)
            {
                return this.client;
            }
        }

        /// <summary>
        /// A handler that records the requested <see cref="Uri"/> and returns a successful release payload.
        /// </summary>
        private sealed class RecordingHandler : HttpMessageHandler
        {
            public Uri? LastRequestUri { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                this.LastRequestUri = request.RequestUri;

                const string json = "{\"tag_name\":\"1.2.3\",\"body\":\"notes\",\"html_url\":\"https://example.com\"}";

                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                });
            }
        }

        /// <summary>
        /// A handler that always responds with a non-success (404) status.
        /// </summary>
        private sealed class NotFoundHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
            }
        }

        /// <summary>
        /// A handler that responds with a 200 status but a body that is not valid JSON.
        /// </summary>
        private sealed class MalformedJsonHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("{ this is not valid json ")
                });
            }
        }

        /// <summary>
        /// A handler that returns a release payload with a configurable tag name.
        /// </summary>
        private sealed class TagNameHandler : HttpMessageHandler
        {
            private readonly string tagName;

            public TagNameHandler(string tagName)
            {
                this.tagName = tagName;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var json = $"{{\"tag_name\":\"{this.tagName}\",\"body\":\"notes\",\"html_url\":\"https://example.com\"}}";

                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                });
            }
        }
    }
}
