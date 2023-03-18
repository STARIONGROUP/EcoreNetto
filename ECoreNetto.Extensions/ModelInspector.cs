// -------------------------------------------------------------------------------------------------
// <copyright file="ModelInspector.cs" company="RHEA System S.A.">
// 
//   Copyright 2017-2023 RHEA System S.A.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The purpose of the <see cref="ModelInspector"/> is to iterate through the model and report on the various kinds of
    /// patters that exist in the ECore model that need to be taken into account for code-generation
    /// </summary>
    public class ModelInspector : IModelInspector
    {
        private readonly HashSet<EClass> interestingClasses = new();

        private readonly List<string> referenceTypes = new();

        private readonly List<string> valueTypes = new();

        private readonly List<string> enums = new();

        /// <summary>
        /// Inspect the content of the provided <see cref="EPackage"/> and returns the variation 
        /// of data-types, enums and multiplicity as an Analysis report
        /// </summary>
        /// <param name="package">
        /// The <see cref="EPackage"/> that needs to be inspected
        /// </param>
        /// <param name="recursive">
        /// Assertion whether the sub packages should be inspected or not
        /// </param>
        /// <returns>
        /// Returns a report of the classes of interest in the provided package 
        /// </returns>
        public string Inspect(EPackage package, bool recursive = false)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"----- PACKAGE {package.Name} ANALYSIS ------");

            this.Inspect(package, sb, recursive);

            sb.AppendLine("----- MULTIPLICITY RESULTS ------");

            var orderedReferenceTypes = this.referenceTypes.OrderBy(x => x);

            foreach (var referenceType in orderedReferenceTypes)
            {
                sb.AppendLine($"reference type: {referenceType}");
            }

            var orderedEnums = this.enums.OrderBy(x => x);

            foreach (var @enum in orderedEnums)
            {
                sb.AppendLine($"enum type: {@enum}");
            }

            var orderedValueTypes = this.valueTypes.OrderBy(x => x);

            foreach (var valueType in orderedValueTypes)
            {
                sb.AppendLine($"value type: {valueType}");
            }

            sb.AppendLine("----- INTERESTING CLASSES ------");

            foreach (var @class in this.interestingClasses.OrderBy(x => x.Name))
            {
                sb.AppendLine($"class: {package.Name}:{@class.Name}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Recursively inspects the content of the provided <see cref="EPackage"/>
        /// and adds the reported results to the provided <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="package">
        /// The <see cref="EPackage"/> which needs to be inspected
        /// </param>
        /// <param name="sb">
        /// The <see cref="StringBuilder"/> to which the results of hte inspection are reported
        /// </param>
        /// <param name="recursive">
        /// A value indicating whether the sub <see cref="EPackage"/>s need to be inspected as well
        /// </param>
        private void Inspect(EPackage package, StringBuilder sb, bool recursive)
        {
            foreach (var eClass in package.EClassifiers.OfType<EClass>().OrderBy(x => x.Name))
            {
                foreach (var structuralFeature in eClass.EStructuralFeatures)
                {
                    if (structuralFeature.Derived || structuralFeature.Transient)
                    {
                        continue;
                    }

                    if (structuralFeature is EReference reference)
                    {
                        var referenceType = reference.IsContainment ? $"{reference.LowerBound}:{reference.UpperBound}:containment" : $"{reference.LowerBound}:{reference.UpperBound}";

                        if (!this.referenceTypes.Contains(referenceType))
                        {
                            this.referenceTypes.Add(referenceType);
                            this.interestingClasses.Add(eClass);

                            sb.AppendLine($"{package.Name}.{eClass.Name} -- REF {referenceType}");
                        }
                    }

                    if (structuralFeature is EAttribute attribute)
                    {
                        if (structuralFeature.QueryIsEnum())
                        {
                            var @enum = $"{attribute.LowerBound}:{attribute.UpperBound}";

                            if (!this.enums.Contains(@enum))
                            {
                                this.enums.Add(@enum);
                                this.interestingClasses.Add(eClass);

                                sb.AppendLine($"{eClass.Name} -- ENUM {@enum}");
                            }

                        }
                        else
                        {
                            var valueType = $"{attribute.EType.Name}:{attribute.LowerBound}:{attribute.UpperBound}";

                            if (!this.valueTypes.Contains(valueType))
                            {
                                this.valueTypes.Add(valueType);
                                this.interestingClasses.Add(eClass);

                                sb.AppendLine($"{eClass.Name} -- VAL {valueType}");
                            }
                        }
                    }
                }
            }

            if (recursive)
            {
                foreach (var subPackage in package.ESubPackages)
                {
                    this.Inspect(subPackage, sb, true);
                }
            }
        }

        /// <summary>
        /// Inspect the provided <see cref="EClass"/> (by name) that is contained in the <see cref="EPackage"/>
        /// and returns the variation of data-types, enums and multiplicity as an Analysis report
        /// </summary>
        /// <param name="package">
        /// The <see cref="EPackage"/> that contains the <see cref="EClass"/> that
        /// is to be inspected
        /// </param>
        /// <param name="className">
        /// the name of the class that is to be inspected
        /// </param>
        /// <returns>
        /// returns a report detailing the various combinations of properties of the provided class
        /// </returns>
        public string Inspect(EPackage package, string className)
        {
            var sb = new StringBuilder();

            var eClass = package.EClassifiers.OfType<EClass>().Single(x => x.Name == className);
            
            sb.AppendLine($"{package.Name}.{eClass.Name}:");
            sb.AppendLine("----------------------------------");

            foreach (var structuralFeature in eClass.AllEStructuralFeaturesOrderByName)
            {
                if (structuralFeature.Derived || structuralFeature.Transient)
                {
                    continue;
                }

                if (structuralFeature is EReference reference)
                {
                    var referenceType = string.Empty;
                    if (reference.IsContainment)
                    {
                        referenceType = $"{reference.Name}:{reference.EType.Name} [{reference.LowerBound}..{reference.UpperBound}] - CONTAINED REFERENCE TYPE";
                    }
                    else
                    {
                        referenceType = $"{reference.Name}:{reference.EType.Name} [{reference.LowerBound}..{reference.UpperBound}] - REFERENCE TYPE";
                    }
                    
                    sb.AppendLine(referenceType);
                }

                if (structuralFeature is EAttribute attribute)
                {
                    if (structuralFeature.QueryIsEnum())
                    {
                        var @enum = $"{attribute.Name}:{attribute.EType.Name} [{attribute.LowerBound}..{attribute.UpperBound}] - ENUM TYPE";
                        sb.AppendLine(@enum);
                    }
                    else
                    {
                        var valueType = $"{attribute.Name}:{attribute.EType.Name} [{attribute.LowerBound}..{attribute.UpperBound}] - VALUETYPE";
                        sb.AppendLine(valueType);
                    }
                }
            }

            sb.AppendLine("-DERIVED--------------------------");
            foreach (var structuralFeature in eClass.AllEStructuralFeaturesOrderByName)
            {
                if (structuralFeature.Derived)
                {
                    if (structuralFeature is EReference reference)
                    {
                        var referenceType = $"{reference.Name}:{reference.EType.Name} [{reference.LowerBound}..{reference.UpperBound}] - REFERENCE TYPE";
                        sb.AppendLine(referenceType);
                    }

                    if (structuralFeature is EAttribute attribute)
                    {
                        if (structuralFeature.QueryIsEnum())
                        {
                            var @enum = $"{attribute.Name}:{attribute.EType.Name} [{attribute.LowerBound}..{attribute.UpperBound}] - ENUM TYPE";
                            sb.AppendLine(@enum);
                        }
                        else
                        {
                            var valueType = $"{attribute.Name}:{attribute.EType.Name} [{attribute.LowerBound}..{attribute.UpperBound}] - VALUETYPE";
                            sb.AppendLine(valueType);
                        }
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Recursively analyzes the documentation of the model and prints the names of all classes 
        /// and features that do not have any documentation in an analysis report
        /// </summary>
        /// <param name="package">
        /// The <see cref="EPackage"/> which needs to be inspected
        /// </param>
        /// <param name="recursive">
        /// A value indicating whether the sub <see cref="EPackage"/>s need to be Analyzed as well
        /// </param>
        /// <returns>
        /// returns a report of the classes and properties that do not contain any documentation
        /// </returns>
        public string AnalyzeDocumentation(EPackage package, bool recursive = false)
        {
            var sb = new StringBuilder();

            sb.AppendLine("----- DOCUMENTATION ANALYSIS ------");

            this.AnalyzeDocumentation(package, sb, recursive);
            
            return sb.ToString();
        }

        /// <summary>
        /// Recursively analyzes the documentation of the model and adds the result to the provided
        /// <see cref="StringBuilder"/>
        /// </summary>
        /// <param name="package">
        /// The <see cref="EPackage"/> which needs to be inspected
        /// </param>
        /// <param name="recursive">
        /// A value indicating whether the sub <see cref="EPackage"/>s need to be Analyzed as well
        /// </param>
        private void AnalyzeDocumentation(EPackage package, StringBuilder sb, bool recursive = false)
        {
            foreach (var eClass in package.EClassifiers.OfType<EClass>().OrderBy(x => x.Name))
            {
                if (string.IsNullOrEmpty(eClass.QueryRawDocumentation()))
                {
                    sb.AppendLine($"{package.Name}.{eClass.Name}");
                }

                foreach (var eStructuralFeature in eClass.EStructuralFeaturesOrderByName)
                {
                    if (string.IsNullOrEmpty(eStructuralFeature.QueryRawDocumentation()))
                    {
                        sb.AppendLine($"{package.Name}.{eClass.Name}:{eStructuralFeature.Name}");
                    }
                }
            }

            if (recursive)
            {
                foreach (var subPackage in package.ESubPackages)
                {
                    this.AnalyzeDocumentation(subPackage, sb, true);
                }
            }
        }
    }
}
