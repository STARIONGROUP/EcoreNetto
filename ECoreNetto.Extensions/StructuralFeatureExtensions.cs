// -------------------------------------------------------------------------------------------------
// <copyright file="StructuralFeatureExtensions.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2024 Starion Group S.A.
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

namespace ECoreNetto.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="EStructuralFeature"/> class
    /// </summary>
    public static class StructuralFeatureExtensions
    {
        /// <summary>
        /// Queries whether the type of the <see cref="EStructuralFeature"/> is an <see cref="EEnum"/>
        /// </summary>
        /// <param name="eStructuralFeature">
        /// The subject <see cref="EStructuralFeature"/>
        /// </param>
        /// <returns>
        /// true of the type is a <see cref="EEnum"/>, false if not
        /// </returns>
        public static bool QueryIsEnum(this EStructuralFeature eStructuralFeature)
        {
            if (eStructuralFeature is EAttribute eAttribute)
            {
                return eAttribute.EType is EEnum;
            }

            return false;
        }

        /// <summary>
        /// Queries the <see cref="EClass"/> that represents the Type of the feature.
        /// </summary>
        /// <param name="eStructuralFeature">
        /// The subject <see cref="EStructuralFeature"/>
        /// </param>
        /// <returns>
        /// An instance of <see cref="EClass"/> when the <see cref="EStructuralFeature"/> is
        /// an <see cref="EReference"/>, null in case it is not.
        /// </returns>
        public static EClass QueryClass(this EStructuralFeature eStructuralFeature)
        {
            return eStructuralFeature is EReference { EType: EClass eClass } ? eClass : null;
        }

        /// <summary>
        /// Queries whether the <see cref="EStructuralFeature"/> is Enumerable
        /// </summary>
        /// <param name="eStructuralFeature">
        /// The subject <see cref="EStructuralFeature"/>
        /// </param>
        /// <returns>
        /// true if <see cref="EStructuralFeature.UpperBound"/> = -1 or > 1, false if not
        /// </returns>
        public static bool QueryIsEnumerable(this EStructuralFeature eStructuralFeature)
        {
            return eStructuralFeature.UpperBound is -1 or > 1;
        }

        /// <summary>
        /// Queries whether the <see cref="EStructuralFeature"/> is an <see cref="EAttribute"/> or not
        /// </summary>
        /// <param name="structuralFeature">
        /// The subject <see cref="EStructuralFeature"/>
        /// </param>
        /// <returns>
        /// true when the <paramref name="structuralFeature"/> is an instance of <see cref="EAttribute"/>, false if not.
        /// </returns>
        public static bool QueryIsAttribute(this EStructuralFeature structuralFeature)
        {
            return structuralFeature is EAttribute;
        }

        /// <summary>
        /// Queries whether the <see cref="EStructuralFeature"/> is an <see cref="EReference"/> or not
        /// </summary>
        /// <param name="structuralFeature">
        /// The subject <see cref="EStructuralFeature"/>
        /// </param>
        /// <returns>
        /// true when the <paramref name="structuralFeature"/> is an instance of <see cref="EReference"/>, false if not.
        /// </returns>
        public static bool QueryIsReference(this EStructuralFeature structuralFeature)
        {
            return structuralFeature is EReference;
        }

        /// <summary>
        /// Queries whether the <see cref="EStructuralFeature"/> is of type containment (which may only be the case
        /// for an <see cref="EReference"/>, not for an <see cref="EAttribute"/>)
        /// </summary>
        /// <param name="structuralFeature">
        /// The subject <see cref="EStructuralFeature"/>
        /// </param>
        /// <returns>
        /// True when it is, false when not
        /// </returns>
        public static bool QueryIsContainment(this EStructuralFeature structuralFeature)
        {
            if (structuralFeature is EReference reference)
            {
                return reference.IsContainment;
            }

            return false;
        }

        /// <summary>
        /// Queries whether the <see cref="EStructuralFeature.Name"/> is equal to the name of the containing <see cref="EClass"/>
        /// </summary>
        /// <param name="structuralFeature">
        /// The subject <see cref="EStructuralFeature"/>
        /// </param>
        /// <param name="class">
        /// The containing <see cref="EClass"/>
        /// </param>
        /// <returns>
        /// true when the <paramref name="structuralFeature"/> name equals the name of the containing <see cref="EClass"/>, false if not.
        /// </returns>
        public static bool QueryStructuralFeatureNameEqualsEnclosingType(this EStructuralFeature structuralFeature, EClass @class)
        {
            if (structuralFeature.Name.ToLower() == @class.Name.ToLower())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Queries whether the <see cref="EStructuralFeature"/> is has a default value
        /// </summary>
        /// <param name="eStructuralFeature">
        /// The subject <see cref="EStructuralFeature"/>
        /// </param>
        /// <returns>
        /// true when the <see cref="EStructuralFeature.DefaultValueLiteral"/> contains a value
        /// </returns>
        public static bool QueryHasDefaultValue(this EStructuralFeature eStructuralFeature)
        {
            return !string.IsNullOrEmpty(eStructuralFeature.DefaultValueLiteral);
        }

        /// <summary>
        /// Queries the type-name of the <see cref="EStructuralFeature"/>
        /// </summary>
        /// <param name="eStructuralFeature">
        /// The subject <see cref="EStructuralFeature"/>
        /// </param>
        /// <returns>
        /// the name of the type
        /// </returns>
        public static string QueryTypeName(this EStructuralFeature eStructuralFeature)
        {
            var typeName = "";

            if (eStructuralFeature is EAttribute eAttribute)
            {
                typeName = eAttribute.EType?.Name;

            }
            else if (eStructuralFeature is EReference eReference)
            {
                typeName = eReference.EType?.Name;
            }

            return typeName;
        }

        /// <summary>
        /// Queries whether the <see cref="EStructuralFeature"/> is nullable
        /// </summary>
        /// <param name="eStructuralFeature">
        /// The subject <see cref="EStructuralFeature"/>
        /// </param>
        /// <returns>
        /// true if <see cref="EStructuralFeature.LowerBound"/> = 0, false if not
        /// </returns>
        public static bool QueryIsNullable(this EStructuralFeature eStructuralFeature)
        {
            return eStructuralFeature.LowerBound == 0 && !eStructuralFeature.QueryIsEnumerable();
        }
    }
}
