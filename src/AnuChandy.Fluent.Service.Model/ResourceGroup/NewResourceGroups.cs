﻿// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace AnuChandy.Fluent.Service.Model.ResourceGroup
{
    public class NewResourceGroups
        : NewResources<NewResourceGroup, IResourceGroup>
    {
    }
}
