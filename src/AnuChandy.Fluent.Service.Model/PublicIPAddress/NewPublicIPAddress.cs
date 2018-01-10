// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Newtonsoft.Json;
using System;

namespace AnuChandy.Fluent.Service.Model.PublicIPAddress
{
    public class NewPublicIPAddress : NewResource<PublicIPAddressModel, IPublicIPAddress>
    {
        [JsonProperty(PropertyName = "leafDomainLabel")]
        public String LeafDomainLabel { get; set; }

        public override void Validate(String propertyName)
        {
            if (this.LeafDomainLabel == null)
            {
                base.Validate(propertyName);
                if (this.Reference == null)
                {
                    throw new ArgumentException($"{propertyName} specified but required {propertyName}.leafDomainLabel is missing");
                }
            }
        }

        public override void ResolveInlineCreatable(IAzure azure, IGroupableModel parentModel)
        {
            if (this.creatable == null)
            {
                if (this.LeafDomainLabel != null)
                {
                    Microsoft.Azure.Management.Network.Fluent.PublicIPAddress.Definition.IWithGroup withGroup;
                    withGroup = azure.PublicIPAddresses.Define(SdkContext.RandomResourceName("pip", 15))
                        .WithRegion(parentModel.Location());

                    Microsoft.Azure.Management.Network.Fluent.PublicIPAddress.Definition.IWithCreate withCreate;
                    if (parentModel.CreatableResourceGroup() != null)
                    {
                        withCreate = withCreate = withGroup.WithNewResourceGroup(parentModel.CreatableResourceGroup());
                    }
                    else if (parentModel.ResourceGroup() != null)
                    {
                        withCreate = withGroup.WithExistingResourceGroup(parentModel.ResourceGroup());
                    }
                    else
                    {
                        withCreate = withGroup.WithNewResourceGroup();
                    }
                    withCreate = withCreate
                        .WithLeafDomainLabel(this.LeafDomainLabel);

                    base.creatable = withCreate;
                }
            }
        }

        protected override CreatableModels<PublicIPAddressModel, IPublicIPAddress> CreatableModels(FluentRequestModel fluentRequestModel)
        {
            return fluentRequestModel.PublicIPAddressModels;
        }

        protected override string ReferencePath()
        {
            return "newPublicIpAddress.ref";
        }

        protected override string ReferencePrefix()
        {
            return "publicIpAddresses";
        }
    }
}
