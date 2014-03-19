// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexDocumentRequest.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Messages
{
    using System.Collections.Generic;

    /// <summary>
    /// The document request type.
    /// </summary>
    public enum DocumentRequestType
    {
        /// <summary>
        /// The create.
        /// </summary>
        Create, 

        /// <summary>
        /// The update.
        /// </summary>
        Update, 

        /// <summary>
        /// The delete.
        /// </summary>
        Delete, 

        ///// <summary>
        ///// The commit.
        ///// </summary>
        //Commit, 

        ///// <summary>
        ///// The delete all.
        ///// </summary>
        //DeleteAll, 

        ///// <summary>
        ///// The start bulk indexing.
        ///// </summary>
        //StartBulkIndexing, 

        ///// <summary>
        ///// The end bulk indexing.
        ///// </summary>
        //EndBulkIndexing
    }

    /// <summary>
    /// The index document request.
    /// </summary>
    public class IndexDocumentRequest
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        public Dictionary<string, string> Document { get; set; }

        /// <summary>
        /// Gets or sets the document id.
        /// </summary>
        public string DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the document request type.
        /// </summary>
        public DocumentRequestType DocumentRequestType { get; set; }

        #endregion
    }
}