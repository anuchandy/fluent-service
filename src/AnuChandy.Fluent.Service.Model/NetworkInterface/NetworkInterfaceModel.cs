// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using AnuChandy.Fluent.Service.Model.LoadBalancer;
using AnuChandy.Fluent.Service.Model.Network;
using AnuChandy.Fluent.Service.Model.NetworkSecurityGroup;
using AnuChandy.Fluent.Service.Model.PublicIPAddress;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.NetworkInterface
{
    public class NetworkInterfaceModel
        : CreatableGroupableModel<Microsoft.Azure.Management.Network.Fluent.NetworkInterface.Definition.IWithPrimaryNetwork, Microsoft.Azure.Management.Network.Fluent.NetworkInterface.Definition.IWithCreate, INetworkInterface>
    {
        [JsonProperty(PropertyName = "newPrimaryNetwork")]
        public NewNetwork NewPrimaryNetwork { get; set; }

        [JsonProperty(PropertyName = "existingPrimaryNetwork")]
        public ExistingPrimaryNetworkSubnet ExistingPrimaryNetwork { get; set; }

        [JsonProperty(PropertyName = "primaryStaticIpAddress")]
        public String PrimaryStaticIpAddress { get; set; }

        [JsonProperty(PropertyName = "newPrimaryPublicIPAddress")]
        public NewPublicIPAddress NewPrimaryPublicIPAddress { get; set; }

        [JsonProperty(PropertyName = "existingPrimaryPublicIPAddress")]
        public ExistingPublicIPAddress ExistingPrimiaryPublicIPAddress { get; set; }

        [JsonProperty(PropertyName = "loadBalancerBackends")]
        ExistingLoadBalancerBackends ExistingLoadBalancerBackends { get; set; }

        [JsonProperty(PropertyName = "loadBalancerInboundNatRules")]
        public ExistingLoadBalancerInboundNatRules ExistingLoadBalancerInboundNatRules { get; set; }

        [JsonProperty(PropertyName = "existingNetworkSecurityGroups")]
        public ExistingNetworkSecurityGroups ExistingNetworkSecurityGroups { get; set; }

        [JsonProperty(PropertyName = "newNetworkSecurityGroups")]
        public NewNetworkSecurityGroups NewNetworkSecurityGroups { get; set; }

        public override async Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.ValidateAndResolveResourceGroupAsync(azure, fluentRequestModel, propertyName, parentModel, cancellationToken);
            if (NewPrimaryNetwork != null)
            {
                NewPrimaryNetwork.Validate($"{propertyName}.newPrimaryNetwork");
                NewPrimaryNetwork.ResolveInlineCreatable(azure, this);
                NewPrimaryNetwork.ResolveCreatableReference(azure, fluentRequestModel);
            }
            if (ExistingPrimaryNetwork != null)
            {
                ExistingPrimaryNetwork.Validate($"{propertyName}.existingPrimaryNetwork");
                await ExistingPrimaryNetwork.ResolveResourceAsync(azure, cancellationToken);
            }
            if (NewPrimaryPublicIPAddress != null)
            {
                NewPrimaryPublicIPAddress.Validate($"{propertyName}.newPrimaryPublicIPAddress");
                NewPrimaryPublicIPAddress.ResolveInlineCreatable(azure, this);
                NewPrimaryPublicIPAddress.ResolveCreatableReference(azure, fluentRequestModel);
            }
            if (ExistingPrimiaryPublicIPAddress != null)
            {
                ExistingPrimiaryPublicIPAddress.Validate($"{propertyName}.existingPrimiaryPublicIPAddress");
                await ExistingPrimiaryPublicIPAddress.ResolveResourceAsync(azure, cancellationToken);
            }
            if (ExistingLoadBalancerBackends != null)
            {
                await ExistingLoadBalancerBackends.ValidateAndResolveAsync(azure, fluentRequestModel, $"{propertyName}.loadBalancerBackends", this, cancellationToken);
            }
            if (ExistingLoadBalancerInboundNatRules != null)
            {
                await ExistingLoadBalancerInboundNatRules.ValidateAndResolveAsync(azure, fluentRequestModel, $"{propertyName}.loadBalancerBackends", this, cancellationToken);
            }
            if (NewNetworkSecurityGroups != null)
            {
                await NewNetworkSecurityGroups.ValidateAndResolveAsync(azure, fluentRequestModel, $"{propertyName}.newPrimaryPublicIPAddress", this, cancellationToken);
            }
            if (ExistingNetworkSecurityGroups != null)
            {
                await ExistingNetworkSecurityGroups.ValidateAndResolveAsync(azure, fluentRequestModel, $"{propertyName}.existingNetworkSecurityGroups", this, cancellationToken);
            }
        }

        protected override ICreatable<INetworkInterface> ToCreatableIntern(IAzure azure)
        {
            var withRegion = azure.NetworkInterfaces.Define(DeriveName("pip"));
            var withGroup = withRegion.WithRegion(this.Region);
            var withPrimaryNetwork = SetResourceGroupAndTags(withGroup);

            Microsoft.Azure.Management.Network.Fluent.NetworkInterface.Definition.IWithPrimaryPrivateIP withPrimaryPrivateIP;
            if (this.NewPrimaryNetwork != null)
            {
                withPrimaryPrivateIP = withPrimaryNetwork
                    .WithNewPrimaryNetwork(this.NewPrimaryNetwork.GetCreatable());
            }
            else if (this.ExistingPrimaryNetwork != null)
            {
                withPrimaryPrivateIP = withPrimaryNetwork
                    .WithExistingPrimaryNetwork(this.ExistingPrimaryNetwork.GetResource())
                    .WithSubnet(this.ExistingPrimaryNetwork.SubnetName);
            }
            else
            {
                // Default Network
                //
                withPrimaryPrivateIP = withPrimaryNetwork
                    .WithNewPrimaryNetwork(SdkContext.RandomResourceName("nw", 15), "10.0.0.0/28");
            }

            Microsoft.Azure.Management.Network.Fluent.NetworkInterface.Definition.IWithCreate withCreate;
            if (this.PrimaryStaticIpAddress != null)
            {
                withCreate = withPrimaryPrivateIP.WithPrimaryPrivateIPAddressStatic(this.PrimaryStaticIpAddress);
            }
            else
            {
                // Default IPAllocation type
                //
                withCreate = withPrimaryPrivateIP.WithPrimaryPrivateIPAddressDynamic();
            }

            if (this.NewPrimaryPublicIPAddress != null)
            {
                withCreate = withCreate.WithNewPrimaryPublicIPAddress(this.NewPrimaryPublicIPAddress.GetCreatable());
            }
            else if (this.ExistingPrimiaryPublicIPAddress != null)
            {
                withCreate = withCreate.WithExistingPrimaryPublicIPAddress(this.ExistingPrimiaryPublicIPAddress.GetResource());
            }

            if (this.ExistingLoadBalancerBackends != null)
            {
                foreach (var e in this.ExistingLoadBalancerBackends)
                {
                    foreach (var backendName in e.BackendNames)
                    {
                        withCreate = withCreate.WithExistingLoadBalancerBackend(e.ExistingLoadBalancer.GetResource(), backendName);
                    }
                }
            }

            if (this.ExistingLoadBalancerInboundNatRules != null)
            {
                foreach (var e in this.ExistingLoadBalancerInboundNatRules)
                {
                    foreach (var natRuleName in e.NatRuleNames)
                    {
                        withCreate = withCreate.WithExistingLoadBalancerInboundNatRule(e.ExistingLoadBalancer.GetResource(), natRuleName);
                    }
                }
            }

            if (this.ExistingNetworkSecurityGroups != null)
            {
                foreach (var sGroup in this.ExistingNetworkSecurityGroups)
                {
                    withCreate = withCreate.WithExistingNetworkSecurityGroup(sGroup.GetResource());
                }
            }

            if (this.NewNetworkSecurityGroups != null)
            {
                foreach (var sGroup in this.NewNetworkSecurityGroups)
                {
                    withCreate = withCreate.WithNewNetworkSecurityGroup(sGroup.GetCreatable());
                }
            }
            SetTags(withCreate);
            return withCreate;
        }
    }
}
