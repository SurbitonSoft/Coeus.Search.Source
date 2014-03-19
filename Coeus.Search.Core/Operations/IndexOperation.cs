// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexOperation.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The index operation.
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
    /// The index operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "index")]
    [ExportMetadata("PostSupported", true)]
    internal class IndexOperation : OperationBase
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
            string id;
            if (!parameters.TryGetValue("id", out id))
            {
                HttpResponseStore.MissingParameterResponse(callback, "'id'");
            }

            string connectorName;
            if (!this.ConfigurationStore.GetConnectorForIndex(indexName, out connectorName))
            {
                HttpResponseStore.ConnectorNotFoundResponse(callback);
            }

            Tuple<IConnectorMetaData, IConnector> connector;
            if (!ServerBase.Connectors.TryGetValue(connectorName, out connector))
            {
                return HttpResponseStore.ConnectorNotFoundResponse(callback);
            }

            if (!connector.Item1.IncrementalIndexingSupported)
            {
                return HttpResponseStore.SingleIndexingNotSupportedResponse(callback);
            }

            IIndexEngine indexEngine;
            if (!this.ConfigurationStore.GetIndexingEngineForIndex(indexName, out indexEngine))
            {
                return HttpResponseStore.MissingIndexingEngineResponse(callback);
            }

            connector.Item2.RequestQueue.SendAsync(
                new IndexRequest
                    {
                        RequestType = RequestType.IndexDocument, 
                        Id = id, 
                        IndexName = indexName, 
                        IndexEngine = indexEngine
                    });

            return HttpResponseStore.RequestSubmittedResponse(callback, "Document indexing");
        }

        #endregion
    }
}