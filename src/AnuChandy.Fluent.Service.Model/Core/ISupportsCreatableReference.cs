// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Type representing a reference that can be resolved to a creatable.
    /// </summary>
    /// <typeparam name="FluentT">The fluent type that resolved creatable produces</typeparam>
    public interface ISupportsCreatableReference<FluentT>
        where FluentT : IIndexable
    {
        /// <returns>The resolved ICreatable&lt;FluentT&gt;</returns>
        ICreatable<FluentT> GetCreatable();

        /// <summary>
        /// Resolves this reference to ICreatable&lt;FluentT&gt;.
        /// </summary>
        /// <param name="azure">azure instance used to prepare the resolved creatable defintion</param>
        /// <param name="fluentRequestModel">The request instance in which property of this type belongs to</param>
        void ResolveCreatableReference(IAzure azure, FluentRequestModel fluentRequestModel);
    }
}
