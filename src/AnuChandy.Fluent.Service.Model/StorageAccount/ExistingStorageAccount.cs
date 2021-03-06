﻿// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Storage.Fluent;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.StorageAccount
{
    public class ExistingStorageAccount
        : ExistingGroupableResource<IStorageAccounts, IStorageAccount>
    {
        public override Task ResolveResourceAsync(IAzure azure, CancellationToken cancellationToken = default(CancellationToken))
        {
            return base.ResolveResourceAsync(azure.StorageAccounts, cancellationToken);
        }
    }
}
