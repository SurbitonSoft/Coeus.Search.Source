// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpsRequiredHandler.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The https required handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Handlers
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// The https required handler.
    /// </summary>
    internal class HttpsRequiredHandler : DelegatingHandler
    {
        #region Methods

        /// <summary>
        /// The send async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The System.Threading.Tasks.Task`1[TResult -&gt; System.Net.Http.HttpResponseMessage].
        /// </returns>
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.Equals(request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                throw new HttpResponseException(
                    new HttpResponseMessage
                        {
                            Content =
                                new ObjectContent<string>(
                                "Https is required to access the server.", new JsonMediaTypeFormatter()), 
                            StatusCode = HttpStatusCode.Forbidden
                        });
            }

            return base.SendAsync(request, cancellationToken);
        }

        #endregion
    }
}