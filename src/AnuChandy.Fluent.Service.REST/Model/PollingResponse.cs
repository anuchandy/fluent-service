// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AnuChandy.Fluent.Service.REST.Model
{
    /// <summary>
    /// Represents response from GetCreateStatus api call.
    /// </summary>
    public class PollingResponse
    {
        public List<ResourceCreateState> resourceGroups { get; set; }

        public List<ResourceCreateState> publicIPAddresses { get; set; }

        public List<ResourceCreateState> networks { get; set; }

        public List<ResourceCreateState> networkSecurityGroups { get; set; }

        public List<ResourceCreateState> networkInterfaces { get; set; }

        public List<ResourceCreateState> storageAccounts { get; set; }

        public List<ResourceCreateState> virtualMachines { get; set; }

        public List<Error> errors { get; set; }

        private PollingResponse()
        {
        }

        /// <summary>
        /// Creates from a table entries representing current create states of a fluent
        /// request processing.
        /// </summary>
        /// <param name="entities">the table entities</param>
        /// <returns></returns>
        public static PollingResponse From(TableQuerySegment<ResourceCreateStatusEntity> entities)
        {
            PollingResponse pollingResponse = new PollingResponse();
            foreach (var entity in entities)
            {
                if (entity.Type.Equals(ResourceType.ResourceGroup))
                {
                    if (pollingResponse.resourceGroups == null)
                    {
                        pollingResponse.resourceGroups = new List<ResourceCreateState>();
                    }
                    pollingResponse.resourceGroups.Add(ResourceCreateState.From(entity));
                }
                else if (entity.Type.Equals(ResourceType.Network))
                {
                    if (pollingResponse.networks == null)
                    {
                        pollingResponse.networks = new List<ResourceCreateState>();
                    }
                    pollingResponse.networks.Add(ResourceCreateState.From(entity));
                }
                else if (entity.Type.Equals(ResourceType.NetworkInterface))
                {
                    if (pollingResponse.networkInterfaces == null)
                    {
                        pollingResponse.networkInterfaces = new List<ResourceCreateState>();
                    }
                    pollingResponse.networkInterfaces.Add(ResourceCreateState.From(entity));
                }
                else if (entity.Type.Equals(ResourceType.NetworkSecurityGroup))
                {
                    if (pollingResponse.networkSecurityGroups == null)
                    {
                        pollingResponse.networkSecurityGroups = new List<ResourceCreateState>();
                    }
                    pollingResponse.networkSecurityGroups.Add(ResourceCreateState.From(entity));
                }
                else if (entity.Type.Equals(ResourceType.PublicIPAddress))
                {
                    if (pollingResponse.publicIPAddresses == null)
                    {
                        pollingResponse.publicIPAddresses = new List<ResourceCreateState>();
                    }
                    pollingResponse.publicIPAddresses.Add(ResourceCreateState.From(entity));
                }
                else if (entity.Type.Equals(ResourceType.StorageAccount))
                {
                    if (pollingResponse.storageAccounts == null)
                    {
                        pollingResponse.storageAccounts = new List<ResourceCreateState>();
                    }
                    pollingResponse.storageAccounts.Add(ResourceCreateState.From(entity));
                }
                else if (entity.Type.Equals(ResourceType.VirtualMachine))
                {
                    if (pollingResponse.virtualMachines == null)
                    {
                        pollingResponse.virtualMachines = new List<ResourceCreateState>();
                    }
                    pollingResponse.virtualMachines.Add(ResourceCreateState.From(entity));
                }
                else if (entity.Type.Equals("Error"))
                {
                    if (pollingResponse.errors == null)
                    {
                        pollingResponse.errors = new List<Error>();
                    }
                    pollingResponse.errors.Add(new Error
                    {
                        Message = entity.FailureMessage != null ? JsonConvert.DeserializeObject(entity.FailureMessage) : "UnknownError"
                    });
                }
            }
            return pollingResponse;
        }
    }

    public class Error
    {
        [JsonProperty(PropertyName = "message")]
        public Object Message { get; set; }
    }
}
