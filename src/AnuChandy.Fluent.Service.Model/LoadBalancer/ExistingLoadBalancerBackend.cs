// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.LoadBalancer
{
    public class ExistingLoadBalancerBackend
    {
        [JsonProperty(PropertyName = "existingLoadBalancer")]
        public ExistingLoadBalancer ExistingLoadBalancer { get; set; }

        [JsonProperty(PropertyName = "backendNames")]
        public IList<String> BackendNames { get; set; }

        public void Validate(String propertyName)
        {
            if (ExistingLoadBalancer == null)
            {
                throw new ArgumentException($"{propertyName} specified but required {propertyName}.ExistingLoadBalancer is not specified");
            }
            ExistingLoadBalancer.Validate($"{propertyName}.ExistingLoadBalancer");
            if (this.BackendNames == null || BackendNames.Count == 0)
            {
                throw new ArgumentException($"{propertyName} specified but required {propertyName}.BackendName is not specified");
            }
        }

        public Task ResolveResourceAsync(IAzure azure, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.ExistingLoadBalancer.ResolveResourceAsync(azure, cancellationToken);
        }
    }

    class ExistingLoadBalancerBackends 
        : List<ExistingLoadBalancerBackend>, ISupportsValidateAndResolve
    {
        public async Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var item in this)
            {
                item.Validate(propertyName);
            }
            foreach (var item in this)
            {
                await item.ResolveResourceAsync(azure, cancellationToken);
            }
        }
    }
}
