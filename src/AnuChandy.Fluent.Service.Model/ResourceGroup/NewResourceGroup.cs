// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Newtonsoft.Json;
using System;

namespace AnuChandy.Fluent.Service.Model.ResourceGroup
{
    public class NewResourceGroup : NewResource<ResourceGroupModel, IResourceGroup>
    {
        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }

        public override void Validate(String propertyName)
        {
            if (this.Name == null)
            {
                base.Validate(propertyName);
                if (this.Reference == null)
                {
                    throw new ArgumentException($"{propertyName} specified but required {propertyName}.name is missing");
                }
            }
        }

        public override void ResolveInlineCreatable(IAzure azure, IGroupableModel parentModel)
        {
            if (this.creatable == null)
            {
                if (this.Name != null)
                {
                    base.creatable = azure.ResourceGroups.Define(this.Name)
                        .WithRegion(parentModel.Location());
                }
            }
        }

        protected override CreatableModels<ResourceGroupModel, IResourceGroup> CreatableModels(FluentRequestModel fluentRequestModel)
        {
            return fluentRequestModel.ResourceGroupModels;
        }

        protected override string ReferencePath()
        {
            return "newResourceGroup.ref";
        }

        protected override string ReferencePrefix()
        {
            return "resourceGroups";
        }
    }
}
