// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationStore.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The configuration store.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.Runtime.Caching;

    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Messages;
    using Coeus.Search.Sdk.Settings;

    using org.apache.lucene.analysis.miscellaneous;

    /// <summary>
    /// The configuration store.
    /// </summary>
    [Export(typeof(IConfigurationStore))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class ConfigurationStore : IConfigurationStore
    {
        #region Static Fields

        /// <summary>
        /// The index connectors collection.
        /// </summary>
        internal static readonly ConcurrentDictionary<string, string> IndexConnectorsCollection =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The index settings collection.
        /// </summary>
        internal static readonly ConcurrentDictionary<string, IIndexSetting> IndexSettingsCollection =
            new ConcurrentDictionary<string, IIndexSetting>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The indexing engine collection.
        /// </summary>
        internal static readonly ConcurrentDictionary<string, IIndexEngine> IndexingEngineCollection =
            new ConcurrentDictionary<string, IIndexEngine>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The conf path.
        /// </summary>
        internal static string confPath;

        /// <summary>
        /// The data path.
        /// </summary>
        internal static string dataPath;

        /// <summary>
        /// The plugins path.
        /// </summary>
        internal static string pluginsPath;

        #endregion

        #region Fields

        /// <summary>
        /// The job store.
        /// </summary>
        private readonly MemoryCache jobStore = MemoryCache.Default;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the extension path.
        /// </summary>
        public string ConfPath
        {
            get
            {
                return confPath;
            }
        }

        /// <summary>
        /// Gets the data path.
        /// </summary>
        public string DataPath
        {
            get
            {
                return dataPath;
            }
        }

        /// <summary>
        /// Gets the extension path.
        /// </summary>
        public string PluginsPath
        {
            get
            {
                return pluginsPath;
            }
        }

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
        public bool FieldsMetaDataForIndex(string indexName, out List<IndexField> fields)
        {
            IIndexSetting indexSetting;
            if (IndexSettingsCollection.TryGetValue(indexName, out indexSetting))
            {
                fields = indexSetting.AllFields;
                return true;
            }

            fields = default(List<IndexField>);
            return false;
        }

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
        public bool GetConnectorForIndex(string indexName, out string connectorName)
        {
            return IndexConnectorsCollection.TryGetValue(indexName, out connectorName);
        }

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
        public bool GetIndexAnalyzerForIndex(string indexName, out PerFieldAnalyzerWrapper perFieldAnalyzerWrapper)
        {
            IIndexSetting indexSetting;
            if (IndexSettingsCollection.TryGetValue(indexName, out indexSetting))
            {
                perFieldAnalyzerWrapper = indexSetting.IndexAnalyzer;
                return true;
            }

            perFieldAnalyzerWrapper = default(PerFieldAnalyzerWrapper);
            return false;
        }

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
        public bool GetIndexSetting(string indexName, out IIndexSetting indexSettings)
        {
            return IndexSettingsCollection.TryGetValue(indexName.ToLowerInvariant(), out indexSettings);
        }

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
        public bool GetIndexingEngineForIndex(string indexName, out IIndexEngine indexEngine)
        {
            return IndexingEngineCollection.TryGetValue(indexName, out indexEngine);
        }

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
        public bool GetJobStatus(Guid jobId, out JobStatus jobStatus)
        {
            if (this.jobStore.Contains(jobId.ToString()))
            {
                jobStatus = (JobStatus)this.jobStore.Get(jobId.ToString());
                return true;
            }

            jobStatus = default(JobStatus);
            return false;
        }

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
        public bool GetSearchAnalyzerForIndex(string indexName, out PerFieldAnalyzerWrapper perFieldAnalyzerWrapper)
        {
            IIndexSetting indexSetting;
            if (IndexSettingsCollection.TryGetValue(indexName, out indexSetting))
            {
                perFieldAnalyzerWrapper = indexSetting.SearchAnalyzer;
                return true;
            }

            perFieldAnalyzerWrapper = default(PerFieldAnalyzerWrapper);
            return false;
        }

        /// <summary>
        /// The index exists or not.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public bool IndexExists(string indexName)
        {
            IIndexSetting indexSettings;
            return IndexSettingsCollection.TryGetValue(indexName, out indexSettings);
        }

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
        public bool UpdateJobStatus(Guid jobId, StatusMessage statusMessage)
        {
            try
            {
                if (this.jobStore.Contains(jobId.ToString()))
                {
                    var jobStatus = (JobStatus)this.jobStore.Get(jobId.ToString());
                    jobStatus.StatusMessageType = statusMessage.StatusMessageType;
                    jobStatus.ProcessedRecords = statusMessage.ProcessedRecords;
                    jobStatus.MessageBody = statusMessage.MessageBody;
                    jobStatus.MessageHeading = statusMessage.MessageHeading;
                    if (statusMessage.StatusMessageType == StatusType.FinshedWithSuccess)
                    {
                        jobStatus.EndTime = DateTime.Now;
                    }

                    this.jobStore.AddOrGetExisting(jobId.ToString(), jobStatus, DateTime.Now.AddDays(1));
                    return true;
                }
                else
                {
                    var jobStatus = new JobStatus
                        {
                            StatusMessageType = statusMessage.StatusMessageType, 
                            StartTime = DateTime.Now, 
                            ProcessedRecords = 0, 
                            TotalRecords = 0
                        };

                    jobStatus.ProcessedRecords = statusMessage.ProcessedRecords;
                    jobStatus.MessageBody = statusMessage.MessageBody;
                    jobStatus.MessageHeading = statusMessage.MessageHeading;
                    this.jobStore.AddOrGetExisting(jobId.ToString(), jobStatus, DateTime.Now.AddDays(1));
                    return true;
                }
            }
            catch
            {
                Debug.Assert(true, "Memory cache should never fail.");
                return false;
            }
        }

        #endregion
    }
}