// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexMetaOperation.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The index meta operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Operations
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Coeus.Search.Sdk.Base;
    using Coeus.Search.Sdk.Helpers;
    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Settings;

    /// <summary>
    /// The index meta operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "indexmeta")]
    [ExportMetadata("GetSupported", true)]
    internal class IndexMetaOperation : OperationBase
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
            IIndexSetting indexSetting;
            if (this.ConfigurationStore.GetIndexSetting(indexName, out indexSetting))
            {
                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<List<IndexField>>(
                            indexSetting.AllFields, 
                            callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                        StatusCode = HttpStatusCode.OK
                    };
            }

            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        "No meta data information found for the index.", 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        #endregion
    }
}