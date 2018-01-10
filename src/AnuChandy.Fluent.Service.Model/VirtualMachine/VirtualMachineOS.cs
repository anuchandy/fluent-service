// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Newtonsoft.Json;
using System;

namespace AnuChandy.Fluent.Service.Model.VirtualMachine
{
    public class VirtualMachineOS 
        : ISupportsValidate
    {
        [JsonProperty(PropertyName = "imageId")]
        public String ImageId { get; set; }

        [JsonProperty(PropertyName = "credentials")]
        public Credentials Credentials { get; set; }

        [JsonIgnore]
        private VirtualMachineImage virtualMachineImage;

        public VirtualMachineImage VirtualMachineImage()
        {
            return this.virtualMachineImage;
        }

        public void Validate(String propertyName)
        {
            this.virtualMachineImage = new VirtualMachineImage(this.ImageId);
            this.virtualMachineImage.Validate($"{propertyName}.imageId");
            this.Credentials.Validate($"{propertyName}.credentails");
        }
    }
}
