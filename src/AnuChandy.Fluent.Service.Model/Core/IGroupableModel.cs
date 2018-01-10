// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// A model representing a new Azure resource type that requires a location and 
    /// a resource group for it's existance.
    /// </summary>
    public interface IGroupableModel
    {
        /// <returns>The region that the azure resource resides</returns>
        string Location();

        /// <returns>The defintion of new resource group in which the azure resource that
        /// this model representing will eventaully resides.</returns>
        ICreatable<IResourceGroup> CreatableResourceGroup();

        /// <returns>An existing resource group in which the azure resource that this model
        /// representing will eventaully resides.</returns>
        IResourceGroup ResourceGroup();
    }
}
