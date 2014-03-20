// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkIndexOperation.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The bulk index operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Operations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net.Http;
    using System.Threading.Tasks.Dataflow;

    using Coeus.Search.Core.Operations.Response;
    using Coeus.Search.Sdk.Base;
    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Messages;

    /// <summary>
    /// The bulk index operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "bulkindex")]
    [ExportMetadata("PostSupported", true)]
    internal class BulkIndexOperation : OperationBase
    {
        #region Public Methods and Operators

        /// <summary>
        /// The execute method which will be called by Searcher.
        /// The class will not be initialized per request but be cached by
        /// Search Server for performance reason. So please ensure that the code does
        /// not rely on constructor or class initialization.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="verb">
        /// The verb.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The System.Net.Http.HttpResponseMessage.
        /// </returns>
        public override HttpResponseMessage Execute(
            string indexName, Dictionary<string, string> parameters, HttpVerbs verb, string callback)
        {
            string connectorName;
            if (!this.ConfigurationStore.GetConnectorForIndex(indexName, out connectorName))
            {
                return HttpResponseStore.ConnectorNotFoundResponse(callback);
            }

            Tuple<IConnectorMetaData, IConnector> connector;
            if (!ServerBase.Connectors.TryGetValue(connectorName, out connector))
            {
                return HttpResponseStore.ConnectorNotFoundResponse(callback);
            }

            if (!connector.Item1.BulkIndexSupported)
            {
                return HttpResponseStore.BulkIndexNotSupportedResponse(callback);
            }

            IIndexEngine indexEngine;
            if (!this.ConfigurationStore.GetIndexingEngineForIndex(indexName, out indexEngine))
            {
                return HttpResponseStore.MissingIndexingEngineResponse(callback);
            }

            Guid guid = Guid.NewGuid();
            this.ConfigurationStore.UpdateJobStatus(
                guid, 
                new StatusMessage
                    {
                       StatusMessageType = StatusType.Preparing, MessageHeading = "Request submitted to the connector." 
                    });

            connector.Item2.RequestQueue.SendAsync(
                new IndexRequest
                    {
                        RequestId = guid, 
                        RequestType = RequestType.BulkIndexDocuments, 
                        IndexName = indexName, 
                        IndexEngine = indexEngine
                    });

            return HttpResponseStore.JobRequestSubmittedResponse(callback, "Bulk index", guid);
        }

        #endregion
    }
}