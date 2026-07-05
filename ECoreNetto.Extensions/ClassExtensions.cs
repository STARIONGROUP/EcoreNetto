// ------------------------------------------------------------------------------------------------
// <copyright file="ClassExtensions.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="EClass"/> class
    /// </summary>
    public static class ClassExtensions
    {
        /// <summary>
        /// Queries for the subject <see cref="EClass"/> which are specializations (sub classes). 
        /// </summary>
        /// <param name="class">
        /// The <see cref="EClass"/> for which the specializations need to be computed
        /// </param>
        /// <param name="allClasses">
        /// The <see cref="IEnumerable{EClass}"/> from which the generalization / specializations are computed
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{EClass}"/> that contains the specializations (sub classes) if the subject <see cref="EClass"/>
        /// </returns>
        public static IEnumerable<EClass> QuerySpecializations(this EClass @class, IEnumerable<EClass> allClasses)
        {
            if (@class == null)
            {
                throw new ArgumentNullException(nameof(@class));
            }

            var result = allClasses
                .Where(x => x.ESuperTypes.Contains(@class));

            return result;
        }

        /// <summary>
        /// The annotation source under which Ecore declares the names of a classifier's constraints.
        /// </summary>
        private const string EcoreAnnotationSource = "http://www.eclipse.org/emf/2002/Ecore";

        /// <summary>
        /// The annotation source under which Ecore stores the OCL body of each named constraint.
        /// </summary>
        private const string OclAnnotationSource = "http://www.eclipse.org/emf/2002/Ecore/OCL";

        /// <summary>
        /// Queries all (transitive) specializations (direct and indirect sub classes) of the subject <see cref="EClass"/>.
        /// </summary>
        /// <param name="class">
        /// The <see cref="EClass"/> for which the descendant specializations are computed.
        /// </param>
        /// <param name="allClasses">
        /// The <see cref="IEnumerable{EClass}"/> from which the specializations are computed.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{EClass}"/> that contains all direct and indirect sub classes of the subject <see cref="EClass"/>.
        /// </returns>
        public static IEnumerable<EClass> QueryAllDescendantSpecializations(this EClass @class, IEnumerable<EClass> allClasses)
        {
            if (@class == null)
            {
                throw new ArgumentNullException(nameof(@class));
            }

            var classes = allClasses.ToList();

            var result = new List<EClass>();
            var queue = new Queue<EClass>(@class.QuerySpecializations(classes));

            while (queue.Count > 0)
            {
                var specialization = queue.Dequeue();

                if (result.Contains(specialization) || specialization == @class)
                {
                    continue;
                }

                result.Add(specialization);

                foreach (var deeper in specialization.QuerySpecializations(classes))
                {
                    queue.Enqueue(deeper);
                }
            }

            return result;
        }

        /// <summary>
        /// Queries the classes that contain the subject <see cref="EClass"/> through a containment <see cref="EReference"/>.
        /// </summary>
        /// <param name="class">
        /// The <see cref="EClass"/> for which the containers are computed.
        /// </param>
        /// <param name="allClasses">
        /// The <see cref="IEnumerable{EClass}"/> from which the containers are computed.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{EClass}"/> that owns a containment reference whose type is the subject <see cref="EClass"/>.
        /// </returns>
        public static IEnumerable<EClass> QueryContainers(this EClass @class, IEnumerable<EClass> allClasses)
        {
            if (@class == null)
            {
                throw new ArgumentNullException(nameof(@class));
            }

            return allClasses
                .Where(candidate => candidate.EStructuralFeatures
                    .OfType<EReference>()
                    .Any(reference => reference.IsContainment && reference.EType == @class));
        }

        /// <summary>
        /// Queries the constraints (rules) declared on the subject <see cref="EClass"/> through its Ecore
        /// constraint annotation, pairing each constraint name with its OCL body when present.
        /// </summary>
        /// <param name="class">
        /// The <see cref="EClass"/> whose constraints are read.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{ConstraintInfo}"/>, one per declared constraint name; empty when the
        /// class declares no constraints.
        /// </returns>
        public static IEnumerable<ConstraintInfo> QueryConstraints(this EClass @class)
        {
            if (@class == null)
            {
                throw new ArgumentNullException(nameof(@class));
            }

            var result = new List<ConstraintInfo>();

            var constraintsAnnotation = @class.EAnnotations.FirstOrDefault(a => a.Source == EcoreAnnotationSource);
            if (constraintsAnnotation == null || !constraintsAnnotation.Details.TryGetValue("constraints", out var names))
            {
                return result;
            }

            var oclAnnotation = @class.EAnnotations.FirstOrDefault(a => a.Source == OclAnnotationSource);

            foreach (var name in names.Split(' '))
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                string? body = null;
                oclAnnotation?.Details.TryGetValue(name, out body);

                result.Add(new ConstraintInfo(name, body ?? string.Empty, oclAnnotation != null ? "OCL" : string.Empty));
            }

            return result;
        }

        /// <summary>
        /// Queries the type hierarchy (chain of super classes) of the provided <see cref="EClass"/>
        /// </summary>
        /// <param name="class">
        /// The subject <see cref="EClass"/> for which the type hierarchy (chain of super classes) needs to be computed
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{EClass}"/> that contains the chain of superclasses
        /// </returns>
        public static IEnumerable<EClass> QueryTypeHierarchy(this EClass @class)
        {
            if (@class == null)
            {
                throw new ArgumentNullException(nameof(@class));
            }

            var result = new List<EClass>();

            foreach (var superType in @class.ESuperTypes)
            {
                result.Add(superType);

                result.AddRange(superType.QueryTypeHierarchy());
            }

            return result;
        }
    }
}
