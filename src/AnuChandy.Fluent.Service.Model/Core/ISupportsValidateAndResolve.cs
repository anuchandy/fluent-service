// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Fluent;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Type that supports validating it's current state and resolve relevant properties to
    /// creatables or existing resources in azure.
    /// </summary>
    public interface ISupportsValidateAndResolve
    {
        /// <summary>
        /// Validate and resolve properties to creatables (inline creatable and reference creatable)
        /// and existing azure resource reference 
        /// </summary>
        /// <param name="azure">azure instance for resolving existing azure resource references</param>
        /// <param name="fluentRequestModel">The request instance in which property of this type belongs to</param>
        /// <param name="propertyName">User facing name of property of this type composed in another type</param>
        /// <param name="parentModel">The parent model that directly references property of this type</param>
        /// <param name="cancellationToken">The task cancellation token</param>
        /// <returns></returns>
        Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, String propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken));
    }
}
