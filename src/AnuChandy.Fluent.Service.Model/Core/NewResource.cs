// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// Base type for a property type in the request which represents a new resource to be
    /// created in azure and can be resolved to a creatable (inline or reference creatable).
    /// </summary>
    /// <typeparam name="ModelT">The model type describing the creatable</typeparam>
    /// <typeparam name="FluentT">The fluent type of the creatable resource</typeparam>
    public abstract class NewResource<ModelT, FluentT> 
        : ISupportsValidate, 
        ISupportsCreatableReference<FluentT>,
        ISupportsInlineCreatable<FluentT>
        where FluentT : IIndexable
        where ModelT : ISupportsToCreatable<FluentT>, ISupportsValidateAndResolve
    {
        /// <summary>
        /// Indexed reference to model instance in the request representing the creatable.
        /// </summary>
        [JsonProperty(PropertyName = "ref")]
        public String Reference { get; set; }

        [JsonIgnore]
        protected ICreatable<FluentT> creatable;

        #region ISupportsValidate

        public virtual void Validate(string propertyName)
        {
            if (this.Reference == null)
            {
                throw new ArgumentException($"{propertyName} specified but required {propertyName}.ref is missing");
            }
        }

        #endregion

        #region ISupportsCreatableReference<FluentT>

        public ICreatable<FluentT> GetCreatable()
        {
            if (this.creatable == null)
            {
                throw new InvalidOperationException("Attempted to get a creatable that is not yet resolved");
            }
            return this.creatable;
        }

        public void ResolveCreatableReference(IAzure azure, FluentRequestModel fluentRequestModel)
        {
            if (this.creatable == null)
            {
                String refName = null;
                int refIndex = -1;
                try
                {
                    Regex regex = new Regex(@"(\w+)\[([0-9]+)]");
                    Match match = regex.Match(this.Reference);

                    if (match.Success && match.Groups.Count == 3)
                    {
                        refName = match.Groups[1].Value;
                        refIndex = Int16.Parse(match.Groups[2].Value);
                    }
                    else
                    {
                        throw new ArgumentException($"Malformed reference {this.Reference}");
                    }
                }
                catch (Exception)
                {
                    throw new ArgumentException($"Malformed reference {this.Reference}");
                }

                if (!refName.Equals(ReferencePrefix(), StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"Value of {ReferencePath()} should be {ReferencePrefix()}[index] but found {this.Reference}");
                }

                var creatableModels = this.CreatableModels(fluentRequestModel);
                if (creatableModels.Count < refIndex)
                {
                    throw new ArgumentException($"The index {refIndex} of the reference {this.Reference} is out of boundary");
                }
                var creatableModel = creatableModels[refIndex];
                this.creatable = creatableModel.ToCreatable(azure);
            }
        }

        #endregion

        #region ISupportsInlineCreatable<FluentT>

        public virtual void ResolveInlineCreatable(IAzure azure, IGroupableModel parentModel)
        {
            // NOP
        }

        #endregion

        /// <returns>The name for the collection in the request containing creatable definition for this new resource</returns>
        protected abstract String ReferencePrefix();

        /// <returns>User facing name of the reference that is pointing to the creatable definition for this new resource</returns>
        protected abstract String ReferencePath();

        /// <summary>
        /// Gets the model collection in the request containing the creatable entry this new resource references.
        /// </summary>
        /// <param name="fluentRequestModel">The request instance in which property of this type belongs to</param>
        /// <returns>The model collection</returns>
        protected abstract CreatableModels<ModelT, FluentT> CreatableModels(FluentRequestModel fluentRequestModel);
    }
}
