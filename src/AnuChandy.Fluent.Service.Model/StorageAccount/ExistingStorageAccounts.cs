// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Storage.Fluent;

namespace AnuChandy.Fluent.Service.Model.StorageAccount
{
    public class ExistingStorageAccounts
        : ExistingResources<ExistingStorageAccount, IStorageAccount>
    {
    }
}
