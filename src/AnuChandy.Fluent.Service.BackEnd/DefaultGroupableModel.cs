// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model.Core;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;

namespace AnuChandy.Fluent.Service.BackEnd
{
    /// <summary>
    /// Groupable model exposing default region and resource group.
    /// </summary>
    class DefaultGroupableModel : IGroupableModel
    {
        private IAzure azure;
        private ICreatable<IResourceGroup> creatableResourceGroup;

        public DefaultGroupableModel(IAzure azure)
        {
            this.azure = azure;
        }

        public string Location()
        {
            // Default region
            //
            return "eastus2";
        }

        public ICreatable<IResourceGroup> CreatableResourceGroup()
        {
            // Default resource group
            //
            if (this.creatableResourceGroup == null)
            {
                this.creatableResourceGroup = this.azure.ResourceGroups
                    .Define(SdkContext.RandomResourceName("rg", 15))
                    .WithRegion(this.Location());
            }
            return this.creatableResourceGroup;
        }

        public IResourceGroup ResourceGroup()
        {
            // Returns null purposefully, no default exising resource gorup
            //
            return null;
        }
    }
}
