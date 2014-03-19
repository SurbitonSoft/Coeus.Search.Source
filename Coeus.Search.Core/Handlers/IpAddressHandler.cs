// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpAddressHandler.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The ip address handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Handlers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// The ip address handler.
    /// </summary>
    internal class IpAddressHandler : DelegatingHandler
    {
        #region Static Fields

        /// <summary>
        /// The ip address.
        /// </summary>
        internal static HashSet<string> IpAddress;

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
        /// <exception cref="HttpResponseException">
        /// </exception>
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var remoteEndpointMessageProperty =
                (RemoteEndpointMessageProperty)
                request.Properties["System.ServiceModel.Channels.RemoteEndpointMessageProperty"];

            if (!IpAddress.Contains(remoteEndpointMessageProperty.Address))
            {
                throw new HttpResponseException(
                    new HttpResponseMessage
                        {
                            Content =
                                new ObjectContent<string>(
                                "The request's ip address is not is allowed list.", new JsonMediaTypeFormatter()), 
                            StatusCode = HttpStatusCode.Forbidden
                        });
            }

            return base.SendAsync(request, cancellationToken);
        }

        #endregion
    }
}