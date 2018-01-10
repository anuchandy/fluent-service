// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Network.Fluent;

namespace AnuChandy.Fluent.Service.Model.NetworkInterface
{
    public class NewNetworkInterface
        : NewResource<NetworkInterfaceModel, INetworkInterface>
    {
        protected override CreatableModels<NetworkInterfaceModel, INetworkInterface> CreatableModels(FluentRequestModel fluentRequestModel)
        {
            return fluentRequestModel.NetworkInterfaceModels;
        }

        protected override string ReferencePath()
        {
            return "newNetworkInterface.ref";
        }

        protected override string ReferencePrefix()
        {
            return "networkInterfaces";
        }
    }
}
