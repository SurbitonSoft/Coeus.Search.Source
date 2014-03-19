// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectorMetaOperation.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The connector meta operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Operations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Coeus.Search.Core.Operations.Response;
    using Coeus.Search.Sdk.Base;
    using Coeus.Search.Sdk.Helpers;
    using Coeus.Search.Sdk.Interface;

    /// <summary>
    /// The connector meta operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "connectormeta")]
    [ExportMetadata("GetSupported", true)]
    internal class ConnectorMetaOperation : OperationBase
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

            try
            {
                object meta = connector.Item2.MetaData(indexName);
                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<object>(
                            meta, 
                            callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                        StatusCode = HttpStatusCode.OK
                    };
            }
            catch
            {
                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<string>(
                            "No meta data information found for the connector.", 
                            callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                        StatusCode = HttpStatusCode.BadRequest
                    };
            }
        }

        #endregion
    }
}