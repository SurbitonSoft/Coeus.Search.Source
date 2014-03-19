// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobStatusOperation.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The job status operation.
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
    using Coeus.Search.Sdk.Messages;

    /// <summary>
    /// The job status operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "jobstatus")]
    [ExportMetadata("GetSupported", true)]
    internal class JobStatusOperation : OperationBase
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

            Guid jobId;
            if (!Guid.TryParse(id, out jobId))
            {
                return HttpResponseStore.InvalidJobIdResponse(callback);
            }

            JobStatus jobStatus;
            if (!this.ConfigurationStore.GetJobStatus(jobId, out jobStatus))
            {
                return HttpResponseStore.JobDoesNotExistResponse(callback);
            }

            return new HttpResponseMessage
                {
                    Content =
                        new ObjectContent<JobStatus>(
                        jobStatus, 
                        callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)), 
                    StatusCode = HttpStatusCode.OK
                };
        }

        #endregion
    }
}