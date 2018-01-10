// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Newtonsoft.Json;
using System;

namespace AnuChandy.Fluent.Service.Model.Network
{
    public class NewNetwork : NewResource<NetworkModel, INetwork>
    {
        [JsonProperty(PropertyName = "addressSpace")]
        public AddressSpaceAndSubnets AddressSpace { get; set; }

        public override void Validate(String propertyName)
        {
            if (this.AddressSpace == null)
            {
                base.Validate(propertyName);
            }
            else
            {
                if (this.AddressSpace.Cidr == null)
                {
                    throw new ArgumentException($"{propertyName}.AddressSpace specified but required {propertyName}.AddressSpace.Cidr is missing");
                }
            }
        }

        public override void ResolveInlineCreatable(IAzure azure, IGroupableModel parentModel)
        {
            if (this.creatable == null)
            {
                if (this.AddressSpace != null && this.AddressSpace.Cidr != null)
                {
                    Microsoft.Azure.Management.Network.Fluent.Network.Definition.IWithGroup withGroup;
                    withGroup = azure.Networks
                        .Define(SdkContext.RandomResourceName("nw", 15))
                        .WithRegion(parentModel.Location());

                    Microsoft.Azure.Management.Network.Fluent.Network.Definition.IWithCreate withCreate;
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

                    Microsoft.Azure.Management.Network.Fluent.Network.Definition.IWithCreateAndSubnet withCreateAndSubnet = withCreate.WithAddressSpace(this.AddressSpace.Cidr);
                    if (this.AddressSpace.Subnets != null)
                    {
                        withCreateAndSubnet.WithSubnets(this.AddressSpace.Subnets);
                    }
                    this.creatable = withCreateAndSubnet;
                }
            }
        }

        protected override String ReferencePath()
        {
            return "newNetwork.ref";
        }

        protected override String ReferencePrefix()
        {
            return "networks";
        }

        protected override CreatableModels<NetworkModel, INetwork> CreatableModels(FluentRequestModel fluentRequestModel)
        {
            return fluentRequestModel.NetworkModels;
        }
    }
}
