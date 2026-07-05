// ------------------------------------------------------------------------------------------------
// <copyright file="DiagramRendererTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Tools.Tests.Drawing
{
    using System.IO;
    using System.Linq;

    using ECoreNetto;
    using ECoreNetto.Reporting.Drawing;
    using ECoreNetto.Reporting.Payload;
    using ECoreNetto.Resource;

    using NUnit.Framework;

    /// <summary>
    /// Suite of smoke tests for the <see cref="InheritanceDiagramRenderer"/> and
    /// <see cref="AssociationDiagramRenderer"/> classes.
    /// </summary>
    [TestFixture]
    public class DiagramRendererTestFixture
    {
        private HandlebarsPayload payload = null!;

        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "report-features.ecore");
            var uri = new System.Uri(Path.GetFullPath(path));

            var resourceSet = new ResourceSet();
            var resource = resourceSet.CreateResource(uri);
            var root = resource.Load(null);

            var enums = root.EClassifiers.OfType<EEnum>();
            var dataTypes = root.EClassifiers.OfType<EDataType>().Where(x => !(x is EEnum)).ToList();
            var classes = root.EClassifiers.OfType<EClass>().ToList();

            this.payload = new HandlebarsPayload(
                root,
                enums,
                dataTypes.Where(x => !string.IsNullOrEmpty(x.InstanceClassName)),
                dataTypes.Where(x => string.IsNullOrEmpty(x.InstanceClassName)),
                classes,
                classes.Where(x => x.Interface));
        }

        [Test]
        public void Verify_that_the_inheritance_renderer_renders_the_whole_model()
        {
            var renderer = new InheritanceDiagramRenderer();

            var svg = renderer.SvgRender(this.payload);

            Assert.Multiple(() =>
            {
                Assert.That(svg, Does.Contain("<svg"));
                Assert.That(svg, Does.Contain("inheritance-diagram"));
            });
        }

        [Test]
        public void Verify_that_the_inheritance_renderer_renders_a_per_class_tree()
        {
            var renderer = new InheritanceDiagramRenderer();

            var person = this.payload.Classes.Single(x => x.Name == "Person");

            var svg = renderer.SvgRenderForClass(person, this.payload);

            Assert.Multiple(() =>
            {
                Assert.That(svg, Does.Contain("<svg"));
                Assert.That(svg, Does.Contain("inheritance-tree-"));
            });
        }

        [Test]
        public void Verify_that_the_association_renderer_renders_a_connected_class()
        {
            var renderer = new AssociationDiagramRenderer();

            var person = this.payload.Classes.Single(x => x.Name == "Person");

            var svg = renderer.SvgRenderForClass(person, this.payload);

            Assert.Multiple(() =>
            {
                Assert.That(svg, Does.Contain("<svg"));
                Assert.That(svg, Does.Contain("association-diagram-"));
            });
        }

        [Test]
        public void Verify_that_the_association_renderer_returns_empty_for_a_class_without_associations()
        {
            var renderer = new AssociationDiagramRenderer();

            var describable = this.payload.Classes.Single(x => x.Name == "Describable");

            var svg = renderer.SvgRenderForClass(describable, this.payload);

            Assert.That(svg, Is.Empty);
        }
    }
}
