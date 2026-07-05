// ------------------------------------------------------------------------------------------------
// <copyright file="ClassHelperTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.HandleBars.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ECoreNetto;
    using ECoreNetto.Extensions;

    using HandlebarsDotNet;
    using HandlebarsDotNet.Helpers;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ClassHelper"/> class
    /// </summary>
    [TestFixture]
    public class ClassHelperTestFixture
    {
        private IHandlebars handlebarsContext = null!;

        private EPackage root = null!;

        private List<EClass> allClasses = null!;

        [SetUp]
        public void Setup()
        {
            this.handlebarsContext = Handlebars.Create();
            HandlebarsHelpers.Register(this.handlebarsContext);

            ClassHelper.RegisterClassHelper(this.handlebarsContext);

            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "report-features.ecore");
            this.root = ModelLoader.Load(path);
            this.allClasses = this.root.EClassifiers.OfType<EClass>().ToList();
        }

        private EClass Class(string name)
        {
            return this.allClasses.Single(x => x.Name == name);
        }

        [Test]
        public void Verify_that_QuerySpecializations_returns_the_subclasses()
        {
            var template = "{{#each (Class.QuerySpecializations subject all) as | c |}}{{c.Name}};{{/each}}";
            var action = this.handlebarsContext.Compile(template);

            var result = action(new { subject = this.Class("Describable"), all = this.allClasses });

            Assert.That(result, Is.EqualTo("Person;"));
        }

        [Test]
        public void Verify_that_QueryContainers_returns_the_containing_classes()
        {
            var template = "{{#each (Class.QueryContainers subject all) as | c |}}{{c.Name}};{{/each}}";
            var action = this.handlebarsContext.Compile(template);

            var result = action(new { subject = this.Class("Address"), all = this.allClasses });

            Assert.That(result, Is.EqualTo("Person;"));
        }

        [Test]
        public void Verify_that_QueryConstraints_returns_the_constraints()
        {
            var template = "{{#each (Class.QueryConstraints subject) as | c |}}{{c.Name}}:{{c.Language}};{{/each}}";
            var action = this.handlebarsContext.Compile(template);

            var result = action(new { subject = this.Class("Person") });

            Assert.That(result, Is.EqualTo("hasFullName:OCL;"));
        }

        [Test]
        public void Verify_that_RenderInheritanceDiagram_renders_the_diagram_when_present()
        {
            var template = "{{#Class.RenderInheritanceDiagram subject diagrams}}{{{this}}}{{/Class.RenderInheritanceDiagram}}";
            var action = this.handlebarsContext.Compile(template);

            var person = this.Class("Person");
            var diagrams = new Dictionary<string, string> { { person.QueryAnchorId(), "<svg>DIAGRAM</svg>" } };

            var result = action(new { subject = person, diagrams });

            Assert.That(result, Does.Contain("DIAGRAM"));
        }

        [Test]
        public void Verify_that_RenderInheritanceDiagram_renders_nothing_when_absent()
        {
            var template = "{{#Class.RenderInheritanceDiagram subject diagrams}}{{{this}}}{{/Class.RenderInheritanceDiagram}}";
            var action = this.handlebarsContext.Compile(template);

            var diagrams = new Dictionary<string, string>();

            var result = action(new { subject = this.Class("Person"), diagrams });

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Verify_that_RenderAssociationDiagram_renders_the_diagram_when_present()
        {
            var template = "{{#Class.RenderAssociationDiagram subject diagrams}}{{{this}}}{{/Class.RenderAssociationDiagram}}";
            var action = this.handlebarsContext.Compile(template);

            var person = this.Class("Person");
            var diagrams = new Dictionary<string, string> { { person.QueryAnchorId(), "<svg>ASSOC</svg>" } };

            var result = action(new { subject = person, diagrams });

            Assert.That(result, Does.Contain("ASSOC"));
        }

        [Test]
        public void Verify_that_QuerySpecializations_throws_when_not_exactly_two_arguments()
        {
            var template = "{{#each (Class.QuerySpecializations subject) as | c |}}{{c.Name}}{{/each}}";
            var action = this.handlebarsContext.Compile(template);

            Assert.Throws<HandlebarsException>(() => action(new { subject = this.Class("Describable") }));
        }

        [Test]
        public void Verify_that_QueryContainers_throws_when_not_exactly_two_arguments()
        {
            var template = "{{#each (Class.QueryContainers subject) as | c |}}{{c.Name}}{{/each}}";
            var action = this.handlebarsContext.Compile(template);

            Assert.Throws<HandlebarsException>(() => action(new { subject = this.Class("Address") }));
        }

        [Test]
        public void Verify_that_QueryConstraints_throws_when_not_exactly_one_argument()
        {
            var template = "{{#each (Class.QueryConstraints subject all) as | c |}}{{c.Name}}{{/each}}";
            var action = this.handlebarsContext.Compile(template);

            Assert.Throws<HandlebarsException>(() => action(new { subject = this.Class("Person"), all = this.allClasses }));
        }
    }
}
