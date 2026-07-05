// ------------------------------------------------------------------------------------------------
// <copyright file="AnchorExtensionsTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions.Tests
{
    using System.IO;
    using System.Linq;

    using ECoreNetto.Extensions;
    using ECoreNetto.Resource;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="AnchorExtensions"/> class
    /// </summary>
    [TestFixture]
    public class AnchorExtensionsTestFixture
    {
        private EPackage rootPackage = null!;

        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");
            var uri = new System.Uri(Path.GetFullPath(path));

            var resourceSet = new ResourceSet();
            var resource = resourceSet.CreateResource(uri);

            this.rootPackage = resource.Load(null);
        }

        [Test]
        public void Verify_that_QueryAnchorId_returns_a_sanitized_slug()
        {
            var recipe = this.rootPackage.EClassifiers.OfType<EClass>().Single(x => x.Name == "Recipe");

            var anchor = recipe.QueryAnchorId();

            Assert.Multiple(() =>
            {
                // the slug contains only URL-safe characters and ends with the class name
                Assert.That(anchor, Does.Match("^[A-Za-z0-9-]+$"));
                Assert.That(anchor, Does.EndWith("Recipe"));
            });
        }

        [Test]
        public void Verify_that_QueryAnchorId_throws_when_argument_is_null()
        {
            EObject? eObject = null;

            Assert.That(() => eObject!.QueryAnchorId(), Throws.ArgumentNullException);
        }
    }
}
