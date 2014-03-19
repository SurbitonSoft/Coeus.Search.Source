// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestType.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Messages
{
    /// <summary>
    /// The request type.
    /// </summary>
    public enum RequestType
    {
        /// <summary>
        /// The index document.
        /// </summary>
        IndexDocument, 

        /// <summary>
        /// The delete document.
        /// </summary>
        DeleteDocument, 

        /// <summary>
        /// The bulk index documents.
        /// </summary>
        BulkIndexDocuments, 

        /// <summary>
        /// The background duplicate documents.
        /// </summary>
        BackgroundDuplicateDocuments, 

        /// <summary>
        /// The interactive duplicate document.
        /// </summary>
        InteractiveDuplicateDocument, 

        /// <summary>
        /// The delete index.
        /// </summary>
        DeleteIndex, 

        /// <summary>
        /// The delete data source.
        /// </summary>
        DeleteDataSource, 

        /// <summary>
        /// The refresh.
        /// </summary>
        Refresh
    }
}