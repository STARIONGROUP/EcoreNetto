// -------------------------------------------------------------------------------------------------
// <copyright file="Resource.cs" company="Starion Group S.A.">
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

namespace ECoreNetto.Resource
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using ECoreNetto.Utils;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// A persistent document
    /// </summary>
    /// <remarks>
    /// A resource of an appropriate type is created by a resource factory; a resource set indirectly creates a resource using such a factory. 
    /// A resource is typically contained by a resource set, along with related resources. 
    /// It has a URI representing it's identity and that URI is used to determine where to save and load. 
    /// It provides modeled contents, in fact, it provides even the tree of modeled contents, as well as diagnostics for errors and other problems. 
    /// It may be unloaded to discard the contents and the load state can be queried. Modification can be tracked, but it's expensive. 
    /// The resource will be informed as objects are attached and detached; if needed, it will be able to maintain a map to support getEObject. 
    /// Structured URI fragments are used rather than IDs, since they are a more general alternative. 
    /// Clients must extend the default implementation, or one of its derived classes, since methods can and will be added to this API. 
    /// </remarks>
    public class Resource : Notifier
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<Resource> logger;

        /// <summary>
        /// backing field that is used to register whether a <see cref="Resource"/> is loaded or not.
        /// </summary>
        private bool isLoaded;

        /// <summary>
        /// a list of errors that may be populated during loading of the <see cref="Resource"/>
        /// </summary>
        private readonly List<Diagnostic> errors;

        /// <summary>
        /// a list of warnings that may be populated during loading of the <see cref="Resource"/>
        /// </summary>
        private readonly List<Diagnostic> warnings;

        /// <summary>
        /// A collection of <see cref="EObject"/> representing ECORE types
        /// </summary>
        private readonly Dictionary<string, EObject> eCoreTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public Resource(ILoggerFactory loggerFactory = null)
        {
            this.loggerFactory = loggerFactory;

            this.logger = this.loggerFactory == null ? NullLogger<Resource>.Instance : this.loggerFactory.CreateLogger<Resource>();

            var ecoreObjectFactory = new EcoreObjectFactory(this, this.loggerFactory);

            this.eCoreTypes = new Dictionary<string, EObject>
            {
                { "//EObject", ecoreObjectFactory.EObject },
                { "//EModelElement", ecoreObjectFactory.EModelElement },
                { "//ENamedElement", ecoreObjectFactory.ENamedElement },
                { "//EFactory", ecoreObjectFactory.EFactory },
                { "//EAnnotation", ecoreObjectFactory.EAnnotation },
                { "//EClassifier", ecoreObjectFactory.EClassifier },
                { "//EEnumLiteral", ecoreObjectFactory.EEnumLiteral },
                { "//EPackage", ecoreObjectFactory.EPackage },
                { "//ETypedElement", ecoreObjectFactory.ETypedElement },
                { "//EClass", ecoreObjectFactory.EClass },
                { "//EDataType", ecoreObjectFactory.EDataType },
                { "//EEnum", ecoreObjectFactory.EEnum },
                { "//EOperation", ecoreObjectFactory.EOperation },
                { "//EParameter", ecoreObjectFactory.EParameter },
                { "//EStructuralFeature", ecoreObjectFactory.EStructuralFeature },
                { "//EAttribute", ecoreObjectFactory.EAttribute },
                { "//EReference", ecoreObjectFactory.EReference },
                { "//EStringToStringMapEntry", ecoreObjectFactory.EStringToStringMapEntry },
                { "//EGenericType", ecoreObjectFactory.EGenericType },
                { "//ETypeParameter", ecoreObjectFactory.ETypeParameter },
                
                { "http://www.eclipse.org/emf/2002/Ecore#//EBigDecimal", ecoreObjectFactory.EBigDecimal},
                { "http://www.eclipse.org/emf/2002/Ecore#//EBigInteger", ecoreObjectFactory.EBigInteger},
                { "http://www.eclipse.org/emf/2002/Ecore#//EBool", ecoreObjectFactory.EBool},
                { "http://www.eclipse.org/emf/2002/Ecore#//EBooleanObject", ecoreObjectFactory.EBooleanObject},
                { "http://www.eclipse.org/emf/2002/Ecore#//EByte", ecoreObjectFactory.EByte},
                { "http://www.eclipse.org/emf/2002/Ecore#//EByteArray", ecoreObjectFactory.EByteArray},
                { "http://www.eclipse.org/emf/2002/Ecore#//EByteObject", ecoreObjectFactory.EByteObject},
                { "http://www.eclipse.org/emf/2002/Ecore#//EChar", ecoreObjectFactory.EChar},
                { "http://www.eclipse.org/emf/2002/Ecore#//ECharacterObject", ecoreObjectFactory.ECharacterObject},
                { "http://www.eclipse.org/emf/2002/Ecore#//EDate", ecoreObjectFactory.EDate},
                { "http://www.eclipse.org/emf/2002/Ecore#//EDiagnosticChain", ecoreObjectFactory.EDiagnosticChain},
                { "http://www.eclipse.org/emf/2002/Ecore#//EDouble", ecoreObjectFactory.EDouble},
                { "http://www.eclipse.org/emf/2002/Ecore#//EDoubleObject", ecoreObjectFactory.EDoubleObject},
                { "http://www.eclipse.org/emf/2002/Ecore#//EEList", ecoreObjectFactory.EEList},
                { "http://www.eclipse.org/emf/2002/Ecore#//EEnumerator", ecoreObjectFactory.EEnumerator},
                { "http://www.eclipse.org/emf/2002/Ecore#//EFeatureMap", ecoreObjectFactory.EFeatureMap},
                { "http://www.eclipse.org/emf/2002/Ecore#//EFeatureMapEntry", ecoreObjectFactory.EFeatureMapEntry},
                { "http://www.eclipse.org/emf/2002/Ecore#//EFloat", ecoreObjectFactory.EFloat},
                { "http://www.eclipse.org/emf/2002/Ecore#//EFloatObject", ecoreObjectFactory.EFloatObject},
                { "http://www.eclipse.org/emf/2002/Ecore#//EInt", ecoreObjectFactory.EInt},
                { "http://www.eclipse.org/emf/2002/Ecore#//EIntegerObject", ecoreObjectFactory.EIntegerObject},
                { "http://www.eclipse.org/emf/2002/Ecore#//EJavaClass", ecoreObjectFactory.EJavaClass},
                { "http://www.eclipse.org/emf/2002/Ecore#//EJavaObject", ecoreObjectFactory.EJavaObject},
                { "http://www.eclipse.org/emf/2002/Ecore#//ELong", ecoreObjectFactory.ELong},
                { "http://www.eclipse.org/emf/2002/Ecore#//ELongObject", ecoreObjectFactory.ELongObject},
                { "http://www.eclipse.org/emf/2002/Ecore#//EMap", ecoreObjectFactory.EMap},
                { "http://www.eclipse.org/emf/2002/Ecore#//EResource", ecoreObjectFactory.EResource},
                { "http://www.eclipse.org/emf/2002/Ecore#//EResourceSet", ecoreObjectFactory.EResourceSet},
                { "http://www.eclipse.org/emf/2002/Ecore#//EShort", ecoreObjectFactory.EShort},
                { "http://www.eclipse.org/emf/2002/Ecore#//EShortObject", ecoreObjectFactory.EShortObject},
                { "http://www.eclipse.org/emf/2002/Ecore#//EString", ecoreObjectFactory.EString},
                { "http://www.eclipse.org/emf/2002/Ecore#//ETreeIterator", ecoreObjectFactory.ETreeIterator},
                { "http://www.eclipse.org/emf/2002/Ecore#//EInvocationTargetException", ecoreObjectFactory.EInvocationTargetException}
            };

            this.Cache = new Dictionary<string, EObject>();

            this.isLoaded = false;

            this.Contents = new List<EObject>();
            this.errors = new List<Diagnostic>();
            this.warnings = new List<Diagnostic>();
        }
        
        /// <summary>
        /// Gets the containing resource set. A resource is contained by a resource set if it appears in the resources, i.e., the contents, of that resource set.
        /// This reference can only be modified by altering the contents of the resource set directly. 
        /// </summary>
        public ResourceSet ResourceSet { get; internal set; }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> of this resource. The URI is normally expected to be absolute and hierarchical; document-relative references will not be serialized and will not be resolved, if this is not the case.
        /// </summary>
        public Uri URI { get; set; }

        /// <summary>
        /// Gets or sets the cached value of the time stamp when this resource was last loaded or saved, or NULL_TIME_STAMP if the resource is not 
        /// loaded and the time stamp has not been set. The return value is represented as the number of milliseconds since the epoch (00:00:00 GMT, January 1, 1970). 
        /// The returned value may not be the same as the actual time stamp if the resource has been modified via external means since the last load or save.
        /// </summary>
        public long TimeStamp { get; set; }

        /// <summary>
        /// Gets the list of the direct content objects; each is of type <see cref="EObject"/>. 
        /// </summary>
        /// <remarks>
        /// The contents may be directly modified. Removing an object will have the same effect as EcoreUtil.remove(EObject). Adding an object will remove it from the previous container; 
        /// it's container will be null and it's resource will the this. 
        /// </remarks>
        public List<EObject> Contents { get; private set; }

        /// <summary>
        /// Gets or sets the Cache containing all parsed <see cref="EObject"/>
        /// </summary>
        internal Dictionary<string, EObject> Cache { get; set; }

        /// <summary>
        /// Returns a tree iterator that iterates over all the direct contents and indirect contents of this resource.
        /// </summary>
        /// <returns>
        /// a tree iterator that iterates over all contents.
        /// </returns>
        public IEnumerable<EObject> AllContents()
        {
            return this.Cache.Values;
        }

        /// <summary>
        /// Returns the URI fragment that, when passed to getEObject will return the given object. 
        /// </summary>
        /// <param name="eObject">
        /// The object to identify
        /// </param>
        /// <returns>
        /// the URI fragment for the object.
        /// </returns>
        public string GetURIFragment(EObject eObject)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the resolved object for the given URI fragment. 
        /// </summary>
        /// <remarks>
        /// The fragment encoding will typically be that produced by getURIFragment. 
        /// </remarks>
        /// <param name="uriFragment">
        /// the fragment to resolve.
        /// </param>
        /// <returns>
        /// The resolved object for the given fragment, or null if it can't be resolved.
        /// </returns>
        public EObject GetEObject(string uriFragment)
        {
            this.logger.LogTrace("Getting EObject for resources {0}", uriFragment);

            if (string.IsNullOrWhiteSpace(uriFragment))
            {
                throw new ArgumentException("The uri cannot be null or empty", nameof(uriFragment));
            }

            if (this.Cache.TryGetValue(uriFragment, out var @object))
            {
                this.logger.LogTrace("EObject using uri fragment '{0}' found in cache", uriFragment);

                return @object;
            }

            var ecoreType = this.eCoreTypes.SingleOrDefault(x => uriFragment.Contains(x.Key));
            if (ecoreType.Value != null)
            {
                this.logger.LogTrace("EObject using Key: '{0}' found in found in ECore Types", ecoreType.Key);

                return ecoreType.Value;
            }

            // load another resource
            // parse uri
            var uriFragments = uriFragment.Split('#');
            if (!uriFragments[0].Contains(".ecore"))
            {
                throw new ArgumentException($"The resource {uriFragments[0]} is invalid.");
            }

            var index = this.URI.AbsolutePath.LastIndexOf('/');
            if (index <= 0)
            {
                throw new ArgumentException($"Invalid path for the current resource: {this.URI.AbsolutePath}");
            }

            var resourceUri = new Uri($"{this.URI.AbsolutePath.Substring(0, index)}/{uriFragments[0]}");

            this.logger.LogTrace("EObject not found in current resource, loading other resources: {0}", resourceUri);

            var resource = this.ResourceSet.Resources.SingleOrDefault(x => x.URI == resourceUri);
            if (resource == null)
            {
                resource = this.ResourceSet.CreateResource(resourceUri);
                resource.Load(null);
            }

            return resource.GetEObject(uriFragment);
        }

        /// <summary>
        /// Saves the resource using the specified options. 
        /// </summary>
        /// <remarks>
        /// Options are handled generically as feature-to-setting entries; the resource will ignore options it doesn't recognize.
        /// </remarks>
        /// <param name="options">
        /// The save options.
        /// </param>
        public void Save(Dictionary<object, object> options)
        {
            this.logger.LogWarning("Saving an Ecore model to file is not yet supported");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the resource using the specified options. 
        /// </summary>
        /// <remarks>
        /// Options are handled generically as feature-to-setting entries; the resource will ignore options it doesn't recognize.
        /// </remarks>
        /// <param name="options">
        /// The load options
        /// </param>
        /// <returns>
        /// The top-level <see cref="EPackage"/> contained by the resource.
        /// </returns>
        public EPackage Load(Dictionary<object, object> options)
        {
            var sw = Stopwatch.StartNew();

            var parser = new ECoreParser(this, this.loggerFactory);
            var package = parser.ParseXml();
            
            this.isLoaded = true;

            this.logger.LogInformation("Package: '{0}' with prefix {1} and uri {2} loaded in {3} [ms]", 
                package.Name, package.NsPrefix, package.NsUri, sw.ElapsedMilliseconds);

            return package;
        }

        /// <summary>
        /// Gets or sets a value indicating whether modification tracking is enabled. 
        /// </summary>
        /// <remarks>
        /// If modification tracking is enabled, each object of the resource must be adapted in order to listen for changes. This will make the processing of attached and detached significantly more expensive. as well as all model editing, in general
        /// </remarks>
        public bool IsTrackingModification { get; set; }

        /// <summary>
        /// Returns whether the resource is loaded. 
        /// </summary>
        /// <returns>
        /// <remarks>
        /// This will be false when the resource is first created and will be set to false, when the resource
        /// is unloaded. It will be set to true when the resource is loaded and when contents are first added
        /// to a resource that isn't loaded. Calling clear for the contents of a resource that isn't loaded,
        /// will set the resource to be loaded; this is the simplest way to create an empty resource that's
        /// considered loaded
        /// </remarks>
        /// whether the resource is loaded.
        /// </returns>
        public bool IsLoaded()
        {
            return this.isLoaded;
        }

        /// <summary>
        /// Clears the contents, errors, and warnings of the resource and marks it as unloaded. 
        /// </summary>
        /// <remarks>
        /// It walks the content tree, and sets each content object to be a proxy. The resource will remain in the resource set, and can be subsequently reloaded. 
        /// </remarks>
        public void UnLoad()
        {
            this.Contents.Clear();
            this.warnings.Clear();
            this.errors.Clear();
            this.isLoaded = false;
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{String}"/> of the errors in the resource;
        /// </summary>
        /// <remarks>
        /// These will typically be produced as the resource is loaded. 
        /// </remarks>
        public IEnumerable<Diagnostic> Errors => this.errors;
        
        /// <summary>
        /// Gets an <see cref="IEnumerable{String}"/> of the warnings in the resource;
        /// </summary>
        /// <remarks>
        /// These will typically be produced as the resource is loaded. 
        /// </remarks>
        public IEnumerable<Diagnostic> Warnings => this.warnings;
        
    }
}
