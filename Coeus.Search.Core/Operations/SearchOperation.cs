// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchOperation.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The search operation.
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
    using Coeus.Search.Sdk.Search;
    using Coeus.Search.Sdk.Settings;

    /// <summary>
    /// The search operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "search")]
    [ExportMetadata("GetSupported", true)]
    internal class SearchOperation : OperationBase
    {

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
            string queryString;
            if (!parameters.TryGetValue("querystring", out queryString))
            {
                HttpResponseStore.MissingParameterResponse(callback, "'querystring'");
            }

            IIndexEngine engine;
            if (!this.ConfigurationStore.GetIndexingEngineForIndex(indexName, out engine))
            {
                HttpResponseStore.MissingIndexingEngineResponse(callback);
            }

            IIndexSetting indexSetting;
            if (!this.ConfigurationStore.GetIndexSetting(indexName, out indexSetting))
            {
                HttpResponseStore.MissingIndexSettingResponse(callback);
            }

            string resultstoreturn;
            int totalResults = 50;
            if (parameters.TryGetValue("resultstoreturn", out resultstoreturn))
            {
                if (!int.TryParse(resultstoreturn, out totalResults))
                {
                    totalResults = 50;
                }
            }

            string relativeCutOff;
            int relativeCutOffScore = 0;
            if (parameters.TryGetValue("relativecutoff", out relativeCutOff))
            {
                if (!int.TryParse(relativeCutOff, out relativeCutOffScore))
                {
                    relativeCutOffScore = 0;
                }
            }

            string returnfields;
            string[] fieldsToReturn = parameters.TryGetValue("fieldsToReturn", out returnfields)
                                          ? returnfields.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                          : indexSetting.AllSearchableFields;
            try
            {
                var result = new List<Dictionary<string, string>>();
                if (engine != null)
                {
                    result = SearchEngine.StandardSearcher(
                        queryString, 
                        totalResults, 
                        relativeCutOffScore, 
                        null, 
                        engine.IndexSearcher, 
                        indexSetting.SearchAnalyzer, 
                        indexSetting.AllSearchableFields, 
                        fieldsToReturn);
                }

                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<List<Dictionary<string, string>>>(
                            result, 
                            callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                        StatusCode = HttpStatusCode.OK
                    };
            }
            catch (Exception e)
            {
                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<string>(
                            e.Message, 
                            callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                        StatusCode = HttpStatusCode.BadRequest
                    };
            }
        }

    }
}