// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.ResourceGroup;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Base type for models in the request representing a new azure resource type that requires
    /// a location and a resource group for it's existance.
    /// </summary>
    /// <typeparam name="AfterGroupT">The defintion stage the azure resource after setting resourcegroup</typeparam>
    /// <typeparam name="WithCreateT">The creatable stage of the azure resource defintion</typeparam>
    /// <typeparam name="FluentT">The fluent type of the azure resource</typeparam>
    public abstract class CreatableGroupableModel<AfterGroupT, WithCreateT, FluentT> 
        : TagsModel<WithCreateT>,
        IGroupableModel,
        ISupportsToCreatable<FluentT>,
        ISupportsValidateAndResolve
       where WithCreateT : Microsoft.Azure.Management.ResourceManager.Fluent.Core.Resource.Definition.IDefinitionWithTags<WithCreateT>
    {
        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }

        [JsonProperty(PropertyName = "region")]
        public String Region { get; set; }

        [JsonProperty(PropertyName = "newResourceGroup")]
        public NewResourceGroup NewResourceGroup { get; set; }

        [JsonProperty(PropertyName = "existingResourceGroup")]
        public ExistingResourceGroup ExistingResourceGroup { get; set; }

        #region ISupportsToCreatable<FluentT>

        [JsonIgnore]
        private ICreatable<FluentT> creatable;

        public bool IsCreatableReady()
        {
            return this.creatable != null;
        }

        public ICreatable<FluentT> ToCreatable(IAzure azure)
        {
            if (this.creatable == null)
            {
                this.creatable = this.ToCreatableIntern(azure);
            }
            return this.creatable;
        }

        #endregion

        #region ISupportsValidateAndResolve

        public abstract Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken));

        #endregion

        #region IGroupableModel

        public string Location()
        {
            return this.Region;
        }

        [JsonIgnore]
        private ICreatable<IResourceGroup> creatableResourceGroup;
        public ICreatable<IResourceGroup> CreatableResourceGroup()
        {
            return creatableResourceGroup;
        }

        [JsonIgnore]
        private IResourceGroup resourceGroup;
        public IResourceGroup ResourceGroup()
        {
            return resourceGroup;
        }

        #endregion

        protected async Task ValidateAndResolveResourceGroupAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.NewResourceGroup != null)
            {
                this.NewResourceGroup.Validate($"{propertyName}.newResourceGroup");
                this.NewResourceGroup.ResolveInlineCreatable(azure, parentModel);
                this.NewResourceGroup.ResolveCreatableReference(azure, fluentRequestModel);
                //
                this.creatableResourceGroup = this.NewResourceGroup.GetCreatable();
            }
            if (this.ExistingResourceGroup != null)
            {
                this.ExistingResourceGroup.Validate($"{propertyName}.existingResourceGroup");
                await this.ExistingResourceGroup.ResolveResourceAsync(azure, cancellationToken);
                //
                this.resourceGroup = this.ExistingResourceGroup.GetResource();
            }

            if (this.creatableResourceGroup == null && this.resourceGroup == null)
            {
                // if 'NewResourceGroup' and/or 'ExistingResourceGroup' is not set
                // in the request use parent model's resource group.
                //
                if (parentModel.CreatableResourceGroup() != null)
                {
                    this.creatableResourceGroup = parentModel.CreatableResourceGroup();
                }
                if (parentModel.ResourceGroup() != null)
                {
                    this.resourceGroup = parentModel.ResourceGroup();
                }
                if (this.creatableResourceGroup == null && this.resourceGroup == null)
                {
                    throw new InvalidOperationException("Unable to derive resource group locally or from parent model");
                }
            }
            if (this.Region == null)
            {
                this.Region = parentModel.Location();
                if (this.Region == null)
                {
                    throw new InvalidOperationException("Unable to derive region locally or from parent model");
                }
            }
        }

        protected AfterGroupT SetResourceGroupAndTags(Microsoft.Azure.Management.ResourceManager.Fluent.Core.GroupableResource.Definition.IWithGroup<AfterGroupT> withGroup)
        {
            AfterGroupT afterGroup;
            if (this.CreatableResourceGroup() != null)
            {
                afterGroup = withGroup.WithNewResourceGroup(this.CreatableResourceGroup());
            }
            else if (this.ResourceGroup() != null)
            {
                afterGroup = withGroup.WithExistingResourceGroup(this.ResourceGroup());
            }
            else
            {
                afterGroup = withGroup.WithNewResourceGroup();
            }
            return afterGroup;
        }

        protected String DeriveName(String prefix)
        {
            return this.Name ?? SdkContext.RandomResourceName(prefix, 15);
        }

        protected abstract ICreatable<FluentT> ToCreatableIntern(IAzure azure);
    }
}
