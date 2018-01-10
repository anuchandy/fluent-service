// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Newtonsoft.Json;
using System;

namespace AnuChandy.Fluent.Service.Model.VirtualMachine
{
    public class Credentials
    {
        [JsonProperty(PropertyName = "userName")]
        public String UserName { get; set; }

        [JsonProperty(PropertyName = "password")]
        public String Password { get; set; }

        public void Validate(String propertyName)
        {
            if (this.UserName == null
                || this.Password == null)
            {
                throw new ArgumentException($"{propertyName}.userName and {propertyName}.password is required");
            }
        }
    }
}
