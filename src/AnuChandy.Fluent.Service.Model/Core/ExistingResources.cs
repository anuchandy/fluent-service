// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Fluent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Collection of existing resources in the request.
    /// </summary>
    /// <typeparam name="ExistingResourceT"></typeparam>
    /// <typeparam name="FluentT"></typeparam>
    public class ExistingResources<ExistingResourceT, FluentT> : 
        List<ExistingResourceT>, 
        ISupportsValidateAndResolve
        where ExistingResourceT : ISupportsValidate, ISupportExistingResource<FluentT>
    {
        public async Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (ExistingResourceT item in this)
            {
                item.Validate(propertyName);
            }
            foreach (ExistingResourceT item in this)
            {
                await item.ResolveResourceAsync(azure, cancellationToken);
            }
        }
    }
}
