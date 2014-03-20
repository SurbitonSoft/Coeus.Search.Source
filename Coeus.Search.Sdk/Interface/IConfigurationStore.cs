// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationStore.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Interface
{
    using System;
    using System.Collections.Generic;

    using Coeus.Search.Sdk.Messages;
    using Coeus.Search.Sdk.Settings;

    using org.apache.lucene.analysis.miscellaneous;

    /// <summary>
    /// The ConfigurationStore interface.
    /// </summary>
    public interface IConfigurationStore
    {
        #region Public Properties

        /// <summary>
        /// Gets the extension path.
        /// </summary>
        string ConfPath { get; }

        /// <summary>
        /// Gets the data path.
        /// </summary>
        string DataPath { get; }

        /// <summary>
        /// Gets the extension path.
        /// </summary>
        string PluginsPath { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The fields meta data for an index.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <returns>
        /// The System.bool.
        /// </returns>
        bool FieldsMetaDataForIndex(string indexName, out List<IndexField> fields);

        /// <summary>
        /// Get the default analyzer associated with the current index
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="perFieldAnalyzerWrapper">
        /// The per Field Analyzer Wrapper.
        /// </param>
        /// <returns>
        /// The System.bool.
        /// </returns>
        bool GetIndexAnalyzerForIndex(string indexName, out PerFieldAnalyzerWrapper perFieldAnalyzerWrapper);

        /// <summary>
        /// Get the default analyzer associated with the current index
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="perFieldAnalyzerWrapper">
        /// The per Field Analyzer Wrapper.
        /// </param>
        /// <returns>
        /// The System.bool.
        /// </returns>
        bool GetSearchAnalyzerForIndex(string indexName, out PerFieldAnalyzerWrapper perFieldAnalyzerWrapper);

        ///// <summary>
        ///// Get computed settings for an index
        ///// </summary>
        ///// <param name="indexName">
        ///// The index name.
        ///// </param>
        ///// <param name="indexComputedSettings">
        ///// The index Computed Settings.
        ///// </param>
        ///// <returns>
        ///// The System.bool.
        ///// </returns>
        // bool GetComputedSettingsForIndex(string indexName, out IndexComputedSettings indexComputedSettings);

        /// <summary>
        /// Get the default connector for the index.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="connectorName">
        /// The connector Name.
        /// </param>
        /// <returns>
        /// The System.bool.
        /// </returns>
        bool GetConnectorForIndex(string indexName, out string connectorName);

        /// <summary>
        /// The get index setting.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="indexSettings">
        /// The index Settings.
        /// </param>
        /// <returns>
        /// The System.bool.
        /// </returns>
        bool GetIndexSetting(string indexName, out IIndexSetting indexSettings);

        /// <summary>
        /// Get the default indexing engine for the index.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="indexEngine">
        /// The index Engine.
        /// </param>
        /// <returns>
        /// The System.bool.
        /// </returns>
        bool GetIndexingEngineForIndex(string indexName, out IIndexEngine indexEngine);

        /// <summary>
        /// The get job status.
        /// </summary>
        /// <param name="jobId">
        /// The job id.
        /// </param>
        /// <param name="jobStatus">
        /// The job Status.
        /// </param>
        /// <returns>
        /// The System.bool.
        /// </returns>
        bool GetJobStatus(Guid jobId, out JobStatus jobStatus);

        /// <summary>
        /// The index exists or not.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        bool IndexExists(string indexName);

        /// <summary>
        /// The update job status.
        /// </summary>
        /// <param name="jobId">
        /// The job id.
        /// </param>
        /// <param name="statusMessage">
        /// The status message.
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        bool UpdateJobStatus(Guid jobId, StatusMessage statusMessage);

        #endregion
    }
}