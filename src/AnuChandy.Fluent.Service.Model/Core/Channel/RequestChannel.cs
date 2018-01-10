// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.Model.Core.Channel
{
    /// <summary>
    /// Channel that supports writing and reading fluent request.
    /// </summary>
    public class RequestChannel
    {
        private CloudTable table;
        private CloudQueue queue;

        /// <summary>
        /// Initializes and return a channel.
        /// </summary>
        /// <param name="cancellationToken">the task cancellation token</param>
        /// <returns></returns>
        public static async Task<RequestChannel> CreateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var connectionString = Environment.GetEnvironmentVariable("FLUENT_SERVICE_STORAGE_CONNECTION_STRING");
            if (connectionString == null)
            {
                throw new ArgumentException("Please set the environment variable 'FLUENT_SERVICE_STORAGE_CONNECTION_STRING' containing connection string to the storage account for channel.");
            }
            var storageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("FluentRequests");
            await table.CreateIfNotExistsAsync();

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("frqueue");
            await queue.CreateIfNotExistsAsync();

            return new RequestChannel(table, queue);
        }

        /// <summary>
        /// Creates RequestChannel.
        /// </summary>
        /// <param name="table">the channel table</param>
        /// <param name="queue">the channel queue</param>
        private RequestChannel(CloudTable table, CloudQueue queue)
        {
            this.table = table;
            this.queue = queue;
        }

        /// <summary>
        /// Write the request to channel.
        /// </summary>
        /// <param name="requestModel">the fluent request model</param>
        /// <param name="cancellationToken">the task cancellation token</param>
        /// <returns>the request id</returns>
        public async Task<string> WriteAsync(FluentRequestModel requestModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            var requestId = Guid.NewGuid().ToString();
            RequestEntity requestEntity = new RequestEntity(requestId, requestModel);
            await table.ExecuteAsync(TableOperation.InsertOrReplace(requestEntity));
            await queue.AddMessageAsync(new CloudQueueMessage(requestId));
            return requestId;
        }

        /// <summary>
        /// If available, read next request from the channel.
        /// </summary>
        /// <param name="cancellationToken">the task cancellation token</param>
        /// <returns>the data read from channel if available, null if channel is empty</returns>
        public async Task<RequestChannelData> TryReadNextAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var message = await queue.GetMessageAsync();
            if (message == null)
            {
                return null;
            }
            else
            {
                var requestId = message.AsString;
                await queue.DeleteMessageAsync(message);
                TableResult result = await table.ExecuteAsync(TableOperation.Retrieve<RequestEntity>(requestId, RequestEntity.RequestRowKey));
                var requestModel = (RequestEntity)result.Result;
                return new RequestChannelData(requestId, requestModel.GetFluentModelPayload());
            }
        }
    }

    /// <summary>
    /// Represents a request data in channel.
    /// </summary>
    public class RequestChannelData
    {
        /// <summary>
        /// The request id.
        /// </summary>
        public string RequestId { get; private set; }

        /// <summary>
        /// The request payload.
        /// </summary>
        public FluentRequestModel RequestModel { get; private set; }

        /// <summary>
        /// Creates RequestChannelData.
        /// </summary>
        /// <param name="requestId">The request id.</param>
        /// <param name="requestModel">The request payload.</param>
        internal RequestChannelData(string requestId, FluentRequestModel requestModel)
        {
            this.RequestId = requestId;
            this.RequestModel = requestModel;
        }
    }

    /// <summary>
    /// Channel table entity containing the fluent request to create azure resources.
    /// </summary>
    public class RequestEntity : TableEntity {
        public static string RequestRowKey = "00000000-0000-0000-0000-000000000000";

        /// <summary>
        /// The fluent request payload in json format.
        /// </summary>
        public String Payload { get; set; }

        /// <summary>
        /// Creates RequestEntity.
        /// </summary>
        public RequestEntity()
        {
        }

        /// <returns>Gets the payload as FluentRequestModel instance</returns>
        public FluentRequestModel GetFluentModelPayload()
        {
            return JsonConvert.DeserializeObject<FluentRequestModel>(this.Payload);
        }

        /// <summary>
        /// Creates RequestEntity to write channel.
        /// </summary>
        /// <param name="requestId">The request Id</param>
        /// <param name="requestPayload">The fluent request payload</param>
        public RequestEntity(string requestId, FluentRequestModel requestPayload)
        {
            this.PartitionKey = requestId ?? throw new ArgumentNullException("requestId");
            if (requestPayload == null)
            {
                throw new ArgumentNullException("requestPayload");
            }
            this.RowKey = RequestRowKey;
            this.Payload = JsonConvert.SerializeObject(requestPayload,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
        }
    }
}
