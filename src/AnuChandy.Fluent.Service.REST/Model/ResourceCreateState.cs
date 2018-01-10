// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model;
using Newtonsoft.Json;
using System;

namespace AnuChandy.Fluent.Service.REST.Model
{
    /// <summary>
    /// Represents create state of an azure resource.
    /// </summary>
    public class ResourceCreateState
    {
        [JsonProperty(PropertyName = "type")]
        public String Type { get; set; }

        [JsonProperty(PropertyName = "status")]
        public String Status { get; set; }

        [JsonProperty(PropertyName = "resource")]
        public Object Resource { get; set; }

        [JsonProperty(PropertyName = "failureMessage")]
        public Object FailureMessage { get; set; }

        /// <summary>
        /// Creates from a table entry representing current create state of a azure resource.
        /// </summary>
        /// <param name="entity">the table entry</param>
        /// <returns></returns>
        public static ResourceCreateState From(ResourceCreateStatusEntity entity)
        {
            return new ResourceCreateState()
            {
                Type = entity.Type,
                Status = entity.Status,
                Resource = entity.Resource != null ? JsonConvert.DeserializeObject(entity.Resource) : null,
                FailureMessage = entity.FailureMessage != null ? JsonConvert.DeserializeObject(entity.FailureMessage) : null
            };
        }
    }
}
