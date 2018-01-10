// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model
{
    public class ResourceCreateStatusesTable
    {
        private string requestId;
        private CloudTable table;

        /// <summary>
        /// Initializes and return ResourceCreateStatusesTable.
        /// </summary>
        /// <param name="cancellationToken">the task cancellation token</param>
        /// <returns></returns>
        public static async Task<ResourceCreateStatusesTable> CreateAsync(string requestId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connectionString = Environment.GetEnvironmentVariable("FLUENT_SERVICE_STORAGE_CONNECTION_STRING");
            if (connectionString == null)
            {
                throw new ArgumentException("Please set the environment variable 'FLUENT_SERVICE_STORAGE_CONNECTION_STRING' containing connection string to the storage account for request statuses.");
            }
            var storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("FluentRequestStates");
            await table.CreateIfNotExistsAsync();

            return new ResourceCreateStatusesTable(requestId, table);
        }

        private ResourceCreateStatusesTable(string requestId, CloudTable table)
        {
            this.requestId = requestId;
            this.table = table;
        }

        public async Task WriteInProgressAsync(string resourceIndexableKey, string resourceType, CancellationToken cancellationToken = default(CancellationToken))
        {
            var status = new ResourceCreateStatusEntity
            {
                PartitionKey = this.requestId,
                RowKey = resourceIndexableKey,
                //
                Type = resourceType,
                Status = "InProgress",
            };
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(status);
            await table.ExecuteAsync(insertOrReplaceOperation);
        }

        public async Task WriteSucceededAsync(string resourceIndexableKey, string resourceType, dynamic resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            var status = new ResourceCreateStatusEntity
            {
                PartitionKey = this.requestId,
                RowKey = resourceIndexableKey,
                //
                Type = resourceType,
                Status = "Success",
                Resource = JsonConvert.SerializeObject(resource, Formatting.Indented)
            };
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(status);
            await table.ExecuteAsync(insertOrReplaceOperation);
        }

        public async Task WriteFailedAsync(string resourceIndexableKey, string resourceType, dynamic exception, CancellationToken cancellationToken = default(CancellationToken))
        {
            var status = new ResourceCreateStatusEntity
            {
                PartitionKey = this.requestId,
                RowKey = resourceIndexableKey,
                //
                Type = resourceType,
                Status = "Failed",
                FailureMessage = JsonConvert.SerializeObject(exception, Formatting.Indented)
            };
            TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(status);
            await table.ExecuteAsync(insertOrReplaceOperation);
        }

        public async Task<TableQuerySegment<ResourceCreateStatusEntity>> GetStatusesAsync()
        {
            TableQuery<ResourceCreateStatusEntity> query = new TableQuery<ResourceCreateStatusEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, this.requestId));
            return await table.ExecuteQuerySegmentedAsync(query, null);
        }
    }

    /// <summary>
    /// Table entry containing create status of an azure resource requested
    /// to create through fluent request.
    /// </summary>
    public class ResourceCreateStatusEntity : TableEntity
    {
        [JsonProperty(PropertyName = "type")]
        public String Type { get; set; }

        [JsonProperty(PropertyName = "status")]
        public String Status { get; set; }

        [JsonProperty(PropertyName = "resource")]
        public String Resource { get; set; }

        [JsonProperty(PropertyName = "failureMessage")]
        public String FailureMessage { get; set; }
    }
}
