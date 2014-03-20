// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpResponseStore.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The http response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Operations.Response
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Coeus.Search.Sdk.Helpers;
    using Coeus.Search.Sdk.Messages;

    /// <summary>
    /// The http response.
    /// </summary>
    internal static class HttpResponseStore
    {
        #region Methods

        /// <summary>
        /// The bulk index not supported response.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        internal static HttpResponseMessage BulkIndexNotSupportedResponse(string callback)
        {
            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        "Error Code 101: The requested operation does not support bulk indexing.", 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        /// <summary>
        /// The connector not found response.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        internal static HttpResponseMessage ConnectorNotFoundResponse(string callback)
        {
            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        "Error Code 100: No connector is registered for the provided index. Please check the configuration file and setup a connector for the index.", 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        /// <summary>
        /// The invalid job id response.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        internal static HttpResponseMessage InvalidJobIdResponse(string callback)
        {
            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        "The paased in job id is not in a valid format (Invalid Guid).", 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        /// <summary>
        /// The job does not exist response.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        internal static HttpResponseMessage JobDoesNotExistResponse(string callback)
        {
            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        "There is no job associated with the passed 'id'.", 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        /// <summary>
        /// The job request submitted response.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="jobId">
        /// The job id.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        internal static HttpResponseMessage JobRequestSubmittedResponse(
            string callback, string operationName, Guid jobId)
        {
            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<JobCreationMessage>(
                        new JobCreationMessage
                            {
                                Message = string.Format("{0} request submitted to the connector.", operationName), 
                                JobId = jobId
                            }, 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.OK
                };
        }

        /// <summary>
        /// The missing index setting response.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        internal static HttpResponseMessage MissingIndexSettingResponse(string callback)
        {
            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        "Error 202: The request cannot be processed at this point of time. Please try again after some time. Root cause: Missing Index Settings.", 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        /// <summary>
        /// The missing indexing engine response.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        internal static HttpResponseMessage MissingIndexingEngineResponse(string callback)
        {
            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        "Error 201: The request cannot be processed at this point of time. Please try again after some time. Root cause: Missing Indexing engine.", 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        /// <summary>
        /// The missing parameter response.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        internal static HttpResponseMessage MissingParameterResponse(string callback, string parameters)
        {
            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        string.Format(
                            "The following mandatory parameters {0} are required by the operation.", parameters), 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        /// <summary>
        /// The request submitted response.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        internal static HttpResponseMessage RequestSubmittedResponse(string callback, string request)
        {
            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        string.Format("{0} request is submitted to the indexer.", request), 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.OK
                };
        }

        /// <summary>
        /// The bulk index not supported response.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponseMessage"/>.
        /// </returns>
        internal static HttpResponseMessage SingleIndexingNotSupportedResponse(string callback)
        {
            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<string>(
                        "Error Code 102: The requested operation does not support incremental indexing.", 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.BadRequest
                };
        }

        #endregion
    }
}