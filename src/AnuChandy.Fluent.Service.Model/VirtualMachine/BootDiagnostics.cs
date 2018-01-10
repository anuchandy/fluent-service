// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using AnuChandy.Fluent.Service.Model.StorageAccount;
using Microsoft.Azure.Management.Fluent;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.VirtualMachine
{
    public class BootDiagnostics : ISupportsValidateAndResolve
    {
        [JsonProperty(PropertyName = "enabled")]
        public bool? Enabled { get; set; }

        [JsonProperty(PropertyName = "newStorageAccount")]
        public NewStorageAccount NewStorageAccount { get; set; }

        [JsonProperty(PropertyName = "existingStorageAccount")]
        public ExistingStorageAccount ExistingStorageAccount { get; set; }

        public async Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.Enabled == null)
            {
                throw new ArgumentException($"{propertyName} is specified then {propertyName}.enabled must be specified");
            }
            if (this.NewStorageAccount != null)
            {
                NewStorageAccount.Validate($"{propertyName}.newStorageAccount");
                NewStorageAccount.ResolveInlineCreatable(azure, parentModel);
                NewStorageAccount.ResolveCreatableReference(azure, fluentRequestModel);
            }
            if (this.ExistingStorageAccount != null)
            {
                ExistingStorageAccount.Validate($"{propertyName}.existingStorageAccount");
                await ExistingStorageAccount.ResolveResourceAsync(azure, cancellationToken);
            }
        }
    }
}
