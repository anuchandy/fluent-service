// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Storage.Fluent;
using Newtonsoft.Json;
using System;

namespace AnuChandy.Fluent.Service.Model.StorageAccount
{
    public class NewStorageAccount : NewResource<StorageAccountModel, IStorageAccount>
    {
        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }

        public override void Validate(String propertyName)
        {
            if (this.Name == null)
            {
                base.Validate(propertyName);
                if (this.Reference == null)
                {
                    throw new ArgumentException($"{propertyName} specified but required {propertyName}.name is missing");
                }
            }
        }

        public override void ResolveInlineCreatable(IAzure azure, IGroupableModel parentModel)
        {
            if (this.creatable == null)
            {
                if (this.Name != null)
                {
                    Microsoft.Azure.Management.Storage.Fluent.StorageAccount.Definition.IWithGroup withGroup;
                    withGroup = azure.StorageAccounts.Define(this.Name)
                        .WithRegion(parentModel.Location());

                    Microsoft.Azure.Management.Storage.Fluent.StorageAccount.Definition.IWithCreate withCreate;
                    if (parentModel.CreatableResourceGroup() != null)
                    {
                        withCreate = withCreate = withGroup.WithNewResourceGroup(parentModel.CreatableResourceGroup());
                    }
                    else if (parentModel.ResourceGroup() != null)
                    {
                        withCreate = withGroup.WithExistingResourceGroup(parentModel.ResourceGroup());
                    }
                    else
                    {
                        withCreate = withGroup.WithNewResourceGroup();
                    }
                    withCreate = withCreate
                        .WithGeneralPurposeAccountKind();

                    base.creatable = withCreate;
                }
            }
        }

        protected override CreatableModels<StorageAccountModel, IStorageAccount> CreatableModels(FluentRequestModel fluentRequestModel)
        {
            return fluentRequestModel.StorageAccountModels;
        }

        protected override string ReferencePath()
        {
            return "newStorageAccount.ref";
        }

        protected override string ReferencePrefix()
        {
            return "storageAccounts";
        }
    }
}
