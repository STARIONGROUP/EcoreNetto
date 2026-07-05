// ------------------------------------------------------------------------------------------------
// <copyright file="ResourceLouderTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2019-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tools.Tests.Resources
{
    using System;
    
    using NUnit.Framework;
    
    using ECoreNetto.Tools.Resources;

    /// <summary>
    /// Test fixture for the <see cref="ResourceLoader"/> class.
    /// </summary>
    [TestFixture]
    public class ResourceLoaderTestFixture
    {
        [Test]
        public void LoadEmbeddedResource_WithValidPath_ReturnsContent()
        {
            var resourcePath = "ECoreNetto.Tools.Resources.ascii-art.txt";

            var content = ResourceLoader.LoadEmbeddedResource(resourcePath);

            Assert.That(content, Is.Not.Null.And.Not.Empty);
        }
        
        [Test]
        public void LoadEmbeddedResource_WithNullPath_ThrowsArgumentNullException()
        {
            Assert.That(() => ResourceLoader.LoadEmbeddedResource(null!),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void QueryVersion_ReturnsNonNullVersion()
        {
            var version = ResourceLoader.QueryVersion();

            Assert.That(version, Is.Not.Null.And.Not.Empty);
            Assert.That(version, Does.Match(@"^\d+\.\d+\.\d+\.\d+$"));
        }

        [Test]
        public void QueryLogo_ReturnsLogoWithVersion()
        {
            var version = ResourceLoader.QueryVersion();

            var logo = ResourceLoader.QueryLogo();

            Assert.That(logo, Is.Not.Null.And.Not.Empty);
            Assert.That(logo, Does.Contain(version));
        }
    }
}
