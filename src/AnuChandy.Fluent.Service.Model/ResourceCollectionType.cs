// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace AnuChandy.Fluent.Service.Model
{
    /// <summary>
    /// The supported azure resource collection names.
    /// </summary>
    public class ResourceCollectionType
    {
        public const string ResourceGroups = "ResourceGroups";

        public const string PublicIPAddresses = "PublicIPAddresses";

        public const string Networks = "Networks";

        public const string NetworkSecurityGroups = "NetworkSecurityGroups";

        public const string NetworkInterfaces = "NetworkInterfaces";

        public const string StorageAccounts = "StorageAccounts";

        public const string VirtualMachines = "VirtualMachines";
    }
}
