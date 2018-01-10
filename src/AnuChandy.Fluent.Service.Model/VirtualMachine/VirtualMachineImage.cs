// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using System;

namespace AnuChandy.Fluent.Service.Model.VirtualMachine
{
    public class VirtualMachineImage 
        : ISupportsValidate
    {
        public String CustomImageId { get; private set; }

        public ImageReference ImageReference
        {
            get; private set;
        }

        private String rawId;

        public VirtualMachineImage(String rawId)
        {
            this.rawId = rawId;
        }

        public void Validate(String propertyName)
        {
            if (this.rawId == null)
            {
                throw new ArgumentException($"{propertyName} is required");
            }
            var parts = this.rawId.Split(new char[] { ':' });
            if (parts.Length == 1)
            {
                this.CustomImageId = this.rawId;
                return;
            }

            this.ImageReference = new ImageReference();
            if (parts.Length != 3 && parts.Length != 4)
            {
                throw new ArgumentException($"{propertyName} should be in either custom image id or should be in the format publisher:offer:sku[:version]");
            }
            this.ImageReference.Publisher = parts[0];

            this.ImageReference.Offer = parts[1];

            this.ImageReference.Sku = parts[2];

            if (parts.Length == 4)
            {
                this.ImageReference.Version = parts[3];
            }
            else
            {
                this.ImageReference.Version = "latest";
            }
        }
    }
}
