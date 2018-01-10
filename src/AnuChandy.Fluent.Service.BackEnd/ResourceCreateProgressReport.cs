// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.BackEnd
{
    /// <summary>
    /// Implementation of IResourceCreateProgressReport that writes progress status
    /// of azure table for storing request states.
    /// </summary>
    class ResourceCreateProgressReport : IResourceCreateProgressReport
    {
        private ResourceCreateStatusesTable statusTable;

        public ResourceCreateProgressReport(ResourceCreateStatusesTable statusTable)
        {
            this.statusTable = statusTable;
        }

        #region IResourceCreateProgressReport

        public async Task InProgressAsync(string resourceIndexableKey, string name, string resourceType, CancellationToken cancellationToken = default(CancellationToken))
        {
            await statusTable.WriteInProgressAsync(resourceIndexableKey, resourceType, cancellationToken);
            Console.WriteLine($"----->  State: InProgress ResourceKey: {resourceIndexableKey} ResourceType: {resourceType}");
        }

        public async Task SucceededAsync(string resourceIndexableKey, string name, string resourceType, dynamic resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            dynamic inner = null;
            if (resourceType.Equals(ResourceType.ResourceGroup, StringComparison.CurrentCultureIgnoreCase))
            {
                inner = ((IResourceGroup)resource).Inner;
            }
            else if (resourceType.Equals(ResourceType.PublicIPAddress, StringComparison.CurrentCultureIgnoreCase))
            {
                inner = ((IPublicIPAddress)resource).Inner;
            }
            else if (resourceType.Equals(ResourceType.Network, StringComparison.CurrentCultureIgnoreCase))
            {
                inner = ((INetwork)resource).Inner;
            }
            else if (resourceType.Equals(ResourceType.NetworkSecurityGroup, StringComparison.CurrentCultureIgnoreCase))
            {
                inner = ((INetworkSecurityGroup)resource).Inner;
            }
            else if (resourceType.Equals(ResourceType.NetworkInterface, StringComparison.CurrentCultureIgnoreCase))
            {
                inner = ((INetworkInterface)resource).Inner;
            }
            else if (resourceType.Equals(ResourceType.StorageAccount, StringComparison.CurrentCultureIgnoreCase))
            {
                inner = ((IStorageAccount)resource).Inner;
            }
            else if (resourceType.Equals(ResourceType.VirtualMachine, StringComparison.CurrentCultureIgnoreCase))
            {
                inner = ((IVirtualMachine)resource).Inner;
            }

            if (inner != null)
            {
                await statusTable.WriteSucceededAsync(resourceIndexableKey, resourceType, inner, cancellationToken);
                Console.WriteLine($"----->  State: Succeeded ResourceKey: {resourceIndexableKey} ResourceType: {resourceType} Resource: {JsonConvert.SerializeObject(inner, Formatting.Indented)}");
            }
        }

        public async Task FailedAsync(string resourceIndexableKey, string name, string resourceType, dynamic exception, CancellationToken cancellationToken = default(CancellationToken))
        {
            await statusTable.WriteFailedAsync(resourceIndexableKey, resourceType, exception, cancellationToken);
            Console.WriteLine($"----->  State: Failed ResourceKey: {resourceIndexableKey} ResourceType: {resourceType} FailureMessage: {JsonConvert.SerializeObject(exception, Formatting.Indented)}");
        }

        #endregion
    }
}
