// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteOperation.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The delete operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Operations
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net.Http;
    using System.Threading.Tasks.Dataflow;

    using Coeus.Search.Core.Operations.Response;
    using Coeus.Search.Sdk.Base;
    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Messages;

    /// <summary>
    /// The delete operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "delete")]
    [ExportMetadata("PostSupported", true)]
    internal class DeleteOperation : OperationBase
    {
        #region Public Methods and Operators

        /// <summary>
        /// The execute.
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
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        public override HttpResponseMessage Execute(
            string indexName, Dictionary<string, string> parameters, HttpVerbs verb, string callback)
        {
            string id;
            if (!parameters.TryGetValue("id", out id))
            {
                HttpResponseStore.MissingParameterResponse(callback, "'id'");
            }

            IIndexEngine indexEngine;
            if (!this.ConfigurationStore.GetIndexingEngineForIndex(indexName, out indexEngine))
            {
                return HttpResponseStore.MissingIndexingEngineResponse(callback);
            }

            indexEngine.RequestQueue.SendAsync(
                new IndexDocumentRequest { DocumentRequestType = DocumentRequestType.Delete, DocumentId = id });

            return HttpResponseStore.RequestSubmittedResponse(callback, "Document deletion");
        }

        #endregion
    }
}