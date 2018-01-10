// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Collection of new resources in the request.
    /// </summary>
    /// <typeparam name="NewResourceT">The new resource type</typeparam>
    /// <typeparam name="FluentT">The fluent type of the creatable resource</typeparam>
    public class NewResources<NewResourceT, FluentT> : List<NewResourceT>, ISupportsValidateAndResolve
        where NewResourceT : ISupportsValidate, ISupportsCreatableReference<FluentT>, ISupportsInlineCreatable<FluentT>
        where FluentT : IIndexable
    {
        #region ISupportsValidateAndResolve

        public Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (NewResourceT item in this)
            {
                item.Validate(propertyName);
            }
            foreach (NewResourceT item in this)
            {
                item.ResolveInlineCreatable(azure, parentModel);
                item.ResolveCreatableReference(azure, fluentRequestModel);
            }
            return Task.CompletedTask;
        }

        #endregion
    }
}
