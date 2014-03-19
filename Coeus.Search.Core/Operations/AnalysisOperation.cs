// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalysisOperation.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The analysis operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Operations
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Coeus.Search.Core.Analyzer;
    using Coeus.Search.Core.Operations.Response;
    using Coeus.Search.Sdk.Base;
    using Coeus.Search.Sdk.Helpers;
    using Coeus.Search.Sdk.Interface;

    using java.util.regex;

    using org.apache.lucene.analysis;
    using org.apache.lucene.queryparser.classic;
    using org.apache.lucene.search;
    using org.apache.lucene.util;

    /// <summary>
    /// The analysis operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "analysis")]
    [ExportMetadata("GetSupported", true)]
    internal class AnalysisOperation : OperationBase
    {
        #region Public Methods and Operators

        /// <summary>
        /// The execute method which will be called by Search.
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
            string analyzerName;
            if (!parameters.TryGetValue("analyzer", out analyzerName))
            {
                return HttpResponseStore.MissingParameterResponse(callback, "analyzer");
            }

            string queryString;
            if (!parameters.TryGetValue("querystring", out queryString))
            {
                return HttpResponseStore.MissingParameterResponse(callback, "querystring");
            }

            Analyzer analyzer;
            try
            {
                analyzer = AnalyzerFactory.GetAnalyzer(analyzerName);
            }
            catch
            {
                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<string>(
                            "The passed analyzer does not exist.", 
                            callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                        StatusCode = HttpStatusCode.OK
                    };
            }

            var queryParser = new QueryParser(Version.LUCENE_40, string.Empty, analyzer);
            
            Query query = queryParser.parse(queryString);

            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        query.toString(), 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.OK
                };
        }

        #endregion
    }
}