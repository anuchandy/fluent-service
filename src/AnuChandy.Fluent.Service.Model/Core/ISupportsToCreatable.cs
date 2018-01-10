// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Type that supports producing it's creatable representation.
    /// </summary>
    /// <typeparam name="FluentT">The fluent type of the creatable resource</typeparam>
    public interface ISupportsToCreatable<FluentT>
    {
        /// <returns>true if ToCreatable(..) is already called hence creatable is produced, false otherwise</returns>
        bool IsCreatableReady();

        /// <summary>
        /// Produces creatable.
        /// </summary>
        /// <param name="azure">the azure instance to prepare the creatable definition</param>
        /// <returns></returns>
        ICreatable<FluentT> ToCreatable(IAzure azure);
    }
}
