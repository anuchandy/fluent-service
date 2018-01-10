// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.ResourceGroup
{
    public class ResourceGroupModel 
        : ISupportsToCreatable<IResourceGroup>, 
        ISupportsValidateAndResolve
    {
        [JsonIgnore]
        private ICreatable<IResourceGroup> creatable;

        [JsonProperty(PropertyName = "region")]
        public String Region { get; set; }
        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }
        [JsonProperty(PropertyName = "tags")]
        public Dictionary<String, String> Tags { get; set; }

        #region ISupportsToCreatable<IResourceGroup>

        public bool IsCreatableReady()
        {
            return this.creatable != null;
        }

        public ICreatable<IResourceGroup> ToCreatable(IAzure azure)
        {
            if (this.creatable == null)
            {
                String name = this.Name != null ? this.Name : SdkContext.RandomResourceName("rg", 15);
                var withRegion = azure.ResourceGroups.Define(name);
                Region region = this.Region != null ? Microsoft.Azure.Management.ResourceManager.Fluent.Core.Region.Create(this.Region) : Microsoft.Azure.Management.ResourceManager.Fluent.Core.Region.USEast2;
                var withCreate = withRegion.WithRegion(region);
                if (this.Tags != null)
                {
                    withCreate = withCreate.WithTags(this.Tags);
                }
                this.creatable = withCreate;
            }
            return this.creatable;
        }

        #endregion

        public Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.Region == null)
            {
                if (parentModel.Location() != null)
                {
                    this.Region = parentModel.Location().ToString();
                }
                else
                {
                    this.Region = Microsoft.Azure.Management.ResourceManager.Fluent.Core.Region.USEast2.ToString();
                }
            }
            //
            // This model has no "New{Resource}" properties on which Validation and Resolution
            // needs to be run
            //
            return Task.CompletedTask;
        }
    }
}
