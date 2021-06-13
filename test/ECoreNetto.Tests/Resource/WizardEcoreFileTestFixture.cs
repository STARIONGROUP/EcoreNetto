// -------------------------------------------------------------------------------------------------
// <copyright file="WizardEcoreFileTestFixture.cs" company="RHEA System S.A.">
//
//   Copyright 2017 RHEA System S.A.
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

using System;

namespace ECoreNetto.Tests.Resource
{
    using System.IO;
    using System.Linq;

    using ECoreNetto.Resource;
    using NUnit.Framework;
    
    /// <summary>
    /// Suite of tests to verify that the wizardEcore.ecore file can be loaded
    /// </summary>
    [TestFixture]
    public class WizardEcoreFileTestFixture
    {
        private string filePath;
        private Uri uri;
        private Resource resource;
        private ResourceSet resourceSet;

        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "wizardEcore.ecore");
            this.filePath = Path.GetFullPath(path);
            this.uri = new System.Uri(this.filePath);
        }

        [Test]
        public void Verify_that_the_ecore_file_can_be_loaded_as_a_resource()
        {
            this.resourceSet = new ResourceSet();
            this.resource = new Resource() { URI = uri, ResourceSet = this.resourceSet };
            this.resourceSet.Resources.Add(this.resource);

            Assert.DoesNotThrow(() => this.resource.Load(null));
        }
    }
}
