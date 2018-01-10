// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using AnuChandy.Fluent.Service.Model.Network;
using AnuChandy.Fluent.Service.Model.NetworkInterface;
using AnuChandy.Fluent.Service.Model.NetworkSecurityGroup;
using AnuChandy.Fluent.Service.Model.PublicIPAddress;
using AnuChandy.Fluent.Service.Model.ResourceGroup;
using AnuChandy.Fluent.Service.Model.StorageAccount;
using AnuChandy.Fluent.Service.Model.VirtualMachine;
using Microsoft.Azure.Management.Fluent;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model
{
    /// <summary>
    /// Type representing payload of fluent request to create azure resources.
    /// </summary>
    public class FluentRequestModel : ISupportsValidateAndResolve
    {
        [JsonProperty(PropertyName = "resourceGroups")]
        public ResourceGroupModels ResourceGroupModels { get; set; }

        [JsonProperty(PropertyName = "networks")]
        public NetworkModels NetworkModels { get; set; }

        [JsonProperty(PropertyName = "publicIpAddresses")]
        public PublicIPAddressModels PublicIPAddressModels { get; set; }

        [JsonProperty(PropertyName = "networkSecurityGroups")]
        public NetworkSecurityGroupModels NetworkSecurityGroupModels { get; set; }

        [JsonProperty(PropertyName = "networkInterfaces")]
        public NetworkInterfaceModels NetworkInterfaceModels { get; set; }

        [JsonProperty(PropertyName = "virtualMachines")]
        public VirtualMachineModels VirtualMachineModels { get; set; }

        [JsonProperty(PropertyName = "storageAccounts")]
        public StorageAccountModels StorageAccountModels { get; set; }

        #region ISupportsValidateAndResolve

        public async Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ResourceGroupModels != null)
            {
                await ResourceGroupModels.ValidateAndResolveAsync(azure, fluentRequestModel, "resourceGroups", parentModel, cancellationToken);
            }
            if (NetworkModels != null)
            {
                await NetworkModels.ValidateAndResolveAsync(azure, fluentRequestModel, "networks", parentModel, cancellationToken);
            }
            if (PublicIPAddressModels != null)
            {
                await PublicIPAddressModels.ValidateAndResolveAsync(azure, fluentRequestModel, "publicIpAddresses", parentModel, cancellationToken);
            }
            if (NetworkSecurityGroupModels != null)
            {
                await NetworkSecurityGroupModels.ValidateAndResolveAsync(azure, fluentRequestModel, "networkSecurityGroups", parentModel, cancellationToken);
            }
            if (NetworkInterfaceModels != null)
            {
                await NetworkInterfaceModels.ValidateAndResolveAsync(azure, fluentRequestModel, "networkInterfaces", parentModel, cancellationToken);
            }
            if (StorageAccountModels != null)
            {
                await StorageAccountModels.ValidateAndResolveAsync(azure, fluentRequestModel, "storageAccounts", parentModel, cancellationToken);
            }
            if (VirtualMachineModels != null)
            {
                await VirtualMachineModels.ValidateAndResolveAsync(azure, fluentRequestModel, "virtualMachines", parentModel, cancellationToken);
            }
        }

        #endregion

        /// <summary>
        /// Resolves the root creatables which are not resolved via ValidateAndResolveAsync. 
        /// </summary>
        /// <param name="azure">the azure client to prepare the root creatable defintions</param>
        /// <returns>the root creatables indexed by their collection type</returns>
        public Dictionary<string, List<dynamic>> ResolveRootCreatables(IAzure azure)
        {
            Dictionary<string, List<dynamic>> rootCreatables = new Dictionary<string, List<dynamic>>();
            var rootResourceGroups = ResolveNonResolvedCreatables(azure, ResourceGroupModels);
            rootCreatables.Add(ResourceCollectionType.ResourceGroups, rootResourceGroups);

            var rootNetworks = ResolveNonResolvedCreatables(azure, NetworkModels);
            rootCreatables.Add(ResourceCollectionType.Networks, rootNetworks);

            var rootPublicIPAddresses = ResolveNonResolvedCreatables(azure, PublicIPAddressModels);
            rootCreatables.Add(ResourceCollectionType.PublicIPAddresses, rootPublicIPAddresses);

            var rootNetworkSecurityGroups = ResolveNonResolvedCreatables(azure, NetworkSecurityGroupModels);
            rootCreatables.Add(ResourceCollectionType.NetworkSecurityGroups, rootNetworkSecurityGroups);

            var rootNetworkInterfaces = ResolveNonResolvedCreatables(azure, NetworkInterfaceModels);
            rootCreatables.Add(ResourceCollectionType.NetworkInterfaces, rootNetworkInterfaces);

            var rootStorageAccounts = ResolveNonResolvedCreatables(azure, StorageAccountModels);
            rootCreatables.Add(ResourceCollectionType.StorageAccounts, rootStorageAccounts);

            var rootVirtualMachines = ResolveNonResolvedCreatables(azure, VirtualMachineModels);
            rootCreatables.Add(ResourceCollectionType.VirtualMachines, rootVirtualMachines);

            return rootCreatables;
        }

        private static List<dynamic> ResolveNonResolvedCreatables<ModelT, FluentT>(IAzure azure, CreatableModels<ModelT, FluentT> creatableModels)
            where ModelT : ISupportsToCreatable<FluentT>, ISupportsValidateAndResolve
        {
            List<dynamic> creatables = new List<dynamic>();
            if (creatableModels != null)
            {
                foreach (var creatableModel in creatableModels)
                {
                    if (!creatableModel.IsCreatableReady())
                    {
                        creatables.Add(creatableModel.ToCreatable(azure));
                    }
                }
            }
            return creatables;
        }
    }
}
