// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Fluent;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Type representing a reference to an existing azure resource.
    /// </summary>
    /// <typeparam name="FluentT">The fluent type of the azure resource</typeparam>
    public interface ISupportExistingResource<FluentT>
    {
        /// <returns>The resolved existing azure resource</returns>
        FluentT GetResource();

        /// <summary>
        /// Resolves the existing azure resource.
        /// </summary>
        /// <param name="azure">azure instance used to resolve existing azure resource references</param>
        /// <param name="cancellation">The task cncellation token</param>
        /// <returns></returns>
        Task ResolveResourceAsync(IAzure azure, CancellationToken cancellation = default(CancellationToken));
    }
}
