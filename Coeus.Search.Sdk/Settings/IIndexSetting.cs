// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIndexSetting.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Settings
{
    using System.Collections.Generic;

    using Coeus.Search.Sdk.Interface;

    using org.apache.lucene.analysis.miscellaneous;

    /// <summary>
    /// The index setting.
    /// </summary>
    public interface IIndexSetting : IComputedConfiguration
    {
        #region Public Properties

        /// <summary>
        /// Gets the all fields.
        /// </summary>
        List<IndexField> AllFields { get; }

        /// <summary>
        /// Gets the all searchable fields.
        /// </summary>
        string[] AllSearchableFields { get; }

        /// <summary>
        /// Gets the base fields.
        /// </summary>
        List<IndexField> BaseFields { get; }

        /// <summary>
        /// Gets the buffer capacity.
        /// </summary>
        int BufferCapacity { get; }

        /// <summary>
        /// Gets the commit time.
        /// </summary>
        int CommitTime { get; }

        /// <summary>
        /// Gets the default connector name.
        /// </summary>
        string DefaultConnectorName { get; }

        /// <summary>
        /// Gets the analyzer.
        /// </summary>
        PerFieldAnalyzerWrapper IndexAnalyzer { get; }

        /// <summary>
        /// Gets the index name.
        /// </summary>
        string IndexName { get; }

        /// <summary>
        /// Gets a value indicating whether is database style index.
        /// </summary>
        bool IsDatabaseStyleIndex { get; }

        /// <summary>
        /// Gets the max degree of parallelism.
        /// </summary>
        int MaxDegreeOfParallelism { get; }

        /// <summary>
        /// Gets the null value.
        /// </summary>
        string NullValue { get; }

        /// <summary>
        /// Gets the ram buffer size max.
        /// </summary>
        int RamBufferSizeMax { get; }

        /// <summary>
        /// Gets the ram buffer size min.
        /// </summary>
        int RamBufferSizeMin { get; }

        /// <summary>
        /// Gets the refresh time.
        /// </summary>
        int RefreshTime { get; }

        /// <summary>
        /// Gets the analyzer.
        /// </summary>
        PerFieldAnalyzerWrapper SearchAnalyzer { get; }

        /// <summary>
        /// Gets the search profiles.
        /// </summary>
        Dictionary<string, SearchProfileSetting> SearchProfiles { get; }

        /// <summary>
        /// Gets the shards.
        /// </summary>
        int Shards { get; }

        #endregion
    }
}