// ------------------------------------------------------------------------------------------------
// <copyright file="ConcurrentLoadTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tests.Resource
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using ECoreNetto.Resource;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests that verify loading is free of shared mutable parsing state: two different models
    /// loaded concurrently must each resolve their own references, with no cross-contamination (see issue
    /// #31 — the former static <c>EObject.TopPackageName</c>).
    /// </summary>
    [TestFixture]
    public class ConcurrentLoadTestFixture
    {
        [Test]
        public void Verify_that_two_different_models_load_concurrently_and_resolve_their_own_references()
        {
            var results = new ConcurrentBag<(string expected, string actual, int errors)>();

            // alternate loading two different single-file models on many threads; each uses its own
            // ResourceSet and must resolve its own references without interference from the other
            Parallel.For(0, 100, i =>
            {
                var (fileName, expectedRoot) = (i % 2 == 0)
                    ? ("recipe.ecore", "recipe")
                    : ("ecore.ecore", "ecore");

                var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", fileName);

                var resourceSet = new ResourceSet();
                var resource = resourceSet.CreateResource(new Uri(Path.GetFullPath(path)));
                var root = resource.Load(null);

                results.Add((expectedRoot, root.Name, resource.Errors.Count()));
            });

            Assert.That(results, Has.Count.EqualTo(100));
            Assert.Multiple(() =>
            {
                Assert.That(
                    results.All(r => r.actual == r.expected),
                    Is.True,
                    "a concurrently loaded model resolved to the wrong root package");
                Assert.That(
                    results.All(r => r.errors == 0),
                    Is.True,
                    "a concurrently loaded model recorded reference-resolution errors");
            });
        }
    }
}
