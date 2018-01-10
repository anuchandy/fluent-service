// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.Network
{
    public class NetworkModel 
        : CreatableGroupableModel<Microsoft.Azure.Management.Network.Fluent.Network.Definition.IWithCreate, Microsoft.Azure.Management.Network.Fluent.Network.Definition.IWithCreate, INetwork>
    {
        [JsonProperty(PropertyName = "addressSpace")]
        public AddressSpaceAndSubnets AddressSpace { get; set; }

        [JsonProperty(PropertyName = "dnsServers")]
        public List<String> DnsServers { get; set; }

        public override async Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.ValidateAndResolveResourceGroupAsync(azure, fluentRequestModel, propertyName, parentModel, cancellationToken);
            if (this.AddressSpace != null
                && this.AddressSpace.Cidr == null
                && this.AddressSpace.Subnets != null)
            {
                throw new ArgumentException($"Specifying {propertyName}.addressSpace.subnets requires {propertyName}.addressSpace.cidr to present");
            }
        }

        protected override ICreatable<INetwork> ToCreatableIntern(IAzure azure)
        {
            var withRegion = azure.Networks.Define(DeriveName("nw"));
            var withGroup = withRegion.WithRegion(this.Region);
            var withCreate = SetResourceGroupAndTags(withGroup);

            if (this.AddressSpace != null)
            {
                if (this.AddressSpace.Cidr != null)
                {
                    var withCreateAndSubnet = withCreate.WithAddressSpace(this.AddressSpace.Cidr);
                    if (this.AddressSpace.Subnets != null)
                    {
                        withCreateAndSubnet.WithSubnets(this.AddressSpace.Subnets);
                    }
                }
            }

            if (this.DnsServers != null)
            {
                foreach (var dnsServer in this.DnsServers)
                {
                    withCreate.WithDnsServer(dnsServer);
                }
            }

            SetTags(withCreate);
            return withCreate;
        }
    }
}
