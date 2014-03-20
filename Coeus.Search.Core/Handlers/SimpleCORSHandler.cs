// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleCORSHandler.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The simple cors handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Handlers
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    // Code based on: http://code.msdn.microsoft.com/Implementing-CORS-support-a677ab5d
    /// <summary>
    /// The simple cors handler.
    /// </summary>
    internal class SimpleCorsHandler : DelegatingHandler
    {
        #region Constants

        /// <summary>
        /// The access control allow headers.
        /// </summary>
        private const string accessControlAllowHeaders = "Access-Control-Allow-Headers";

        /// <summary>
        /// The access control allow methods.
        /// </summary>
        private const string accessControlAllowMethods = "Access-Control-Allow-Methods";

        /// <summary>
        /// The access control allow origin.
        /// </summary>
        private const string accessControlAllowOrigin = "Access-Control-Allow-Origin";

        /// <summary>
        /// The access control request headers.
        /// </summary>
        private const string accessControlRequestHeaders = "Access-Control-Request-Headers";

        /// <summary>
        /// The access control request method.
        /// </summary>
        private const string accessControlRequestMethod = "Access-Control-Request-Method";

        /// <summary>
        /// The origin.
        /// </summary>
        private const string origin = "Origin";

        #endregion

        #region Static Fields

        /// <summary>
        /// The allow origin list.
        /// </summary>
        internal static string AllowOriginList = "*";

        #endregion

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
            bool isCorsRequest = request.Headers.Contains(origin);
            bool isPreflightRequest = request.Method == HttpMethod.Options;

            if (isCorsRequest)
            {
                if (isPreflightRequest)
                {
                    return Task.Factory.StartNew(
                        () =>
                            {
                                var response = new HttpResponseMessage(HttpStatusCode.OK);
                                response.Headers.Add(accessControlAllowOrigin, AllowOriginList);

                                string currentAccessControlRequestMethod =
                                    request.Headers.GetValues(accessControlRequestMethod).FirstOrDefault();

                                if (currentAccessControlRequestMethod != null)
                                {
                                    response.Headers.Add(accessControlAllowMethods, currentAccessControlRequestMethod);
                                }

                                string requestedHeaders = string.Join(
                                    ", ", request.Headers.GetValues(accessControlRequestHeaders));

                                if (!string.IsNullOrEmpty(requestedHeaders))
                                {
                                    response.Headers.Add(accessControlAllowHeaders, requestedHeaders);
                                }

                                return response;
                            }, 
                        cancellationToken);
                }

                return base.SendAsync(request, cancellationToken).ContinueWith(
                    t =>
                        {
                            HttpResponseMessage resp = t.Result;
                            resp.Headers.Add(accessControlAllowOrigin, AllowOriginList);

                            return resp;
                        });
            }

            return base.SendAsync(request, cancellationToken);
        }

        #endregion
    }
}