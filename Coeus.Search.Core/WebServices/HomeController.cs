// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   Default controller to handle all search request
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.WebServices
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Reflection;
    using System.ServiceModel;
    using System.Web;
    using System.Web.Http;

    using Coeus.Search.Core.Configuration;
    using Coeus.Search.Sdk.Helpers;
    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Settings;

    /// <summary>
    /// Default controller to handle all search request
    /// </summary>
    //// [Authorize]
    [Obfuscation(Exclude = true)]
    public class HomeController : ApiController
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get method.
        /// </summary>
        /// <param name="indexname">
        /// The index name
        /// </param>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <returns>
        /// The System.Net.Http.HttpResponseMessage.
        /// </returns>
        [AcceptVerbs("GET")]
        public HttpResponseMessage Get([FromUri] string indexname, [FromUri] string operation)
        {
            var query = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(this.Request.RequestUri.Query))
            {
                try
                {
                    query = QueryStringParser(this.Request.RequestUri.Query);
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage
                        {
                            Content = new ObjectContent<string>(e.Message, new JsonMediaTypeFormatter()), 
                            StatusCode = HttpStatusCode.BadRequest
                        };
                }
            }

            string callback;
            query.TryGetValue("callback", out callback);

            IIndexSetting indexSetting;
            if (!ServerBase.ConfigurationStore.GetIndexSetting(indexname, out indexSetting))
            {
                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<string>(
                            "The requested index does not exist.", 
                            callback != null ? new JsonpMediaTypeFormatter(callback) : new JsonMediaTypeFormatter()), 
                        StatusCode = HttpStatusCode.BadRequest
                    };
            }

            if (ServerBase.GlobalSetting.ClientCredentialType == HttpClientCredentialType.Windows
                || ServerBase.GlobalSetting.ClientCredentialType == HttpClientCredentialType.Ntlm)
            {
                if (!this.User.Identity.IsAuthenticated)
                {
                    return new HttpResponseMessage
                        {
                            Content =
                                new ObjectContent<string>(
                                "The current user does is not authenticated.", 
                                callback != null ? new JsonpMediaTypeFormatter(callback) : new JsonMediaTypeFormatter()), 
                            StatusCode = HttpStatusCode.BadRequest
                        };
                }

                if (!((IndexSetting)indexSetting).ReadPermissions.Contains(this.User.Identity.Name))
                {
                    return new HttpResponseMessage
                        {
                            Content =
                                new ObjectContent<string>(
                                "The current user does not have read permission on the index.", 
                                callback != null ? new JsonpMediaTypeFormatter(callback) : new JsonMediaTypeFormatter()), 
                            StatusCode = HttpStatusCode.BadRequest
                        };
                }
            }

            if (operation != null)
            {
                Tuple<IOperationMetaData, IOperation> opr;
                if (ServerBase.Operations.TryGetValue(operation, out opr))
                {
                    if (opr.Item1.GetSupported)
                    {
                        return opr.Item2.Execute(indexname, query, HttpVerbs.Get, callback);
                    }

                    return new HttpResponseMessage
                        {
                            Content =
                                new ObjectContent<string>(
                                "The requested operation does not support 'GET' request.", new JsonMediaTypeFormatter()), 
                            StatusCode = HttpStatusCode.BadRequest
                        };
                }
            }

            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        "No operation is registered for the requested Uri.", 
                        callback != null ? new JsonpMediaTypeFormatter(callback) : new JsonMediaTypeFormatter()), 
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        /// <summary>
        /// Post method implementation
        /// </summary>
        /// <param name="indexname">
        /// The index name
        /// </param>
        /// <param name="operation">
        /// Different message types
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <returns>
        /// The System.Object.
        /// </returns>
        public HttpResponseMessage Post([FromUri] string indexname, [FromUri] string operation, [FromBody] object body)
        {
            var query = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(this.Request.RequestUri.Query))
            {
                try
                {
                    query = QueryStringParser(this.Request.RequestUri.Query);
                }
                catch (Exception e)
                {
                    return new HttpResponseMessage
                        {
                            Content = new ObjectContent<string>(e.Message, new JsonMediaTypeFormatter()), 
                            StatusCode = HttpStatusCode.BadRequest
                        };
                }
            }

            if (body != null)
            {
                query.Add("body", body.ToString());
            }

            IIndexSetting indexSetting;
            if (!ServerBase.ConfigurationStore.GetIndexSetting(indexname, out indexSetting))
            {
                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<string>(
                            "The requested index does not exist.", new JsonMediaTypeFormatter()), 
                        StatusCode = HttpStatusCode.BadRequest
                    };
            }

            if (ServerBase.GlobalSetting.ClientCredentialType == HttpClientCredentialType.Windows
                || ServerBase.GlobalSetting.ClientCredentialType == HttpClientCredentialType.Ntlm)
            {
                if (!this.User.Identity.IsAuthenticated)
                {
                    return new HttpResponseMessage
                        {
                            Content =
                                new ObjectContent<string>(
                                "The current user does is not authenticated.", new JsonMediaTypeFormatter()), 
                            StatusCode = HttpStatusCode.BadRequest
                        };
                }

                if (!((IndexSetting)indexSetting).WritePermissions.Contains(this.User.Identity.Name))
                {
                    return new HttpResponseMessage
                        {
                            Content =
                                new ObjectContent<string>(
                                "The current user does not have read permission on the index.", 
                                new JsonMediaTypeFormatter()), 
                            StatusCode = HttpStatusCode.BadRequest
                        };
                }
            }

            if (operation != null)
            {
                Tuple<IOperationMetaData, IOperation> opr;
                if (ServerBase.Operations.TryGetValue(operation, out opr))
                {
                    if (opr.Item1.PostSupported)
                    {
                        return opr.Item2.Execute(indexname, query, HttpVerbs.Post, null);
                    }

                    return new HttpResponseMessage
                        {
                            Content =
                                new ObjectContent<string>(
                                "The requested operation does not support 'POST' request.", new JsonMediaTypeFormatter()), 
                            StatusCode = HttpStatusCode.BadRequest
                        };
                }
            }

            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>("The requested index does not exist.", new JsonMediaTypeFormatter()), 
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        #endregion

        #region Methods

        /// <summary>
        /// The query string parser.
        /// </summary>
        /// <param name="queryString">
        /// The query string.
        /// </param>
        /// <returns>
        /// The System.Collections.Generic.Dictionary`2[TKey -&gt; System.String, TValue -&gt; System.String].
        /// </returns>
        private static Dictionary<string, string> QueryStringParser(string queryString)
        {
            var queryParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string[] querySegments = queryString.Split('&');
            foreach (string segment in querySegments)
            {
                int breakCharacterPosition = segment.IndexOf('=');
                if (breakCharacterPosition == -1)
                {
                    throw new HttpException("The passed string is not in a valid format.");
                }

                string key = segment.Substring(0, breakCharacterPosition).Trim(new[] { '?', ' ' });
                string val = HttpUtility.UrlDecode(segment.Substring(breakCharacterPosition + 1));
                queryParameters.Add(key, val);
            }

            return queryParameters;
        }

        #endregion
    }
}