// ------------------------------------------------------------------------------------------------
// <copyright file="RecipeEcoreFileTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tests.Resource
{
    using System;
    using System.IO;
    using System.Linq;

    using ECoreNetto.Resource;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    
    using Serilog;
    using ILogger = Serilog.ILogger;

    /// <summary>
    /// Suite of tests to verify that the recipe.ecore file can be loaded
    /// </summary>
    [TestFixture]
    public class RecipeEcoreFileTestFixture
    {
        private string filePath = null!;
        private Uri uri = null!;
        private Resource resource = null!;
        private ResourceSet resourceSet = null!;

        private ILoggerFactory loggerFactory = null!;
        
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
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");
            this.filePath = Path.GetFullPath(path);
            this.uri = new System.Uri(this.filePath);
        }

        [Test]
        public void Verify_that_the_ecore_file_can_be_loaded_as_a_resource()
        {
            this.resourceSet = new ResourceSet(this.loggerFactory);
            this.resource = this.resourceSet.CreateResource(this.uri);

            EPackage? rootPackage = null;

            Assert.DoesNotThrow(() => rootPackage = this.resource.Load(null));

            var eEnum = rootPackage!.EClassifiers.OfType<EEnum>().Single(x => x.Name == "Unit");

            var decagram = eEnum.ELiterals.Single(x => x.Name == "DECAGRAM");
            Assert.That(decagram.Value, Is.EqualTo(0));
            
            var teaspoon = eEnum.ELiterals.Single(x => x.Name == "TEASPOON");
            Assert.That(teaspoon.Value, Is.EqualTo(5));
        }
    }
}
