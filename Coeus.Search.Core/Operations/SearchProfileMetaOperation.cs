// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchProfileMetaOperation.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The search profile meta operation.
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
    using Coeus.Search.Sdk.Settings;

    /// <summary>
    /// The search profile meta operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "searchprofilemeta")]
    [ExportMetadata("GetSupported", true)]
    internal class SearchProfileMetaOperation : OperationBase
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
            string profileName;
            if (!parameters.TryGetValue("profilename", out profileName))
            {
                return HttpResponseStore.MissingParameterResponse(callback, "'profilename'");
            }

            IIndexSetting indexSetting;
            if (!this.ConfigurationStore.GetIndexSetting(indexName, out indexSetting))
            {
                return HttpResponseStore.MissingIndexSettingResponse(callback);
            }

            SearchProfileSetting searchProfileSetting;
            if (profileName == "default")
            {
                if (indexSetting.SearchProfiles.Any())
                {
                    searchProfileSetting = indexSetting.SearchProfiles.First().Value;
                }
                else
                {
                    throw new Exception("No 'default' search profile can be found.");
                }
            }
            else if (!indexSetting.SearchProfiles.TryGetValue(profileName, out searchProfileSetting))
            {
                throw new Exception("The passed profile cannot be found.");
            }

            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<SearchProfileSetting>(
                        searchProfileSetting, 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.OK
                };
        }

        #endregion
    }
}