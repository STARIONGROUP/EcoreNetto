// -------------------------------------------------------------------------------------------------
// <copyright file="EClassifier.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2025 Starion Group S.A.
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

namespace ECoreNetto
{
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// The super abstract type for a type representing a classifier
    /// </summary>
    public abstract class EClassifier : ENamedElement
    {
        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<EClassifier> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EClassifier"/> class
        /// </summary>        
        /// <param name="resource">
        /// The <see cref="ECoreNetto.Resource.Resource"/> containing all instantiated <see cref="EObject"/>
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        protected EClassifier(Resource.Resource resource, ILoggerFactory loggerFactory = null) : base(resource, loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<EClassifier>.Instance : loggerFactory.CreateLogger<EClassifier>();
        }

        /// <summary>
        /// Gets the instance class name.
        /// </summary>
        public string InstanceClassName { get; private set; }

        /// <summary>
        /// Gets the containing <see cref="EPackage"/>
        /// </summary>
        public EPackage EPackage => (EPackage)this.EContainer;

        /// <summary>
        /// Gets the hierarchy of  containing <see cref="EPackage"/>
        /// </summary>
        public IEnumerable<EPackage> EPackageTree
        {
            get
            {
                var tree = new List<EPackage>();
                var package = this.EPackage;
                while (package != null)
                {
                    tree.Add(package);
                    package = package.ESuperPackage;
                }

                tree.Reverse();
                return tree;
            }
        }

        /// <summary>
        /// Set the properties of this <see cref="EClassifier"/>
        /// </summary>
        internal override void SetProperties()
        {
            this.logger.LogTrace("setting properties of EClassifier {0}:{1}", this.Identifier, this.Name);

            base.SetProperties();

            if (this.Attributes.TryGetValue("instanceClassName", out var output))
            {
                this.InstanceClassName = output;
            }
        }

        /// <summary>
        /// Build the EModelElement.Identifier property
        /// </summary>
        /// <returns>
        /// The identifier
        /// </returns>
        protected override string BuildIdentifier()
        {
            return $"{this.EPackage.Identifier}/{this.Name}";
        }
    }
}
