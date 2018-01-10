// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Fluent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Collection of models in the request those represents the azure resources to be created.
    /// </summary>
    /// <typeparam name="ModelT">The type of the model</typeparam>
    /// <typeparam name="FluentT">The fluent type of the azure resource</typeparam>
    public abstract class CreatableModels<ModelT, FluentT> : List<ModelT>,
        ISupportsValidateAndResolve
        where ModelT : ISupportsToCreatable<FluentT>, ISupportsValidateAndResolve
    {
        #region ISupportsValidateAndResolve

        public async Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            int i = 0;
            foreach (var model in this)
            {
                await model.ValidateAndResolveAsync(azure, fluentRequestModel, $"{propertyName}[{i}]", parentModel, cancellationToken);
                i++;
            }
        }

        #endregion
    }
}
