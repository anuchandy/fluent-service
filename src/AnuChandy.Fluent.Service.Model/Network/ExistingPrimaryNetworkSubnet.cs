// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System;

namespace AnuChandy.Fluent.Service.Model.Network
{
    public class ExistingPrimaryNetworkSubnet : ExistingNetwork
    {
        [JsonProperty(PropertyName = "subnetName")]
        public String SubnetName { get; set; }

        public new void Validate(String propertyName)
        {
            base.Validate(propertyName);
            if (this.SubnetName == null)
            {
                throw new ArgumentException($"{propertyName} specified but required {propertyName}.SubnetName is not specified");
            }
        }
    }
}
