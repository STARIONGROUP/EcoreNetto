// -------------------------------------------------------------------------------------------------
// <copyright file="ContainmentUpdater.cs" company="RHEA System S.A.">
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

namespace ECoreNetto.Utils
{
    using System;

    /// <summary>
    /// The purpose of <see cref="ContainmentUpdater"/> is to remove a contained <see cref="EObject"/>
    /// from its previous container when it is added to a containment property of a <see cref="EObject"/>
    /// </summary>
    internal static class ContainmentUpdater
    {
        /// <summary>
        /// Removes the 
        /// </summary>
        /// <param name="object">
        /// The subject <see cref="EObject"/> that is to be removed from its container
        /// </param>
        internal static void RemoveFromContainer(this EObject @object)
        {
            if (@object == null)
            {
                throw new ArgumentNullException(nameof(@object), "The EObject may not be null");
            }

            if (@object.EContainer == null)
            {
                return;
            }
            
            var annotation = @object as EAnnotation;
            if (annotation != null)
            {
                RemoveFromContainer(annotation);
                return;
            }

            var package = @object as EPackage;
            if (package != null)
            {
                RemoveFromContainer(package);
                return;
            }

            var classifier = @object as EClassifier;
            if (classifier != null)
            {
                RemoveFromContainer(classifier);
                return;
            }

            var parameter = @object as EParameter;
            if (parameter != null)
            {
                RemoveFromContainer(parameter);
                return;
            }

            var operation = @object as EOperation;
            if (operation != null)
            {
                RemoveFromContainer(operation);
                return;
            }

            var structuralFeature = @object as EStructuralFeature;
            if (structuralFeature != null)
            {
                RemoveFromContainer(structuralFeature);
                return;
            }

            var enumLiteral = @object as EEnumLiteral;
            if (enumLiteral != null)
            {
                RemoveFromContainer(enumLiteral);                
            }

            throw new ArgumentException($"The subclass is not supported {@object.GetType()}");
        }

        /// <summary>
        /// removes the <see cref="EAnnotation"/> from its container <see cref="EModelElement"/>
        /// </summary>
        /// <param name="annotation">
        /// the subject <see cref="EAnnotation"/>
        /// </param>
        private static void RemoveFromContainer(EAnnotation annotation)
        {
            var container = (EModelElement)annotation.EContainer;
            container.EAnnotations.Remove(annotation);
        }

        /// <summary>
        /// removes the <see cref="EPackage"/> from its container <see cref="EPackage"/>
        /// </summary>
        /// <param name="package">
        /// the subject <see cref="EPackage"/>
        /// </param>
        private static void RemoveFromContainer(EPackage package)
        {
            var container = (EPackage)package.EContainer;
            container.ESubPackages.Remove(package);
        }

        /// <summary>
        /// removes the <see cref="EClassifier"/> from its container <see cref="EPackage"/>
        /// </summary>
        /// <param name="classifier">
        /// the subject <see cref="EClassifier"/>
        /// </param>
        private static void RemoveFromContainer(EClassifier classifier)
        {
            var container = (EPackage)classifier.EContainer;
            container.EClassifiers.Remove(classifier);
        }

        /// <summary>
        /// removes the <see cref="EParameter"/> from its container <see cref="EOperation"/>
        /// </summary>
        /// <param name="parameter">
        /// the subject <see cref="EParameter"/>
        /// </param>
        private static void RemoveFromContainer(EParameter parameter)
        {
            var container = (EOperation)parameter.EContainer;
            container.EParameters.Remove(parameter);
        }

        /// <summary>
        /// removes the <see cref="EOperation"/> from its container <see cref="EClass"/>
        /// </summary>
        /// <param name="operation">
        /// the subject <see cref="EOperation"/>
        /// </param>
        private static void RemoveFromContainer(EOperation operation)
        {
            var container = (EClass)operation.EContainer;
            container.EOperations.Remove(operation);
        }

        /// <summary>
        /// removes the <see cref="EStructuralFeature"/> from its container <see cref="EClass"/>
        /// </summary>
        /// <param name="structuralFeature">
        /// the subject <see cref="EStructuralFeature"/>
        /// </param>
        private static void RemoveFromContainer(EStructuralFeature structuralFeature)
        {
            var container = (EClass)structuralFeature.EContainer;
            container.EStructuralFeatures.Remove(structuralFeature);
        }

        /// <summary>
        /// removes the <see cref="EEnumLiteral"/> from its container <see cref=""/>
        /// </summary>
        /// <param name="enumLiteral">
        /// the subject <see cref="EEnum"/>
        /// </param>
        private static void RemoveFromContainer(EEnumLiteral enumLiteral)
        {
            var container = (EEnum)enumLiteral.EContainer;
            container.ELiterals.Remove(enumLiteral);
        }
    }
}
