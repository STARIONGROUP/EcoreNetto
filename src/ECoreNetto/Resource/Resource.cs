// -------------------------------------------------------------------------------------------------
// <copyright file="Resource.cs" company="RHEA System S.A.">
//
//   Copyright 2017-2020 RHEA System S.A.
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
    using System.Linq;
    using Utils;

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
        private readonly Dictionary<string, EObject> ECoreTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        public Resource()
        {
            var ecoreInstantiator = new EcoreObjectInstantiator(this);

            this.ECoreTypes = new Dictionary<string, EObject>
            {
                { "//EObject", ecoreInstantiator.EObject },
                { "//EModelElement", ecoreInstantiator.EModelElement },
                { "//ENamedElement", ecoreInstantiator.ENamedElement },
                { "//EFactory", ecoreInstantiator.EFactory },
                { "//EAnnotation", ecoreInstantiator.EAnnotation },
                { "//EClassifier", ecoreInstantiator.EClassifier },
                { "//EEnumLiteral", ecoreInstantiator.EEnumLiteral },
                { "//EPackage", ecoreInstantiator.EPackage },
                { "//ETypedElement", ecoreInstantiator.ETypedElement },
                { "//EClass", ecoreInstantiator.EClass },
                { "//EDataType", ecoreInstantiator.EDataType },
                { "//EEnum", ecoreInstantiator.EEnum },
                { "//EOperation", ecoreInstantiator.EOperation },
                { "//EParameter", ecoreInstantiator.EParameter },
                { "//EStructuralFeature", ecoreInstantiator.EStructuralFeature },
                { "//EAttribute", ecoreInstantiator.EAttribute },
                { "//EReference", ecoreInstantiator.EReference }
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
            if (string.IsNullOrWhiteSpace(uriFragment))
            {
                throw new ArgumentException("The uri cannot be null or empty", "uriFragment");
            }

            if (this.Cache.TryGetValue(uriFragment, out var @object))
            {
                return @object;
            }

            var ecoreType = this.ECoreTypes.SingleOrDefault(x => uriFragment.Contains(x.Key));
            if (ecoreType.Value != null)
            {
                return ecoreType.Value;
            }

            // load another resource
            // parse uri
            var urifragments = uriFragment.Split('#');
            if (!urifragments[0].Contains(".ecore"))
            {
                throw new ArgumentException($"The resource {urifragments[0]} is invalid.");
            }

            var index = this.URI.AbsolutePath.LastIndexOf('/');
            if (index <= 0)
            {
                throw new ArgumentException($"Invalid path for the current resource: {this.URI.AbsolutePath}");
            }

            var resourceUri = new Uri($"{this.URI.AbsolutePath.Substring(0, index)}/{urifragments[0]}");

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
            var parser = new ECoreParser(this);
            var pacakge = parser.ParseXml();
            
            this.isLoaded = true;

            return pacakge;
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
        /// This will be false when the resource is first created and will be set to false, when the resource is unloaded. It will be set to true when the resource is loaded and when contents are first added to a resource that isn't loaded. Calling clear for the contents of a resource that isn't loaded, will set the resource to be loaded; this is the simplest way to create an empty resource that's considered loaded
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
        public IEnumerable<Diagnostic> Errors
        {
            get
            {
                return this.errors;
            }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{String}"/> of the warnings in the resource;
        /// </summary>
        /// <remarks>
        /// These will typically be produced as the resource is loaded. 
        /// </remarks>
        public IEnumerable<Diagnostic> Warnings
        {
            get
            {
                return this.warnings;
            }
        }
    }
}
