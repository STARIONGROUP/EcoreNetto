// ------------------------------------------------------------------------------------------------
// <copyright file="Resource.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------

namespace ECoreNetto.Resource
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
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
        private readonly ILoggerFactory? loggerFactory;

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
        /// Maps the intrinsic <c>xmi:id</c> of an <see cref="EObject"/> to that object, populated while the
        /// resource is read. It backs resolution of the bare <c>xmi:id</c> URI-fragment form (a fragment
        /// without a leading slash), which EMF resolves through <c>getEObjectByID</c>.
        /// </summary>
        private readonly Dictionary<string, EObject> idToEObject;

        /// <summary>
        /// The namespace URI prefix under which the built-in Ecore types are referenced, i.e. the part that
        /// precedes the <c>//EName</c> fragment in a fully-qualified reference such as
        /// <c>http://www.eclipse.org/emf/2002/Ecore#//EString</c>.
        /// </summary>
        private const string EcoreNamespacePrefix = "http://www.eclipse.org/emf/2002/Ecore#";

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public Resource(ILoggerFactory? loggerFactory = null)
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
                
                { "//EBigDecimal", ecoreObjectFactory.EBigDecimal},
                { "//EBigInteger", ecoreObjectFactory.EBigInteger},
                { "//EBoolean", ecoreObjectFactory.EBoolean},
                { "//EBooleanObject", ecoreObjectFactory.EBooleanObject},
                { "//EByte", ecoreObjectFactory.EByte},
                { "//EByteArray", ecoreObjectFactory.EByteArray},
                { "//EByteObject", ecoreObjectFactory.EByteObject},
                { "//EChar", ecoreObjectFactory.EChar},
                { "//ECharacterObject", ecoreObjectFactory.ECharacterObject},
                { "//EDate", ecoreObjectFactory.EDate},
                { "//EDiagnosticChain", ecoreObjectFactory.EDiagnosticChain},
                { "//EDouble", ecoreObjectFactory.EDouble},
                { "//EDoubleObject", ecoreObjectFactory.EDoubleObject},
                { "//EEList", ecoreObjectFactory.EEList},
                { "//EEnumerator", ecoreObjectFactory.EEnumerator},
                { "//EFeatureMap", ecoreObjectFactory.EFeatureMap},
                { "//EFeatureMapEntry", ecoreObjectFactory.EFeatureMapEntry},
                { "//EFloat", ecoreObjectFactory.EFloat},
                { "//EFloatObject", ecoreObjectFactory.EFloatObject},
                { "//EInt", ecoreObjectFactory.EInt},
                { "//EIntegerObject", ecoreObjectFactory.EIntegerObject},
                { "//EJavaClass", ecoreObjectFactory.EJavaClass},
                { "//EJavaObject", ecoreObjectFactory.EJavaObject},
                { "//ELong", ecoreObjectFactory.ELong},
                { "//ELongObject", ecoreObjectFactory.ELongObject},
                { "//EMap", ecoreObjectFactory.EMap},
                { "//EResource", ecoreObjectFactory.EResource},
                { "//EResourceSet", ecoreObjectFactory.EResourceSet},
                { "//EShort", ecoreObjectFactory.EShort},
                { "//EShortObject", ecoreObjectFactory.EShortObject},
                { "//EString", ecoreObjectFactory.EString},
                { "//ETreeIterator", ecoreObjectFactory.ETreeIterator},
                { "//EInvocationTargetException", ecoreObjectFactory.EInvocationTargetException}
            };

            // register the XMLType data types (keyed by their fully-qualified nsURI reference) so that
            // references into the 'http://www.eclipse.org/emf/2003/XMLType' namespace resolve without a
            // backing file, the way EMF resolves them through its package registry.
            foreach (var xmlType in XmlTypeObjectFactory.CreateDataTypes(this, this.loggerFactory))
            {
                this.eCoreTypes.Add(xmlType.Key, xmlType.Value);
            }

            this.Cache = new Dictionary<string, EObject>();
            this.idToEObject = new Dictionary<string, EObject>();

            this.isLoaded = false;

            this.Contents = new List<EObject>();
            this.errors = new List<Diagnostic>();
            this.warnings = new List<Diagnostic>();
        }
        
        /// <summary>
        /// Gets the containing resource set. A resource is contained by a resource set if it appears in the resources, i.e., the contents, of that resource set.
        /// This reference can only be modified by altering the contents of the resource set directly. 
        /// </summary>
        public ResourceSet? ResourceSet { get; internal set; }

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> of this resource. The URI is normally expected to be absolute and hierarchical; document-relative references will not be serialized and will not be resolved, if this is not the case.
        /// </summary>
        public Uri URI { get; set; } = null!;

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
        /// Gets the Ecore meta class registered for the provided simple type name.
        /// </summary>
        /// <param name="name">
        /// The simple name of the meta class (e.g. <c>EClass</c>, <c>EAttribute</c>), which matches the
        /// runtime type name of the model elements.
        /// </param>
        /// <returns>
        /// The <see cref="EClass"/> meta class, or null when no meta class is registered for <paramref name="name"/>.
        /// </returns>
        internal EClass? GetMetaClass(string name)
        {
            if (this.eCoreTypes.TryGetValue($"//{name}", out var metaClass) && metaClass is EClass eClass)
            {
                return eClass;
            }

            return null;
        }

        /// <summary>
        /// Returns the URI fragment that, when passed to <see cref="GetEObject(string)"/>, will return the given object.
        /// </summary>
        /// <param name="eObject">
        /// The object to identify
        /// </param>
        /// <returns>
        /// the URI fragment for the object.
        /// </returns>
        /// <remarks>
        /// The returned fragment is the name-based Ecore reference under which the object is registered in this
        /// resource (its <see cref="EObject.Identifier"/>), for example <c>recipe.ecore#//Recipe</c> for a class or
        /// <c>EStructuralFeature::recipe.ecore#//Recipe/ingredients</c> for a structural feature. This is exactly the
        /// reference string consumed by <see cref="GetEObject(string)"/>, so the round-trip
        /// <c>GetEObject(GetURIFragment(eObject))</c> returns the same instance.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="eObject"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="eObject"/> is not contained in this resource and therefore cannot be turned
        /// into a resolvable URI fragment.
        /// </exception>
        public string GetURIFragment(EObject eObject)
        {
            if (eObject == null)
            {
                throw new ArgumentNullException(nameof(eObject));
            }

            var fragment = eObject.Identifier;

            if (string.IsNullOrEmpty(fragment) || !this.Cache.TryGetValue(fragment, out var cached) || !ReferenceEquals(cached, eObject))
            {
                throw new InvalidOperationException(
                    $"The provided '{eObject.GetType().Name}' is not contained in this resource and cannot be turned into a URI fragment.");
            }

            return fragment;
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
        public EObject? GetEObject(string uriFragment)
        {
            return this.GetEObject(uriFragment, new HashSet<Resource>());
        }

        /// <summary>
        /// Returns the resolved object for the given URI fragment, tracking the resources already consulted
        /// during the current resolution so that resolution always terminates.
        /// </summary>
        /// <param name="uriFragment">
        /// the fragment to resolve.
        /// </param>
        /// <param name="visitedResources">
        /// the resources already visited while resolving <paramref name="uriFragment"/>. A delegation to a
        /// resource that is already present would repeat with the same (resource, fragment) pair and never
        /// make progress, so it is reported as unresolved rather than recursed into.
        /// </param>
        /// <returns>
        /// The resolved object for the given fragment, or null if it can't be resolved.
        /// </returns>
        private EObject? GetEObject(string uriFragment, HashSet<Resource> visitedResources)
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

            // resolve a built-in Ecore type by exact key. A reference to such a type is either a bare
            // fragment (the type name preceded by a double slash) or the same fragment prefixed with the
            // Ecore namespace URI. Stripping that namespace prefix lets one exact lookup cover both forms.
            // The previous substring match mis-resolved the boolean type and threw on type names that share
            // a prefix, for example EClass versus EClassifier, or EString versus EStringToStringMapEntry.
            var ecoreTypeKey = uriFragment.StartsWith(EcoreNamespacePrefix, StringComparison.Ordinal)
                ? uriFragment.Substring(EcoreNamespacePrefix.Length)
                : uriFragment;

            if (this.eCoreTypes.TryGetValue(ecoreTypeKey, out var ecoreType))
            {
                this.logger.LogTrace("EObject using Key: '{0}' found in ECore Types", ecoreTypeKey);

                return ecoreType;
            }

            // load another resource
            // parse uri
            var uriFragments = uriFragment.Split('#');

            // the file part may carry a type prefix (e.g. 'EStructuralFeature::CompositeStructure.ecore' for an
            // eOpposite reference); strip it so only the '.ecore' file name is used to locate the sibling resource.
            var filePart = uriFragments[0];
            var typePrefixIndex = filePart.LastIndexOf("::", StringComparison.Ordinal);
            if (typePrefixIndex >= 0)
            {
                filePart = filePart.Substring(typePrefixIndex + 2);
            }

            // the part after '#' is the in-document fragment: a name-based path ('//A/b'), a positional path
            // ('//@eClassifiers.3'), or a bare xmi:id. When there is no '#', the whole value may itself be a
            // bare xmi:id.
            var inDocumentFragment = uriFragments.Length > 1 ? uriFragments[1] : filePart;

            if (!Path.HasExtension(filePart))
            {
                // the fragment does not point at another resource file: resolve a positional path or an
                // xmi:id within this resource, otherwise it cannot be resolved.
                var localResolved = this.ResolveInDocumentFragment(inDocumentFragment);
                if (localResolved == null)
                {
                    this.logger.LogTrace("EObject using uri fragment '{0}' could not be resolved", uriFragment);
                }

                return localResolved;
            }

            // resolve the file part against the current resource URI (proper URI resolution handles escaped
            // path segments and any file extension without hardcoded string manipulation)
            var resourceUri = new Uri(this.URI, filePart);

            this.logger.LogTrace("EObject not found in current resource, loading other resources: {0}", resourceUri);

            // record that resolution has now passed through this resource
            visitedResources.Add(this);

            var resource = this.ResourceSet!.Resources.SingleOrDefault(x => x.URI == resourceUri);
            if (resource == null)
            {
                if (!File.Exists(resourceUri.LocalPath))
                {
                    // the referenced '.ecore' resource does not exist: record a descriptive diagnostic
                    // and report the reference as unresolved rather than throwing a raw FileNotFoundException
                    var message = $"The reference '{uriFragment}' points at resource '{resourceUri.LocalPath}' which could not be found.";
                    this.AddError(message);
                    this.logger.LogTrace(message);

                    return null;
                }

                resource = this.ResourceSet.CreateResource(resourceUri);
                resource.Load(null);
            }

            // a positional path or an xmi:id is resolved structurally within the owning resource. That owning
            // resource is this resource itself for a same-file positional reference such as
            // 'file.ecore#//@eClassifiers.3', which carries the file name after reference rewriting.
            var withinResource = resource.ResolveInDocumentFragment(inDocumentFragment);
            if (withinResource != null)
            {
                return withinResource;
            }

            if (visitedResources.Contains(resource))
            {
                // delegating to a resource already on the resolution path would repeat with the same
                // (resource, fragment) pair and never terminate; report the reference as unresolved
                // instead of recursing unboundedly.
                var message = $"The reference '{uriFragment}' could not be resolved; resolution cycled back to resource '{resource.URI}'.";
                this.AddError(message);
                this.logger.LogTrace(message);

                return null;
            }

            return resource.GetEObject(uriFragment, visitedResources);
        }

        /// <summary>
        /// Resolves the given <paramref name="uriFragment"/> and returns it typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The expected <see cref="EObject"/> subtype of the resolved object.
        /// </typeparam>
        /// <param name="uriFragment">
        /// the fragment to resolve.
        /// </param>
        /// <returns>
        /// The resolved object, typed as <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the <paramref name="uriFragment"/> cannot be resolved, or resolves to an object
        /// that is not a <typeparamref name="T"/>. The exception message names the offending fragment.
        /// </exception>
        public T GetEObject<T>(string uriFragment) where T : EObject
        {
            var eObject = this.GetEObject(uriFragment);

            if (eObject is not T typedEObject)
            {
                throw new InvalidOperationException(
                    $"The reference '{uriFragment}' could not be resolved to a '{typeof(T).Name}'; " +
                    $"it resolved to '{(eObject == null ? "null (unresolved)" : eObject.GetType().Name)}'.");
            }

            return typedEObject;
        }

        /// <summary>
        /// Resolves an in-document URI fragment against this resource: a positional path of the
        /// <c>//@feature.index</c> form, or a bare <c>xmi:id</c>.
        /// </summary>
        /// <param name="fragment">
        /// The in-document fragment (the part after '#', or the whole value when there is no '#').
        /// </param>
        /// <returns>
        /// The resolved <see cref="EObject"/>, or null when the fragment is not a positional path or a
        /// known <c>xmi:id</c>. Pure name-based paths return null here because they are already resolved
        /// through the identifier cache.
        /// </returns>
        private EObject? ResolveInDocumentFragment(string fragment)
        {
            if (string.IsNullOrEmpty(fragment))
            {
                return null;
            }

            if (fragment[0] == '/')
            {
                // a '/'-rooted path. Pure name paths are already covered by the identifier cache; only the
                // positional '@feature.index' form needs structural navigation here.
                return fragment.IndexOf('@') >= 0 ? this.ResolveStructuralFragment(fragment) : null;
            }

            // a fragment without a leading slash is an intrinsic xmi:id
            return this.idToEObject.TryGetValue(fragment, out var byId) ? byId : null;
        }

        /// <summary>
        /// Navigates a positional (structural) URI fragment such as
        /// <c>//@eClassifiers.3/@eStructuralFeatures.1</c> from the root of this resource, indexing into the
        /// containment feature named by each <c>@feature.index</c> segment. This mirrors EMF's
        /// <c>eObjectForURIFragmentSegment</c>.
        /// </summary>
        /// <param name="fragment">
        /// The positional path, starting with a leading slash.
        /// </param>
        /// <returns>
        /// The resolved <see cref="EObject"/>, or null when the path does not address an existing object.
        /// </returns>
        private EObject? ResolveStructuralFragment(string fragment)
        {
            // 'fragment' has a leading slash: split yields an empty first element (before that slash), then the
            // resource root-position segment, then the '@feature.index' navigation segments.
            var parts = fragment.Split('/');
            if (parts.Length < 2)
            {
                return null;
            }

            var rootSegment = parts[1];
            if (rootSegment.Length != 0 && rootSegment != "0")
            {
                // a non-zero resource root position addresses a second root object; ECoreNetto resources are
                // single-root, so this cannot be resolved.
                return null;
            }

            var current = this.QueryRootObject();

            for (var i = 2; i < parts.Length && current != null; i++)
            {
                current = NavigateContainmentSegment(current, parts[i]);
            }

            return current;
        }

        /// <summary>
        /// Returns the root object of this resource, i.e. the object at position 0 of its contents.
        /// </summary>
        /// <returns>
        /// The root <see cref="EObject"/>, or null when the resource has no contents.
        /// </returns>
        /// <remarks>
        /// While the resource is still being loaded its <see cref="Contents"/> are not yet populated, so the
        /// root is taken as the single cached object that has no container.
        /// </remarks>
        private EObject? QueryRootObject()
        {
            if (this.Contents.Count > 0)
            {
                return this.Contents[0];
            }

            return this.Cache.Values.FirstOrDefault(o => o.EContainer == null);
        }

        /// <summary>
        /// Resolves a single positional navigation segment of the <c>@feature.index</c> form against the
        /// given owner, returning the child at that index of the named containment feature.
        /// </summary>
        /// <param name="owner">
        /// The object the segment is navigated from.
        /// </param>
        /// <param name="segment">
        /// The <c>@feature.index</c> segment.
        /// </param>
        /// <returns>
        /// The addressed child, or null when the segment is malformed, names an unknown containment feature,
        /// or the index is out of range.
        /// </returns>
        private static EObject? NavigateContainmentSegment(EObject owner, string segment)
        {
            if (segment.Length == 0 || segment[0] != '@')
            {
                return null;
            }

            var body = segment.Substring(1);
            var separator = body.LastIndexOf('.');
            if (separator < 0)
            {
                return null;
            }

            var featureName = body.Substring(0, separator);
            if (!int.TryParse(body.Substring(separator + 1), out var index) || index < 0)
            {
                return null;
            }

            var containment = QueryContainmentList(owner, featureName);
            if (containment == null || index >= containment.Count)
            {
                return null;
            }

            return containment[index];
        }

        /// <summary>
        /// Returns the ordered containment list of <paramref name="owner"/> for the containment feature named
        /// <paramref name="featureName"/>, or null when the owner has no such containment feature.
        /// </summary>
        /// <param name="owner">
        /// The object whose containment feature is requested.
        /// </param>
        /// <param name="featureName">
        /// The Ecore name of the containment feature (e.g. <c>eClassifiers</c>, <c>eStructuralFeatures</c>).
        /// </param>
        /// <returns>
        /// The containment list, or null.
        /// </returns>
        private static IReadOnlyList<EObject>? QueryContainmentList(EObject owner, string featureName)
        {
            switch (featureName)
            {
                case "eAnnotations":
                    return (owner as EModelElement)?.EAnnotations;
                case "eSubpackages":
                    return (owner as EPackage)?.ESubPackages;
                case "eClassifiers":
                    return (owner as EPackage)?.EClassifiers;
                case "eStructuralFeatures":
                    return (owner as EClass)?.EStructuralFeatures;
                case "eOperations":
                    return (owner as EClass)?.EOperations;
                case "eGenericSuperTypes":
                    return (owner as EClass)?.EGenericSuperTypes;
                case "eLiterals":
                    return (owner as EEnum)?.ELiterals;
                case "eParameters":
                    return (owner as EOperation)?.EParameters;
                case "eGenericExceptions":
                    return (owner as EOperation)?.EGenericExceptions;
                case "eTypeParameters":
                    return (owner as EClassifier)?.ETypeParameters ?? (owner as EOperation)?.ETypeParameters;
                case "eBounds":
                    return (owner as ETypeParameter)?.EBounds;
                default:
                    return null;
            }
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
        public void Save(Dictionary<object, object>? options)
        {
            this.logger.LogWarning("Saving an Ecore model to file is not yet supported");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the resource using the specified options. 
        /// </summary>
        /// <remarks>
        /// Options are handled generically as feature-to-setting entries; the resource will ignore options it doesn't recognize.
        /// When the load fails because the file is missing or malformed, a descriptive <see cref="Diagnostic"/> is
        /// recorded in <see cref="Errors"/> before the exception is surfaced.
        /// </remarks>
        /// <param name="options">
        /// The load options
        /// </param>
        /// <returns>
        /// The top-level <see cref="EPackage"/> contained by the resource.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// If the resource's file could not be found.
        /// </exception>
        /// <exception cref="System.Xml.XmlException">
        /// If the resource's file is not well-formed XML.
        /// </exception>
        public EPackage Load(Dictionary<object, object>? options)
        {
            if (this.isLoaded)
            {
                // the resource is already loaded; return its root package without re-parsing (loading twice
                // would duplicate cache keys). This makes create-then-load loops over a ResourceSet safe.
                return (EPackage)this.Contents[0];
            }

            var sw = Stopwatch.StartNew();

            var parser = new ECoreParser(this, this.loggerFactory);
            var package = parser.ParseXml();

            this.Contents.Add(package);
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
        /// Records an error <see cref="Diagnostic"/> that was encountered while loading the resource.
        /// </summary>
        /// <param name="message">
        /// The translated message describing the issue.
        /// </param>
        /// <remarks>
        /// The diagnostic is exposed through <see cref="Errors"/>. The source location is the URI of this
        /// resource; line and column are not available once parsing has reached the property-resolution phase.
        /// </remarks>
        internal void AddError(string message)
        {
            this.errors.Add(new Diagnostic(0, 0, this.URI?.AbsoluteUri ?? string.Empty, message));
        }

        /// <summary>
        /// Registers the given <see cref="EObject"/> under its intrinsic <c>xmi:id</c> so that a bare
        /// <c>xmi:id</c> URI fragment can be resolved to it.
        /// </summary>
        /// <param name="id">
        /// The <c>xmi:id</c> value carried by <paramref name="eObject"/>.
        /// </param>
        /// <param name="eObject">
        /// The object to register.
        /// </param>
        /// <remarks>
        /// The first registration for a given <paramref name="id"/> wins. A duplicate <c>xmi:id</c> is
        /// invalid, but recording it must not abort the load, so later duplicates are ignored.
        /// </remarks>
        internal void RegisterEObjectId(string id, EObject eObject)
        {
            if (!this.idToEObject.ContainsKey(id))
            {
                this.idToEObject.Add(id, eObject);
            }
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
