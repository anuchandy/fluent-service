// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model
{
    public class StorageUtil
    {
        public static async Task<CloudTable> GetRequestStateTableAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            CloudTableClient tableClient = CloudStorageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("FluentRequestStates");
            await table.CreateIfNotExistsAsync();
            return table;
        }

        private static CloudStorageAccount CloudStorageAccount
        {
            get
            {
                var connectionString = Environment.GetEnvironmentVariable("FLUENT_SERVICE_STORAGE_CONNECTION_STRING");
                if (connectionString == null)
                {
                    throw new ArgumentException("Please set the environment variable 'FLUENT_SERVICE_STORAGE_CONNECTION_STRING' containing connection string to the storage account to store requests state.");
                }
                return Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionString);
            }
        }
    }
}
