// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexRequest.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Messages
{
    using System;

    using Coeus.Search.Sdk.Interface;

    /// <summary>
    /// The index request class.
    /// </summary>
    public class IndexRequest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the index engine.
        /// </summary>
        public IIndexEngine IndexEngine { get; set; }

        /// <summary>
        /// Gets or sets the index name.
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// Gets or sets the request type.
        /// </summary>
        public RequestType RequestType { get; set; }

        #endregion
    }
}