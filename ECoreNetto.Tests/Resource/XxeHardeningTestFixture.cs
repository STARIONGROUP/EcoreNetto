// -------------------------------------------------------------------------------------------------
// <copyright file="XxeHardeningTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
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

namespace ECoreNetto.Tests.Resource
{
    using System.IO;
    using System.Xml;

    using ECoreNetto.Resource;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests that verify the <see cref="ECoreNetto.ECoreParser"/> is hardened against
    /// XML External Entity (XXE) attacks (see issue #29).
    /// </summary>
    [TestFixture]
    public class XxeHardeningTestFixture
    {
        private ResourceSet resourceSet = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
        }

        [Test]
        public void Verify_that_a_document_with_an_external_entity_is_rejected()
        {
            // a DOCTYPE declaring an external SYSTEM entity - the classic XXE payload
            const string xxe =
                "<?xml version=\"1.0\"?>\r\n" +
                "<!DOCTYPE EPackage [ <!ENTITY xxe SYSTEM \"file:///etc/passwd\"> ]>\r\n" +
                "<EPackage>&xxe;</EPackage>";

            var resource = this.CreateResourceForContent("xxe-attack.ecore", xxe);

            // DtdProcessing.Prohibit rejects the DOCTYPE before the entity is ever resolved
            Assert.That(() => resource.Load(null), Throws.InstanceOf<XmlException>());
        }

        [Test]
        public void Verify_that_an_entity_expansion_document_is_rejected()
        {
            // a "billion laughs" style nested-entity document; also blocked by prohibiting DTDs
            const string billionLaughs =
                "<?xml version=\"1.0\"?>\r\n" +
                "<!DOCTYPE lolz [\r\n" +
                "  <!ENTITY lol \"lol\">\r\n" +
                "  <!ENTITY lol2 \"&lol;&lol;&lol;&lol;&lol;\">\r\n" +
                "  <!ENTITY lol3 \"&lol2;&lol2;&lol2;&lol2;&lol2;\">\r\n" +
                "]>\r\n" +
                "<EPackage>&lol3;</EPackage>";

            var resource = this.CreateResourceForContent("billion-laughs.ecore", billionLaughs);

            Assert.That(() => resource.Load(null), Throws.InstanceOf<XmlException>());
        }

        /// <summary>
        /// Writes the provided <paramref name="content"/> to a file in the test directory and
        /// creates a <see cref="Resource"/> for it.
        /// </summary>
        private Resource CreateResourceForContent(string fileName, string content)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);
            File.WriteAllText(path, content);

            var uri = new System.Uri(Path.GetFullPath(path));

            return this.resourceSet.CreateResource(uri);
        }
    }
}
