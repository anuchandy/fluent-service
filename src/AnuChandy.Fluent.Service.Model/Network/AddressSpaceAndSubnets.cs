// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AnuChandy.Fluent.Service.Model.Network
{
    public class AddressSpaceAndSubnets
    {
        [JsonProperty(PropertyName = "cidr")]
        public String Cidr { get; set; }

        [JsonProperty(PropertyName = "subnets")]
        public Dictionary<String, String> Subnets { get; set; }
    }
}
