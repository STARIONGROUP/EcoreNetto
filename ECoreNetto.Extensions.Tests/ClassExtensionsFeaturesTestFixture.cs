// ------------------------------------------------------------------------------------------------
// <copyright file="ClassExtensionsFeaturesTestFixture.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ECoreNetto.Extensions;
    using ECoreNetto.Resource;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the report-parity extension methods on <see cref="ClassExtensions"/> that are
    /// exercised against the feature-rich synthetic model.
    /// </summary>
    [TestFixture]
    public class ClassExtensionsFeaturesTestFixture
    {
        private EPackage rootPackage = null!;
        private List<EClass> allClasses = null!;

        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "report-features.ecore");
            var uri = new System.Uri(Path.GetFullPath(path));

            var resourceSet = new ResourceSet();
            var resource = resourceSet.CreateResource(uri);

            this.rootPackage = resource.Load(null);
            this.allClasses = this.rootPackage.EClassifiers.OfType<EClass>().ToList();
        }

        [Test]
        public void Verify_that_QueryContainers_returns_the_containing_classes()
        {
            var address = this.allClasses.Single(x => x.Name == "Address");

            var containers = address.QueryContainers(this.allClasses).ToList();

            // Person owns a containment reference (addresses) whose type is Address
            Assert.That(containers.Select(x => x.Name), Is.EquivalentTo(new[] { "Person" }));
        }

        [Test]
        public void Verify_that_QueryContainers_returns_empty_when_no_containment_reference_targets_the_class()
        {
            var company = this.allClasses.Single(x => x.Name == "Company");

            var containers = company.QueryContainers(this.allClasses);

            Assert.That(containers, Is.Empty);
        }

        [Test]
        public void Verify_that_QueryContainers_throws_when_argument_is_null()
        {
            EClass? @class = null;

            Assert.That(() => @class!.QueryContainers(this.allClasses), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryAllDescendantSpecializations_returns_all_subclasses()
        {
            var describable = this.allClasses.Single(x => x.Name == "Describable");

            var descendants = describable.QueryAllDescendantSpecializations(this.allClasses).ToList();

            Assert.That(descendants.Select(x => x.Name), Is.EquivalentTo(new[] { "Person" }));
        }

        [Test]
        public void Verify_that_QueryAllDescendantSpecializations_throws_when_argument_is_null()
        {
            EClass? @class = null;

            Assert.That(() => @class!.QueryAllDescendantSpecializations(this.allClasses), Throws.ArgumentNullException);
        }

        [Test]
        public void Verify_that_QueryConstraints_reads_the_constraint_and_its_ocl_body()
        {
            var person = this.allClasses.Single(x => x.Name == "Person");

            var constraints = person.QueryConstraints().ToList();

            Assert.That(constraints, Has.Count.EqualTo(1));

            var constraint = constraints.Single();

            Assert.Multiple(() =>
            {
                Assert.That(constraint.Name, Is.EqualTo("hasFullName"));
                Assert.That(constraint.Language, Is.EqualTo("OCL"));
                Assert.That(constraint.Body, Is.EqualTo("self.fullName.notEmpty()"));
            });
        }

        [Test]
        public void Verify_that_QueryConstraints_returns_empty_when_no_constraints_are_declared()
        {
            var address = this.allClasses.Single(x => x.Name == "Address");

            var constraints = address.QueryConstraints();

            Assert.That(constraints, Is.Empty);
        }

        [Test]
        public void Verify_that_QueryConstraints_throws_when_argument_is_null()
        {
            EClass? @class = null;

            Assert.That(() => @class!.QueryConstraints(), Throws.ArgumentNullException);
        }
    }
}
