// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.CollectionActions;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Base type for a property in the request representing an existing Azure resource type that 
    /// belongs to a resource group.
    /// </summary>
    /// <typeparam name="FluentCollectionT">The azure collection type that exposes methods to retrieve the azure resource</typeparam>
    /// <typeparam name="FluentT">The fluent type of the azure resource</typeparam>
    public abstract class ExistingGroupableResource<FluentCollectionT, FluentT> 
        : ISupportsValidate, ISupportExistingResource<FluentT>
        where FluentCollectionT : ISupportsGettingById<FluentT>, ISupportsGettingByResourceGroup<FluentT>
    {
        [JsonProperty(PropertyName = "id")]
        public String Id { get; set; }

        [JsonProperty(PropertyName = "resourceGroupName")]
        public String ResourceGroupName { get; set; }

        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }

        #region ISupportsValidate

        public void Validate(String propertyName)
        {
            if (this.Id != null)
            {
                return;
            }

            if (this.ResourceGroupName == null || this.Name == null)
            {
                throw new ArgumentException($"{propertyName} specified then either {propertyName}.Id or ({propertyName}.ResourceGroupName and {propertyName}.Name) must be specified");
            }
        }

        #endregion

        #region ISupportExistingResource<U>

        [JsonIgnore]
        private FluentT resource;

        public FluentT GetResource()
        {
            if (this.resource == null)
            {
                throw new InvalidOperationException("Attempted to get a resource that is not yet resolved");
            }
            return this.resource;
        }

        abstract public Task ResolveResourceAsync(IAzure azure, CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        /// <summary>
        /// Resolves the azure resource this type instance represents.
        /// </summary>
        /// <param name="azureCollection">The azure collection</param>
        /// <param name="cancellationToken">The task cancellation token</param>
        /// <returns>The resolved resource</returns>
        protected async Task ResolveResourceAsync(FluentCollectionT azureCollection, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.resource == null)
            {
                if (this.Id != null)
                {
                    this.resource = await azureCollection
                        .GetByIdAsync(this.Id);
                    if (GetResource() == null)
                    {
                        throw new ArgumentException($"Resource with id {this.Id} cannot be resolved");
                    }
                }
                else
                {
                    this.resource = await azureCollection
                        .GetByResourceGroupAsync(this.ResourceGroupName, this.Name, cancellationToken);
                    if (GetResource() == null)
                    {
                        throw new ArgumentException($"Resource with group {this.ResourceGroupName} and name {this.Name} cannot be resolved");
                    }
                }
            }
        }
    }
}
