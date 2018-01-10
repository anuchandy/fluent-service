// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Network.Fluent;
using System;

namespace AnuChandy.Fluent.Service.Model.NetworkSecurityGroup
{
    public class NewNetworkSecurityGroup 
        : NewResource<NetworkSecurityGroupModel, INetworkSecurityGroup>
    {
        protected override CreatableModels<NetworkSecurityGroupModel, INetworkSecurityGroup> CreatableModels(FluentRequestModel fluentRequestModel)
        {
            return fluentRequestModel.NetworkSecurityGroupModels;
        }
        protected override String ReferencePath()
        {
            return "newNetworkSecurityGroup.ref";
        }

        protected override String ReferencePrefix()
        {
            return "networkSecurityGroups";
        }
    }
}
