// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Microsoft.Azure.Management.Storage.Fluent;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.StorageAccount
{
    public class StorageAccountModel : CreatableGroupableModel<Microsoft.Azure.Management.Storage.Fluent.StorageAccount.Definition.IWithCreate, Microsoft.Azure.Management.Storage.Fluent.StorageAccount.Definition.IWithCreate, IStorageAccount>
    {
        [JsonProperty(PropertyName = "enableOnlyBlobService")]
        public bool? EnableOnlyBlobService { get; set; }

        [JsonProperty(PropertyName = "sku")]
        public String Sku { get; set; }

        public override async Task ValidateAndResolveAsync(IAzure azure, FluentRequestModel fluentRequestModel, string propertyName, IGroupableModel parentModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.ValidateAndResolveResourceGroupAsync(azure, fluentRequestModel, propertyName, parentModel);
        }

        protected override ICreatable<IStorageAccount> ToCreatableIntern(IAzure azure)
        {
            var withRegion = azure.StorageAccounts.Define(DeriveName("stg"));
            var withGroup = withRegion.WithRegion(this.Region);
            var withCreate = SetResourceGroupAndTags(withGroup);

            if (this.EnableOnlyBlobService != null)
            {
                if (this.EnableOnlyBlobService.Value)
                {
                    withCreate.WithBlobStorageAccountKind();
                }
                else
                {
                    withCreate.WithGeneralPurposeAccountKind();
                }
            }
            if (this.Sku != null)
            {
                switch (this.Sku)
                {
                    case "PremiumLRS":
                        withCreate.WithSku(Microsoft.Azure.Management.Storage.Fluent.Models.SkuName.PremiumLRS);
                        break;
                    case "StandardGRS":
                        withCreate.WithSku(Microsoft.Azure.Management.Storage.Fluent.Models.SkuName.StandardGRS);
                        break;
                    case "StandardLRS":
                        withCreate.WithSku(Microsoft.Azure.Management.Storage.Fluent.Models.SkuName.StandardLRS);
                        break;
                    case "StandardRAGRS":
                        withCreate.WithSku(Microsoft.Azure.Management.Storage.Fluent.Models.SkuName.StandardRAGRS);
                        break;
                    case "StandardZRS":
                        withCreate.WithSku(Microsoft.Azure.Management.Storage.Fluent.Models.SkuName.StandardZRS);
                        break;
                }
            }
            SetTags(withCreate);
            return withCreate;
        }
    }
}
