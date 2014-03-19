// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIndexEngine.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks.Dataflow;

    using Coeus.Search.Sdk.Messages;

    using org.apache.lucene.search;

    /// <summary>
    /// The IndexEngine interface.
    /// </summary>
    public interface IIndexEngine
    {
        #region Public Properties

        /// <summary>
        /// Gets the deleted documents.
        /// </summary>
        int DeletedDocuments { get; }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        string IndexName { get; }

        /// <summary>
        /// Gets the index searcher.
        /// </summary>
        IndexSearcher[] IndexSearcher { get; }

        /// <summary>
        /// Gets the ram documents.
        /// </summary>
        int RamDocuments { get; }

        /// <summary>
        /// Gets the request queue.
        /// </summary>
        BufferBlock<IndexDocumentRequest> RequestQueue { get; }

        /// <summary>
        /// Gets the total documents.
        /// </summary>
        int TotalDocuments { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The all document.
        /// </summary>
        /// <param name="totalRecordsToReturn">
        /// The total records to return.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<Dictionary<string, string>> AllDocument(int totalRecordsToReturn);

        /// <summary>
        /// The bulk indexing.
        /// </summary>
        /// <param name="prepareForBulkIndexing">
        /// The prepare for bulk indexing.
        /// </param>
        void BulkIndexing(bool prepareForBulkIndexing);

        #endregion
    }
}