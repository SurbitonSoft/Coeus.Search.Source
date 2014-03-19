// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchProfileOperation.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The live duplicate operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Operations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
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
    /// The live duplicate operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "searchprofile")]
    [ExportMetadata("GetSupported", true)]
    internal class SearchProfileOperation : OperationBase
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
            string queryString;
            var keyValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!parameters.TryGetValue("querystring", out queryString))
            {
                return HttpResponseStore.MissingParameterResponse(callback, "'querystring'");
            }

            try
            {
                string[] splits = queryString.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (
                    string split in splits.Where(split => split.IndexOf(":", StringComparison.OrdinalIgnoreCase) != -1))
                {
                    keyValue.Add(
                        split.Substring(0, split.IndexOf(":", StringComparison.OrdinalIgnoreCase)).Trim(), 
                        split.Substring(split.IndexOf(":", StringComparison.OrdinalIgnoreCase) + 1).Trim());
                }
            }
            catch (Exception e)
            {
                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<string>(
                            string.Format("Unable to parse the passed query string. Root cause: {0}", e.Message), 
                            callback != null ? new JsonpMediaTypeFormatter(callback) : new JsonMediaTypeFormatter()), 
                        StatusCode = HttpStatusCode.BadRequest
                    };
            }

            string profilename;
            if (!parameters.TryGetValue("profilename", out profilename))
            {
                profilename = "default";
            }

            IIndexEngine engine;
            if (!this.ConfigurationStore.GetIndexingEngineForIndex(indexName, out engine))
            {
                return HttpResponseStore.MissingIndexingEngineResponse(callback);
            }

            IIndexSetting indexSetting;
            if (!this.ConfigurationStore.GetIndexSetting(indexName, out indexSetting))
            {
                return HttpResponseStore.MissingIndexSettingResponse(callback);
            }

            var result = new List<Dictionary<string, string>>();

            string resultstoreturn;
            int totalResults = 10;
            if (parameters.TryGetValue("resultstoreturn", out resultstoreturn))
            {
                if (!int.TryParse(resultstoreturn, out totalResults))
                {
                    totalResults = 10;
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

            string id;
            parameters.TryGetValue("id", out id);

            string returnfields;
            string[] fieldsToReturn = parameters.TryGetValue("fieldsToReturn", out returnfields)
                                          ? returnfields.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                          : indexSetting.AllSearchableFields;
            try
            {
                if (engine != null)
                {
                    result = SearchEngine.DuplicateDetection(
                        keyValue, 
                        profilename, 
                        totalResults, 
                        relativeCutOffScore, 
                        id, 
                        engine.IndexSearcher, 
                        indexSetting, 
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

        #endregion
    }
}