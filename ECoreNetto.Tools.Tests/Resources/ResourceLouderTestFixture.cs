// -------------------------------------------------------------------------------------------------
//  <copyright file="ResourceLoaderTestFixture.cs" company="Starion Group S.A.">
// 
//    Copyright 2019-2026 Starion Group S.A.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
//  </copyright>
//  ------------------------------------------------------------------------------------------------

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
