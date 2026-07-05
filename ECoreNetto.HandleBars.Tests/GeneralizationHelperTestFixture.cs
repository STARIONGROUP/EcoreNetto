// ------------------------------------------------------------------------------------------------
// <copyright file="GeneralizationHelperTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars.Tests
{
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using ECoreNetto;
    
    using HandlebarsDotNet;
    
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="GeneralizationHelper"/> class
    /// </summary>
    [TestFixture]
    public class GeneralizationHelperTestFixture
    {
        private IHandlebars handlebarsContenxt = null!;

        private EPackage root = null!;

        [SetUp]
        public void Setup()
        {
            this.handlebarsContenxt = Handlebars.Create();
            this.handlebarsContenxt.Configuration.FormatProvider = CultureInfo.InvariantCulture;

            GeneralizationHelper.RegisterGeneralizationHelper(this.handlebarsContenxt);
            
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "recipe.ecore");

            this.root = ModelLoader.Load(path);
        }
        
        [Test]
        public void Verify_that_GeneralizationInterfaces_returns_expected_results()
        {
            var template = "{{ #Generalization.Interfaces this }}";
            
            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.Single(x => x.Name == "ContainmentRelation");

            var result = action(eClass);

            Assert.That(result, Is.EqualTo(": IRelation, IAction"));
        }

        [Test]
        public void Verify_that_GeneralizationClasses_returns_expected_results()
        {
            var template = "{{ #Generalization.Classes this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.Single(x => x.Name == "ContainmentRelation");

            var result = action(eClass);

            Assert.That(result, Is.EqualTo(": Relation, IContainmentRelation"));
        }

        [Test]
        public void Verify_that_GeneralizationClasses_returns_interface_for_class_without_supertypes()
        {
            var template = "{{ #Generalization.Classes this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.Single(x => x.Name == "Relation");

            var result = action(eClass);

            Assert.That(result, Is.EqualTo(": IRelation"));
        }

        [Test]
        public void Verify_that_GeneralizationInterfaces_returns_empty_for_class_without_supertypes()
        {
            var template = "{{ #Generalization.Interfaces this }}";

            var action = this.handlebarsContenxt.Compile(template);

            var eClass = this.root.EClassifiers.Single(x => x.Name == "Relation");

            var result = action(eClass);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Verify_that_GeneralizationInterfaces_throws_when_context_is_not_an_EClass()
        {
            var template = "{{ #Generalization.Interfaces this }}";

            var action = this.handlebarsContenxt.Compile(template);

            Assert.That(() => action("not-an-eclass"), Throws.ArgumentException);
        }

        [Test]
        public void Verify_that_GeneralizationClasses_throws_when_context_is_not_an_EClass()
        {
            var template = "{{ #Generalization.Classes this }}";

            var action = this.handlebarsContenxt.Compile(template);

            Assert.That(() => action("not-an-eclass"), Throws.ArgumentException);
        }
    }
}
