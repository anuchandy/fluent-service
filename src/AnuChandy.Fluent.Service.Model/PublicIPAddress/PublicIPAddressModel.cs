// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.Network.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.PublicIPAddress
{
    public class PublicIPAddressModel
        : CreatableGroupableModel<Microsoft.Azure.Management.Network.Fluent.PublicIPAddress.Definition.IWithCreate, Microsoft.Azure.Management.Network.Fluent.PublicIPAddress.Definition.IWithCreate, IPublicIPAddress>
    {
        [JsonProperty(PropertyName = "leafDomainLabel")]
        public String LeafDomainLabel { get; set; }

        [JsonProperty(PropertyName = "enableStaticIp")]
        public bool? EnableStaticIpAddress { get; set; }

        [JsonProperty(PropertyName = "reverseFqdn")]
        public String ReverseFqdn { get; set; }

        [JsonProperty(PropertyName = "idleTimeoutInMinutes")]
        public int? IdleTimeoutInMinutes { get; set; }

        [JsonProperty(PropertyName = "sku")]
        public String Sku { get; set; }

        [JsonProperty(PropertyName = "availabiltiyZones")]
        public IList<String> AvailabiltiyZones { get; set; }

        public override async Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken)
        {
            await base.ValidateAndResolveResourceGroupAsync(azure, fluentRequestModel, propertyName, parentModel, cancellationToken);
        }

        protected override ICreatable<IPublicIPAddress> ToCreatableIntern(IAzure azure)
        {
            var withRegion = azure.PublicIPAddresses.Define(DeriveName("pip"));
            var withGroup = withRegion.WithRegion(this.Region);
            var withCreate = SetResourceGroupAndTags(withGroup);

            if (this.LeafDomainLabel != null)
            {
                withCreate.WithLeafDomainLabel(this.LeafDomainLabel);
            }
            if (this.EnableStaticIpAddress != null && this.EnableStaticIpAddress.Value)
            {
                withCreate.WithStaticIP();
            }
            if (this.ReverseFqdn != null)
            {
                withCreate.WithReverseFqdn(this.ReverseFqdn);
            }
            if (this.IdleTimeoutInMinutes != null)
            {
                withCreate.WithIdleTimeoutInMinutes(this.IdleTimeoutInMinutes.Value);
            }
            if (this.Sku != null)
            {
                withCreate.WithSku(PublicIPSkuType.Parse(this.Sku));
            }
            if (this.AvailabiltiyZones != null)
            {
                foreach (var zone in this.AvailabiltiyZones)
                {
                    withCreate.WithAvailabilityZone(AvailabilityZoneId.Parse(zone));
                }
            }
            SetTags(withCreate);
            return withCreate;
        }
    }
}
