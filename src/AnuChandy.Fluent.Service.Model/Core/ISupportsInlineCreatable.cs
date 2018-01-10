// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Type represents inline creatable definition.
    /// </summary>
    /// <typeparam name="FluentT">The fluent type that creatable produces</typeparam>
    public interface ISupportsInlineCreatable<FluentT> 
        where FluentT : IIndexable
    {
        /// <returns>The resolved ICreatable&lt;FluentT&gt;</returns>
        ICreatable<FluentT> GetCreatable();

        /// <summary>
        /// Resolves this inline creatable definition to ICreatable&lt;FluentT&gt;.
        /// </summary>
        /// <param name="azure">azure instance used to prepare the resolved creatable defintion</param>
        /// <param name="parentModel">The parent model that directly references property of this type</param>
        void ResolveInlineCreatable(IAzure azure, IGroupableModel parentModel);
    }
}
