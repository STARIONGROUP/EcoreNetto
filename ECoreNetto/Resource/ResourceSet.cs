// -------------------------------------------------------------------------------------------------
// <copyright file="ResourceSet.cs" company="Starion Group S.A.">
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

namespace ECoreNetto.Resource
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    /// <summary>
    /// A collection of related persistent documents. 
    /// </summary>
    /// <remarks>
    /// A resource set manages a collection of related resources and produces notification for changes to that collection. 
    /// It provides a tree of contents. A collection of adapter factories supports adapter lookup via registered adapter factory.
    /// A resource can be created or demand loaded into the collection. The registry of resource factories can be configured to create resources 
    /// of the appropriate type. A proxy can be resolved by the resource set, and may cause the demand load of a resource. 
    /// Default load options are used during demand load. A URI converter can be configured to normalize URIs for comparison and to 
    /// monitor access to the backing store. Clients must extend the default implementation, since methods can and will be added to this API. 
    /// </remarks>
    public class ResourceSet : Notifier
    {
        /// <summary>
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </summary>
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// The <see cref="ILogger"/> used to log
        /// </summary>
        private readonly ILogger<ResourceSet> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSet"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to set up logging
        /// </param>
        public ResourceSet(ILoggerFactory loggerFactory = null)
        {
            this.loggerFactory = loggerFactory;

            this.logger = this.loggerFactory == null ? NullLogger<ResourceSet>.Instance : this.loggerFactory.CreateLogger<ResourceSet>();

            this.Resources = new List<Resource>();
        }

        /// <summary>
        /// Gets the direct Resources being managed.
        /// </summary>
        /// <remarks>
        /// A resource added to this list will be contained by this resource set. If it was previously contained by a resource set, it will have been removed.
        /// </remarks>
        /// <returns>
        /// the resources.
        /// </returns>
        public List<Resource> Resources { get; private set; }

        /// <summary>
        /// Creates a new resource, of the appropriate type, and returns it.
        /// </summary>
        /// <param name="uri">
        /// the <see cref="Uri"/> of the resource to create.
        /// </param>
        /// <returns>
        /// a new resource
        /// </returns>
        public Resource CreateResource(Uri uri)
        {
            this.logger.LogInformation("Creating resource for uri: {0}", uri);

            var resource = new Resource(this.loggerFactory)
            {
                URI = uri
            };
            
            this.Resources.Add(resource);
            resource.ResourceSet = this;
            return resource;
        }
        
        /// <summary>
        /// Returns a tree iterator that iterates over all the direct resources and over the content tree of each.
        /// </summary>
        /// <returns>
        /// a tree iterator that iterates over all contents.
        /// </returns>
        public IEnumerable<Notifier> AllContents()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the object resolved by the URI.
        /// </summary>
        /// <param name="uri">
        /// The URI to resolve.
        /// </param>
        /// <param name="loadOnDemand">
        /// whether to create and load the resource, if it doesn't already exists.
        /// </param>
        /// <returns>
        /// the object resolved by the URI, or null if there isn't one.
        /// </returns>
        public EObject EObject(Uri uri, bool loadOnDemand)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the resource resolved by the URI.
        /// </summary>
        /// <param name="uri">
        /// the URI to resolve.
        /// </param>
        /// <param name="loadOnDemand">
        /// whether to create and load the resource, if it doesn't already exist.
        /// </param>
        /// <returns>
        /// the resource resolved by the URI, or null if there isn't one, and it's not being demand loaded.
        /// </returns>
        public Resource Resource(Uri uri, bool loadOnDemand)
        {
            foreach (var resource in this.Resources)
            {
                if (resource.URI.AbsoluteUri == uri.AbsoluteUri)
                {
                    return resource;
                }
            }

            return null;
        }
    }
}
