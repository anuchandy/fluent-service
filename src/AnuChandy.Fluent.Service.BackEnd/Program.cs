// Copyright (c) AnuChandy (https://github.com/anuchandy). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using AnuChandy.Fluent.Service.Model;
using AnuChandy.Fluent.Service.Model.Core.Channel;
using Microsoft.Azure.Management.Compute.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core.ResourceActions;
using Microsoft.Azure.Management.Storage.Fluent;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AnuChandy.Fluent.Service.BackEnd
{
    /// <summary>
    /// Back-end service entry-point.
    /// </summary>
    class Program
    {
        private static string FailureRowKey = "11111111-1111-1111-1111-111111111111";
        private static string AZURE_AUTH_LOCATION = "AZURE_AUTH_LOCATION";

        static void Main(string[] args)
        {
            if (Environment.GetEnvironmentVariable(AZURE_AUTH_LOCATION) == null)
            {
                throw new ArgumentException("Please set the environment variable 'AZURE_AUTH_LOCATION'");
            }

            ServiceClientTracing.AddTracingInterceptor(new TracingInterceptor());
            ServiceClientTracing.IsEnabled = true;

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            (new Program()).RunAsync(tokenSource.Token).Wait();
        }

        /// <summary>
        /// The message loop to process fluent requests from REST endpoint.
        /// </summary>
        /// <param name="cancellationToken">task cancellation token</param>
        /// <returns></returns>
        private async Task RunAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.WriteLine("\nStarting Message loop...!\n");

            var channel = await RequestChannel.CreateAsync(cancellationToken);
            while (!cancellationToken.IsCancellationRequested)
            {
                var nextRequest = await channel.TryReadNextAsync(cancellationToken);
                if (nextRequest != null)
                {
                    Console.WriteLine($"Received request: {nextRequest.RequestId}");
                    //
                    // Don't call await here. Since this call is happening in the message loop
                    // the parent method will not be returned. By not calling await, we can 
                    // prcoess requests concurrenlty. With await the requests will be processed
                    // async-serially.
                    //
                    HandleRequestAsync(nextRequest.RequestId, nextRequest.RequestModel, cancellationToken);
                }
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }

            Console.WriteLine("\nMessage loop Stopped...!\n");

        }

        /// <summary>
        /// Handle a fluent request from the REST endpoint.
        /// </summary>
        /// <param name="requestId">the request id</param>
        /// <param name="fluentRequestModel">the fluent request</param>
        /// <param name="cancellationToken">task cancellation token</param>
        /// <returns>task representing the request handling</returns>
        private async Task HandleRequestAsync(string requestId, FluentRequestModel fluentRequestModel, CancellationToken cancellationToken = default(CancellationToken))
        {
            IAzure azure = GetAzure();
            var progressReport = new ResourceCreateProgressReport(await ResourceCreateStatusesTable.CreateAsync(requestId, cancellationToken));

            Dictionary<string, List<dynamic>> rootCreatablesDict = new Dictionary<string, List<dynamic>>();
            try
            {
                await fluentRequestModel.ValidateAndResolveAsync(azure, fluentRequestModel, "", new DefaultGroupableModel(azure), cancellationToken);
                rootCreatablesDict = fluentRequestModel.ResolveRootCreatables(azure);
            }
            catch(Exception exception)
            {
                await progressReport.FailedAsync(FailureRowKey, null, "Error", exception, cancellationToken);
                return;
            }

            foreach (var rootCreatableDictEntry in rootCreatablesDict)
            {
                var resourceType = rootCreatableDictEntry.Key;
                foreach (var rootCreatable in rootCreatableDictEntry.Value)
                {
                    switch(resourceType)
                    {
                        case ResourceCollectionType.ResourceGroups:
                            await InvokeCreateIngoreErrorAsync((ICreatable<IResourceGroup>)rootCreatable, progressReport, cancellationToken);
                            break;

                        case ResourceCollectionType.PublicIPAddresses:
                            await InvokeCreateIngoreErrorAsync((ICreatable<IPublicIPAddress>)rootCreatable, progressReport, cancellationToken);
                            break;

                        case ResourceCollectionType.Networks:
                            await InvokeCreateIngoreErrorAsync((ICreatable<INetwork>)rootCreatable, progressReport, cancellationToken);
                            break;

                        case ResourceCollectionType.NetworkSecurityGroups:
                            await InvokeCreateIngoreErrorAsync((ICreatable<INetworkSecurityGroup>)rootCreatable, progressReport, cancellationToken);
                            break;

                        case ResourceCollectionType.NetworkInterfaces:
                            await InvokeCreateIngoreErrorAsync((ICreatable<INetworkInterface>)rootCreatable, progressReport, cancellationToken);
                            break;

                        case ResourceCollectionType.StorageAccounts:
                            await InvokeCreateIngoreErrorAsync((ICreatable<IStorageAccount>)rootCreatable, progressReport, cancellationToken);
                            break;

                        case ResourceCollectionType.VirtualMachines:
                            await InvokeCreateIngoreErrorAsync((ICreatable<IVirtualMachine>)rootCreatable, progressReport, cancellationToken);
                            break;

                        default:
                            await progressReport.FailedAsync(FailureRowKey, null, "Error", new InvalidOperationException($"Unknown creatable type {resourceType}"), cancellationToken);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Invokes CreateAsync(..) on the given creatable.
        /// </summary>
        /// <typeparam name="FluentT">type of fluent resource that creatable creates</typeparam>
        /// <param name="creatable">the creatable</param>
        /// <param name="progressReport">to report status of the create operation</param>
        /// <param name="cancellation">the ask cancellation token</param>
        /// <returns>task representing the CreateAsync invocation</returns>
        private async Task InvokeCreateIngoreErrorAsync<FluentT>(ICreatable<FluentT> creatable, 
            ResourceCreateProgressReport progressReport, 
            CancellationToken cancellation = default(CancellationToken))
        {
            try
            {
                await creatable.CreateAsync(progressReport, cancellation);
            }
            catch (Exception ex)
            {
                // Ok to catch since any error will be reported to client via progressReport
                //
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <returns>the azure client to prepare creatable defintions and
        /// creating and retrieving resources</returns>
        private static IAzure GetAzure()
        {
            var credentials = SdkContext
                .AzureCredentialsFactory
                .FromFile(Environment.GetEnvironmentVariable(AZURE_AUTH_LOCATION));

            return Microsoft.Azure.Management.Fluent.Azure
                    .Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.BodyAndHeaders)
                    .Authenticate(credentials)
                    .WithDefaultSubscription();
        }
    }
}
