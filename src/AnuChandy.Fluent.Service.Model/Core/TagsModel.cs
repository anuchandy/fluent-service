// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AnuChandy.Fluent.Service.Model.Core
{
    /// <summary>
    /// A model support setting tags.
    /// </summary>
    /// <typeparam name="CreatableT"></typeparam>
    public class TagsModel<CreatableT>
    {
        [JsonProperty(PropertyName = "tags")]
        public Dictionary<String, String> Tags { get; set; }

        /// <summary>
        /// Set tags in the given entity.
        /// </summary>
        /// <param name="withTags">The entity in wich tags needs to set</param>
        protected void SetTags(Microsoft.Azure.Management.ResourceManager.Fluent.Core.Resource.Definition.IDefinitionWithTags<CreatableT> withTags)
        {
            if (this.Tags != null)
            {
                withTags.WithTags(this.Tags);
            }
        }
    }
}
