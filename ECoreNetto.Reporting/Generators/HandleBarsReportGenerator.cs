// -------------------------------------------------------------------------------------------------
// <copyright file="HandleBarsReportGenerator.cs" company="Starion Group S.A">
// 
//   Copyright 2017-2025 Starion Group S.A.
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

namespace ECoreNetto.Reporting.Generators
{
    using System.Collections.Generic;
    using System.Linq;

    using ECoreNetto.Extensions;
    using ECoreNetto.Reporting.Payload;
    using ECoreNetto.Reporting.Resources;

    using HandlebarsDotNet;
    using HandlebarsDotNet.Helpers;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Abstract super class from which all <see cref="HandlebarsDotNet"/> generators
    /// need to derive
    /// </summary>
    public abstract class HandleBarsReportGenerator : ReportGenerator
    {
        /// <summary>
        /// The <see cref="IHandlebars"/> instance used to generate code with
        /// </summary>
        protected IHandlebars Handlebars;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandleBarsReportGenerator"/> class
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected HandleBarsReportGenerator(ILoggerFactory loggerFactory = null) : base (loggerFactory)
        {
            this.Templates = new Dictionary<string, HandlebarsTemplate<object, object>>();

            this.Handlebars = HandlebarsDotNet.Handlebars.CreateSharedEnvironment();
            HandlebarsHelpers.Register(Handlebars);

            this.RegisterHelpers();
            this.RegisterTemplates();
        }

        /// <summary>
        /// Gets the registered <see cref="HandlebarsTemplate{TContext,TData}"/>
        /// </summary>
        public Dictionary<string, HandlebarsTemplate<object, object>> Templates { get; private set; }

        /// <summary>
        /// Register the custom helpers
        /// </summary>
        protected virtual void RegisterHelpers()
        {
            ECoreNetto.HandleBars.StringHelper.RegisterStringHelper(this.Handlebars);
            ECoreNetto.HandleBars.StructuralFeatureHelper.RegisterStructuralFeatureHelper(this.Handlebars);
            ECoreNetto.HandleBars.GeneralizationHelper.RegisterGeneralizationHelper(this.Handlebars);
            ECoreNetto.HandleBars.DocumentationHelper.RegisteredDocumentationHelper(this.Handlebars);
        }

        /// <summary>
        /// Register the code templates
        /// </summary>
        protected abstract void RegisterTemplates();

        /// <summary>
        /// Register handle-bars templates based on the template (file) name (without extension)
        /// </summary>
        /// <param name="name">
        /// (file) name (without extension)
        /// </param>
        protected void RegisterEmbeddedTemplate(string name)
        {
            var templatePath = $"ECoreNetto.Reporting.Templates.{name}.hbs";

            var template = ResourceLoader.LoadEmbeddedResource(templatePath);

            var compiledTemplate = Handlebars.Compile(template);

            this.Templates.Add(name, compiledTemplate);
        }

        /// <summary>
        /// Creates a <see cref="HandlebarsPayload"/> based on the provided root <see cref="EPackage"/>
        /// </summary>
        /// <param name="rootPackage">
        /// the subject root <see cref="EPackage"/>
        /// </param>
        /// <returns>
        /// an instance of <see cref="HandlebarsPayload"/>
        /// </returns>
        protected static HandlebarsPayload CreateHandlebarsPayload(EPackage rootPackage)
        {
            var packages = rootPackage.QueryPackages();

            var enums = new List<EEnum>();
            var dataTypes = new List<EDataType>();
            var eClasses = new List<EClass>();

            foreach (var package in packages)
            {
                enums.AddRange(package.EClassifiers.OfType<EEnum>());

                dataTypes.AddRange(package.EClassifiers
                    .OfType<EDataType>()
                    .Where(x => !(x is EEnum))
                    .OrderBy(x => x.Name));

                eClasses.AddRange(package.EClassifiers.OfType<EClass>());
            }

            var orderedEnums = enums.OrderBy(x => x.Name);
            var orderedDataTypes = dataTypes.OrderBy(x => x.Name);
            var orderedClasses = eClasses.OrderBy(x => x.Name);

            var payload = new HandlebarsPayload(rootPackage, orderedEnums, orderedDataTypes, orderedClasses);

            return payload;
        }
    }
}
