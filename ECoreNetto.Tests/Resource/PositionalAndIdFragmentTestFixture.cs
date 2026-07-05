// ------------------------------------------------------------------------------------------------
// <copyright file="PositionalAndIdFragmentTestFixture.cs" company="Starion Group S.A.">
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

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the EMF URI-fragment-resolution items of issue #106: positional
    /// <c>//@feature.index</c> fragments (resolved by structural navigation) and bare <c>xmi:id</c>
    /// fragments (resolved through an id-to-object map), both same-file and cross-file.
    /// </summary>
    [TestFixture]
    public class PositionalAndIdFragmentTestFixture
    {
        private ResourceSet resourceSet = null!;

        [SetUp]
        public void SetUp()
        {
            this.resourceSet = new ResourceSet();
        }

        [Test]
        public void Verify_that_same_file_positional_fragments_resolve()
        {
            var root = this.Load("positional.ecore", PositionalModel);

            var alpha = Class(root, "Alpha");
            var beta = Class(root, "Beta");
            var text = root.EClassifiers.OfType<EDataType>().Single(c => c.Name == "Text");

            var toBeta = alpha.EStructuralFeatures.OfType<EReference>().Single(r => r.Name == "toBeta");
            var label = beta.EStructuralFeatures.OfType<EAttribute>().Single(a => a.Name == "label");

            Assert.Multiple(() =>
            {
                // eType="#//@eClassifiers.1" -> the second classifier, Beta
                Assert.That(toBeta.EType, Is.SameAs(beta));

                // eType="#//@eClassifiers.2" -> the third classifier, the Text data type
                Assert.That(label.EType, Is.SameAs(text));
            });
        }

        [Test]
        public void Verify_that_a_deep_positional_fragment_resolves_to_a_structural_feature()
        {
            var root = this.Load("positional.ecore", PositionalModel);
            var resource = this.resourceSet.Resources.Single();

            var toBeta = Class(root, "Alpha").EStructuralFeatures.Single(f => f.Name == "toBeta");

            var resolved = resource.GetEObject("positional.ecore#//@eClassifiers.0/@eStructuralFeatures.0");

            Assert.That(resolved, Is.SameAs(toBeta));
        }

        [Test]
        public void Verify_that_out_of_range_and_unknown_positional_fragments_do_not_resolve()
        {
            this.Load("positional.ecore", PositionalModel);
            var resource = this.resourceSet.Resources.Single();

            Assert.Multiple(() =>
            {
                // index beyond the number of classifiers
                Assert.That(resource.GetEObject("positional.ecore#//@eClassifiers.99"), Is.Null);

                // a containment feature that Alpha does not have
                Assert.That(resource.GetEObject("positional.ecore#//@eClassifiers.0/@eOperations.0"), Is.Null);

                // an unknown containment feature name
                Assert.That(resource.GetEObject("positional.ecore#//@eClassifiers.0/@bogus.0"), Is.Null);

                // a navigation segment that is not of the '@feature.index' form
                Assert.That(resource.GetEObject("positional.ecore#//@eClassifiers.0/plain"), Is.Null);

                // an '@feature' segment without an index
                Assert.That(resource.GetEObject("positional.ecore#//@eClassifiers"), Is.Null);

                // an '@feature.index' segment with a non-numeric index
                Assert.That(resource.GetEObject("positional.ecore#//@eClassifiers.x"), Is.Null);

                // a name-based path that does not exist (structural navigation is only for '@' segments)
                Assert.That(resource.GetEObject("positional.ecore#//NoSuchClass"), Is.Null);

                // an empty in-document fragment
                Assert.That(resource.GetEObject("positional.ecore#"), Is.Null);

                // an unknown xmi:id
                Assert.That(resource.GetEObject("#_missing"), Is.Null);
            });
        }

        [Test]
        public void Verify_that_positional_fragments_navigate_every_containment_feature()
        {
            this.Load("rich.ecore", RichModel);
            var resource = this.resourceSet.Resources.Single();

            EObject? Resolve(string fragment) => resource.GetEObject($"rich.ecore#{fragment}");

            Assert.Multiple(() =>
            {
                Assert.That(((EPackage)Resolve("//@eSubpackages.0")!).Name, Is.EqualTo("sub"));
                Assert.That(((EClass)Resolve("//@eSubpackages.0/@eClassifiers.0")!).Name, Is.EqualTo("InSub"));
                Assert.That(Resolve("//@eClassifiers.0/@eAnnotations.0"), Is.InstanceOf<EAnnotation>());
                Assert.That(((ETypeParameter)Resolve("//@eClassifiers.0/@eTypeParameters.0")!).Name, Is.EqualTo("T"));
                Assert.That(Resolve("//@eClassifiers.0/@eTypeParameters.0/@eBounds.0"), Is.InstanceOf<EGenericType>());
                Assert.That(((EOperation)Resolve("//@eClassifiers.0/@eOperations.0")!).Name, Is.EqualTo("doIt"));
                Assert.That(((ETypeParameter)Resolve("//@eClassifiers.0/@eOperations.0/@eTypeParameters.0")!).Name, Is.EqualTo("U"));
                Assert.That(((EParameter)Resolve("//@eClassifiers.0/@eOperations.0/@eParameters.0")!).Name, Is.EqualTo("arg"));
                Assert.That(Resolve("//@eClassifiers.0/@eOperations.0/@eGenericExceptions.0"), Is.InstanceOf<EGenericType>());
                Assert.That(Resolve("//@eClassifiers.1/@eGenericSuperTypes.0"), Is.InstanceOf<EGenericType>());
                Assert.That(((EEnumLiteral)Resolve("//@eClassifiers.2/@eLiterals.0")!).Name, Is.EqualTo("RED"));
            });
        }

        [Test]
        public void Verify_that_a_same_file_xmi_id_fragment_resolves()
        {
            var root = this.Load("ids.ecore", XmiIdModel);

            var node = Class(root, "Node");
            var parent = node.EStructuralFeatures.OfType<EReference>().Single(r => r.Name == "parent");

            // eType="#_node" is an intrinsic xmi:id reference; Node registers the id first, so a later
            // duplicate xmi:id does not override it and the reference resolves back to Node
            Assert.That(parent.EType, Is.SameAs(node));
        }

        [Test]
        public void Verify_that_cross_file_positional_and_xmi_id_fragments_resolve()
        {
            // the target file must sit next to the source so the demand-load can find and load it; it is
            // written to disk only (not pre-registered as a resource), mirroring real cross-file loading
            WriteFile("target.ecore", TargetModel);
            var root = this.Load("source.ecore", SourceModel);

            var source = Class(root, "SourceClass");
            var viaPosition = source.EStructuralFeatures.OfType<EReference>().Single(r => r.Name == "viaPosition");
            var viaId = source.EStructuralFeatures.OfType<EReference>().Single(r => r.Name == "viaId");

            Assert.Multiple(() =>
            {
                Assert.That(viaPosition.EType, Is.Not.Null);
                Assert.That(viaPosition.EType!.Name, Is.EqualTo("TargetClass"));

                // both forms address the same object in the target resource
                Assert.That(viaId.EType, Is.SameAs(viaPosition.EType));
            });
        }

        private static EClass Class(EPackage root, string name)
        {
            return root.EClassifiers.OfType<EClass>().Single(c => c.Name == name);
        }

        private static void WriteFile(string fileName, string content)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);
            File.WriteAllText(path, content);
        }

        private Resource CreateResource(string fileName, string content)
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, fileName);
            File.WriteAllText(path, content);

            return this.resourceSet.CreateResource(new Uri(path));
        }

        private EPackage Load(string fileName, string content)
        {
            var resource = this.CreateResource(fileName, content);
            var root = resource.Load(null);

            Assert.That(resource.Errors, Is.Empty, string.Join("; ", resource.Errors.Select(e => e.Message)));

            return root;
        }

        private const string Header =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
            "<ecore:EPackage xmi:version=\"2.0\" xmlns:xmi=\"http://www.omg.org/XMI\" " +
            "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
            "xmlns:ecore=\"http://www.eclipse.org/emf/2002/Ecore\" ";

        private const string PositionalModel =
            Header +
            "name=\"positional\" nsURI=\"positional\" nsPrefix=\"positional\">\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"Alpha\">\r\n" +
            "    <eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"toBeta\" eType=\"#//@eClassifiers.1\"/>\r\n" +
            "  </eClassifiers>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"Beta\">\r\n" +
            "    <eStructuralFeatures xsi:type=\"ecore:EAttribute\" name=\"label\" eType=\"#//@eClassifiers.2\"/>\r\n" +
            "  </eClassifiers>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EDataType\" name=\"Text\" instanceClassName=\"java.lang.String\"/>\r\n" +
            "</ecore:EPackage>";

        private const string RichModel =
            Header +
            "name=\"rich\" nsURI=\"rich\" nsPrefix=\"rich\">\r\n" +
            "  <eSubpackages name=\"sub\" nsURI=\"sub\" nsPrefix=\"sub\">\r\n" +
            "    <eClassifiers xsi:type=\"ecore:EClass\" name=\"InSub\"/>\r\n" +
            "  </eSubpackages>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"Container\">\r\n" +
            "    <eAnnotations source=\"http://example.org/a\"/>\r\n" +
            "    <eTypeParameters name=\"T\">\r\n" +
            "      <eBounds eClassifier=\"#//Container\"/>\r\n" +
            "    </eTypeParameters>\r\n" +
            "    <eOperations name=\"doIt\">\r\n" +
            "      <eTypeParameters name=\"U\"/>\r\n" +
            "      <eParameters name=\"arg\" eType=\"#//Container\"/>\r\n" +
            "      <eGenericExceptions eClassifier=\"#//Container\"/>\r\n" +
            "    </eOperations>\r\n" +
            "  </eClassifiers>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"Derived\">\r\n" +
            "    <eGenericSuperTypes eClassifier=\"#//Container\"/>\r\n" +
            "  </eClassifiers>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EEnum\" name=\"Color\">\r\n" +
            "    <eLiterals name=\"RED\" value=\"0\"/>\r\n" +
            "  </eClassifiers>\r\n" +
            "</ecore:EPackage>";

        private const string XmiIdModel =
            Header +
            "name=\"ids\" nsURI=\"ids\" nsPrefix=\"ids\">\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"Node\" xmi:id=\"_node\">\r\n" +
            "    <eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"parent\" eType=\"#_node\"/>\r\n" +
            "  </eClassifiers>\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"Other\" xmi:id=\"_node\"/>\r\n" +
            "</ecore:EPackage>";

        private const string TargetModel =
            Header +
            "name=\"target\" nsURI=\"target\" nsPrefix=\"target\">\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"TargetClass\" xmi:id=\"_target\"/>\r\n" +
            "</ecore:EPackage>";

        private const string SourceModel =
            Header +
            "name=\"source\" nsURI=\"source\" nsPrefix=\"source\">\r\n" +
            "  <eClassifiers xsi:type=\"ecore:EClass\" name=\"SourceClass\">\r\n" +
            "    <eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"viaPosition\" eType=\"target.ecore#//@eClassifiers.0\"/>\r\n" +
            "    <eStructuralFeatures xsi:type=\"ecore:EReference\" name=\"viaId\" eType=\"target.ecore#_target\"/>\r\n" +
            "  </eClassifiers>\r\n" +
            "</ecore:EPackage>";
    }
}
