// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.ResourceGroup
{
    public class ExistingResourceGroup 
        : ISupportsValidate, ISupportExistingResource<IResourceGroup>
    {
        private IResourceGroup resource;

        public IResourceGroup GetResource()
        {
            return resource;
        }

        internal void SetResource(IResourceGroup value)
        {
            resource = value;
        }

        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }

        #region ISupportsValidate

        public void Validate(String propertyName)
        {
            if (this.Name == null)
            {
                throw new ArgumentException($"{propertyName} specified then {propertyName}.Name must be specified");
            }
        }

        #endregion

        #region ISupportResolveResource<U>

        public async Task ResolveResourceAsync(IAzure azure, CancellationToken cancellation = default(CancellationToken))
        {
            this.SetResource(await azure.ResourceGroups.GetByNameAsync(this.Name, cancellation));
            if (GetResource() == null)
            {
                throw new ArgumentException($"Resource group with name {this.Name} cannot be resolved");
            }
        }


        #endregion
    }
}
