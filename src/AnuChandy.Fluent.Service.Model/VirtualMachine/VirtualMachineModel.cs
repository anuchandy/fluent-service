// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using AnuChandy.Fluent.Service.Model.Network;
using AnuChandy.Fluent.Service.Model.NetworkInterface;
using AnuChandy.Fluent.Service.Model.PublicIPAddress;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Compute.Fluent.VirtualMachine.Definition;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.VirtualMachine
{
    public class VirtualMachineModel
                : CreatableGroupableModel<Microsoft.Azure.Management.Compute.Fluent.VirtualMachine.Definition.IWithNetwork, Microsoft.Azure.Management.Compute.Fluent.VirtualMachine.Definition.IWithCreate, IVirtualMachine>
    {
        [JsonProperty(PropertyName = "newPrimaryNetwork")]
        public NewNetwork NewPrimaryNetwork { get; set; }

        [JsonProperty(PropertyName = "existingPrimaryNetwork")]
        public ExistingPrimaryNetworkSubnet ExistingPrimaryNetwork { get; set; }

        [JsonProperty(PropertyName = "newPrimaryNetworkInterface")]
        public NewNetworkInterface NewPrimaryNetworkInterface {get; set;}

        [JsonProperty(PropertyName = "existingPrimaryNetworkInterface")]
        public ExistingNetworkInterface ExistingPrimaryNetworkInterface { get; set; }

        [JsonProperty(PropertyName = "primaryStaticPrivateIPAddress")]
        public String PrimaryStaticPrivateIPAddress { get; set; }

        [JsonProperty(PropertyName = "newPrimaryPublicIPAddress")]
        public NewPublicIPAddress NewPrimaryPublicIPAddress { get; set; }

        [JsonProperty(PropertyName = "existingPrimaryPublicIPAddress")]
        public ExistingPublicIPAddress ExistingPrimaryPublicIPAddress { get; set; }

        [JsonProperty(PropertyName = "linux")]
        public VirtualMachineOS Linux { get; set; }

        [JsonProperty(PropertyName = "windows")]
        public VirtualMachineOS Windows { get; set; }

        [JsonProperty(PropertyName = "bootDiagnostics")]
        public BootDiagnostics BootDiagnostics { get; set; }

        public override async Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken)
        {
            await this.ValidateAndResolveResourceGroupAsync(azure, fluentRequestModel, propertyName, parentModel, cancellationToken);
            if (this.Linux != null)
            {
                this.Linux.Validate($"{propertyName}.linux");
            }
            else if (this.Windows != null)
            {
                this.Windows.Validate($"{propertyName}.windows");
            }
            else
            {
                throw new ArgumentException($"{propertyName}.linux or {propertyName}.windows should be specified");
            }
            if (NewPrimaryNetwork != null)
            {
                NewPrimaryNetwork.Validate($"{propertyName}.newPrimaryNetwork");
                NewPrimaryNetwork.ResolveInlineCreatable(azure, this);
                NewPrimaryNetwork.ResolveCreatableReference(azure, fluentRequestModel);
            }
            if (ExistingPrimaryNetwork != null)
            {
                ExistingPrimaryNetwork.Validate($"{propertyName}.existingPrimaryNetwork");
                await ExistingPrimaryNetwork.ResolveResourceAsync(azure, cancellationToken);
            }
            if (NewPrimaryPublicIPAddress != null)
            {
                NewPrimaryPublicIPAddress.Validate($"{propertyName}.newPrimaryPublicIPAddress");
                NewPrimaryPublicIPAddress.ResolveInlineCreatable(azure, this);
                NewPrimaryPublicIPAddress.ResolveCreatableReference(azure, fluentRequestModel);
            }
            if (ExistingPrimaryPublicIPAddress != null)
            {
                ExistingPrimaryPublicIPAddress.Validate($"{propertyName}.existingPrimaryPublicIPAddress");
                await ExistingPrimaryPublicIPAddress.ResolveResourceAsync(azure, cancellationToken);
            }
            if (NewPrimaryNetworkInterface != null)
            {
                NewPrimaryNetworkInterface.Validate($"{propertyName}.newPrimaryNetworkInterface");
                NewPrimaryNetworkInterface.ResolveInlineCreatable(azure, this);
                NewPrimaryNetworkInterface.ResolveCreatableReference(azure, fluentRequestModel);
            }
            if (ExistingPrimaryNetworkInterface != null)
            {
                ExistingPrimaryNetworkInterface.Validate($"{propertyName}.existingPrimaryNetworkInterface");
                await ExistingPrimaryNetworkInterface.ResolveResourceAsync(azure, cancellationToken);
            }
            if (this.BootDiagnostics != null)
            {
                await this.BootDiagnostics.ValidateAndResolveAsync(azure, fluentRequestModel, $"{propertyName}.bootDiagnostics", this, cancellationToken);
            }
        }

        protected override ICreatable<IVirtualMachine> ToCreatableIntern(IAzure azure)
        {
            var withRegion = azure.VirtualMachines.Define(DeriveName("vm"));
            var withGroup = withRegion.WithRegion(this.Region);
            var withNetwork = SetResourceGroupAndTags(withGroup);

            IWithPrivateIP withPrivateIP;
            if (this.NewPrimaryNetwork != null)
            {
                withPrivateIP = withNetwork.WithNewPrimaryNetwork(this.NewPrimaryNetwork.GetCreatable());
            }
            else if (this.ExistingPrimaryNetwork != null)
            {
                withPrivateIP = withNetwork.WithExistingPrimaryNetwork(this.ExistingPrimaryNetwork.GetResource())
                    .WithSubnet(this.ExistingPrimaryNetwork.SubnetName);
            }
            else
            {
                withPrivateIP = withNetwork.WithNewPrimaryNetwork("10.0.0.0/28");
            }

            IWithPublicIPAddress withPublicIPAddress;
            if (this.PrimaryStaticPrivateIPAddress != null)
            {
                withPublicIPAddress = withPrivateIP.WithPrimaryPrivateIPAddressStatic(this.PrimaryStaticPrivateIPAddress);
            }
            else
            {
                withPublicIPAddress = withPrivateIP.WithPrimaryPrivateIPAddressDynamic();
            }

            IWithOS withOS;
            if (this.NewPrimaryPublicIPAddress != null)
            {
                withOS = withPublicIPAddress.WithNewPrimaryPublicIPAddress(this.NewPrimaryPublicIPAddress.GetCreatable());
            }
            else if (this.ExistingPrimaryPublicIPAddress != null)
            {
                withOS = withPublicIPAddress.WithExistingPrimaryPublicIPAddress(this.ExistingPrimaryPublicIPAddress.GetResource());
            }
            else
            {
                withOS = withPublicIPAddress.WithoutPrimaryPublicIPAddress();
            }

            IWithCreate withCreate = null;
            if (this.Linux != null)
            {
                var image = this.Linux.VirtualMachineImage();
                if (image.CustomImageId != null)
                {
                    var withLinuxRootUserNameManaged = withOS.WithLinuxCustomImage(image.CustomImageId);
                    withCreate = withLinuxRootUserNameManaged
                        .WithRootUsername(this.Linux.Credentials.UserName)
                        .WithRootPassword(this.Linux.Credentials.Password);
                }
                else
                {
                    var withLinuxRootUserNameManagedOrUnManaged = withOS.WithSpecificLinuxImageVersion(image.ImageReference);
                    withCreate = withLinuxRootUserNameManagedOrUnManaged
                        .WithRootUsername(this.Linux.Credentials.UserName)
                        .WithRootPassword(this.Linux.Credentials.Password);
                }
            }
            else if (this.Windows != null)
            {
                var image = this.Windows.VirtualMachineImage();
                if (image.CustomImageId != null)
                {
                    var withWindowsAdminUserNameManaged = withOS.WithWindowsCustomImage(image.CustomImageId);
                    withCreate = withWindowsAdminUserNameManaged
                        .WithAdminUsername(this.Windows.Credentials.UserName)
                        .WithAdminPassword(this.Windows.Credentials.Password);
                }
                else
                {
                    var withWindowsAdminUserNameManagedOrUnManaged = withOS.WithSpecificWindowsImageVersion(image.ImageReference);
                    withCreate = withWindowsAdminUserNameManagedOrUnManaged
                        .WithAdminUsername(this.Windows.Credentials.UserName)
                        .WithAdminPassword(this.Windows.Credentials.Password);
                }
            }
            if (this.BootDiagnostics != null)
            {
                if (this.BootDiagnostics.Enabled.Value)
                {
                    if (this.BootDiagnostics.NewStorageAccount != null)
                    {
                        withCreate.WithBootDiagnostics(this.BootDiagnostics.NewStorageAccount.GetCreatable());
                    }
                    else if (this.BootDiagnostics.ExistingStorageAccount != null)
                    {
                        withCreate.WithBootDiagnostics(this.BootDiagnostics.ExistingStorageAccount.GetResource());
                    }
                    else
                    {
                        withCreate.WithBootDiagnostics();
                    }
                }
            }

            if (withCreate == null)
            {
                throw new NullReferenceException("DevError: VirtualMachine::withCreate");
            }
            return withCreate;
        }
    }
}
