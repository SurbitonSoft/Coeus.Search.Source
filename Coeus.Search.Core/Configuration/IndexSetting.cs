// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexSetting.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The index setting.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;

    using Coeus.Search.Configuration.Setting;
    using Coeus.Search.Core.Analyzer;
    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Settings;

    using org.apache.lucene.analysis.miscellaneous;

    /// <summary>
    /// The index setting.
    /// </summary>
    internal class IndexSetting : IIndexSetting
    {
        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="IndexSetting"/> class from being created.
        /// </summary>
        private IndexSetting()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the all fields.
        /// </summary>
        public List<IndexField> AllFields { get; private set; }

        /// <summary>
        /// Gets the all searchable fields.
        /// </summary>
        public string[] AllSearchableFields { get; private set; }

        /// <summary>
        /// Gets the base fields.
        /// </summary>
        public List<IndexField> BaseFields { get; private set; }

        /// <summary>
        /// Gets the buffer capacity.
        /// </summary>
        public int BufferCapacity { get; private set; }

        /// <summary>
        /// Gets the commit time.
        /// </summary>
        public int CommitTime { get; private set; }

        /// <summary>
        /// Gets the default connector name.
        /// </summary>
        public string DefaultConnectorName { get; private set; }

        /// <summary>
        /// Gets the analyzer.
        /// </summary>
        public PerFieldAnalyzerWrapper IndexAnalyzer { get; private set; }

        /// <summary>
        /// Gets the index name.
        /// </summary>
        public string IndexName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is database style index.
        /// </summary>
        public bool IsDatabaseStyleIndex { get; private set; }

        /// <summary>
        /// Gets the max degree of parallelism.
        /// </summary>
        public int MaxDegreeOfParallelism { get; private set; }

        /// <summary>
        /// Gets the null value.
        /// </summary>
        public string NullValue { get; private set; }

        /// <summary>
        /// Gets the ram buffer size max.
        /// </summary>
        public int RamBufferSizeMax { get; private set; }

        /// <summary>
        /// Gets the ram buffer size min.
        /// </summary>
        public int RamBufferSizeMin { get; private set; }

        /// <summary>
        /// Gets the read permissions.
        /// </summary>
        public HashSet<string> ReadPermissions { get; private set; }

        /// <summary>
        /// Gets the refresh time.
        /// </summary>
        public int RefreshTime { get; private set; }

        /// <summary>
        /// Gets the analyzer.
        /// </summary>
        public PerFieldAnalyzerWrapper SearchAnalyzer { get; private set; }

        /// <summary>
        /// Gets the search profiles.
        /// </summary>
        public Dictionary<string, SearchProfileSetting> SearchProfiles { get; private set; }

        /// <summary>
        /// Gets the shards.
        /// </summary>
        public int Shards { get; private set; }

        /// <summary>
        /// Gets the write permissions.
        /// </summary>
        public HashSet<string> WritePermissions { get; private set; }

        #endregion

        /// <summary>
        /// The builder.
        /// </summary>
        internal sealed class Builder
        {
            #region Fields

            /// <summary>
            /// The configuration.
            /// </summary>
            private IndexSetting indexSetting = new IndexSetting();

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// The build.
            /// </summary>
            /// <returns>
            /// The Coeus.Search.Sdk.Settings.IndexSetting.
            /// </returns>
            public IndexSetting Build()
            {
                IndexSetting returnSettings = this.indexSetting;
                this.indexSetting = null;
                return returnSettings;
            }

            /// <summary>
            /// The index configuration file.
            /// </summary>
            /// <param name="directoryPath">
            /// The directory path.
            /// </param>
            /// <returns>
            /// The Coeus.Search.Core.Configuration.Configuration+Builder.
            /// </returns>
            public Builder IndexConfigurationFile(string directoryPath)
            {
                if (File.Exists(directoryPath + "\\index_settings.json"))
                {
                    string directoryName = Path.GetFileName(directoryPath);
                    if (directoryName != null)
                    {
                        this.ValidateIndexConfiguration(directoryPath + "\\index_settings.json");

                        if (this.indexSetting != null && this.indexSetting.AllFields != null)
                        {
                            this.indexSetting.IndexName = directoryName;
                            this.indexSetting.IndexAnalyzer =
                                PerFieldAnalyzerFactory.GetPerFieldAnalyzer(this.indexSetting.AllFields, true);
                            this.indexSetting.SearchAnalyzer =
                                PerFieldAnalyzerFactory.GetPerFieldAnalyzer(this.indexSetting.AllFields, false);
                            this.indexSetting.BaseFields =
                                this.indexSetting.AllFields.Where(a => !a.IsAdditionalAnalysisField).ToList();
                            this.indexSetting.AllSearchableFields =
                                this.indexSetting.AllFields.Where(a => !a.DoNotIndexField).Select(b => b.Name).ToArray();

                            // Load security information if any
                            if (ServerBase.GlobalSetting.ClientCredentialType == HttpClientCredentialType.Ntlm
                                || ServerBase.GlobalSetting.ClientCredentialType == HttpClientCredentialType.Windows)
                            {
                                this.LoadSecuritySettings(directoryPath);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception(
                        "index_settings.json is not present in the directory: " + directoryPath
                        + "\\index_settings.json");
                }

                return this;
            }

            /// <summary>
            /// The search profiles.
            /// </summary>
            /// <param name="directoryPath">
            /// The directory Path.
            /// </param>
            /// <returns>
            /// The Coeus.Search.Core.Configuration.Configuration+Builder.
            /// </returns>
            public Builder SearchProfiles(string directoryPath)
            {
                if (File.Exists(directoryPath + "\\searchprofile_settings.json"))
                {
                    this.ValidateSearchProfilesConfiguration(directoryPath + "\\searchprofile_settings.json");
                }

                return this;
            }

            #endregion

            #region Methods

            /// <summary>
            /// The get computed configuration.
            /// </summary>
            /// <param name="configurationItems">
            /// The configuration items.
            /// </param>
            private void GetIndexConfiguration(IndexSettingFile configurationItems)
            {
                this.indexSetting.AllFields = new List<IndexField>();

                // Initialize global properties
                this.indexSetting.BufferCapacity = configurationItems.BufferCapacity;
                this.indexSetting.MaxDegreeOfParallelism = configurationItems.MaxDegreeOfParallelism;
                this.indexSetting.RamBufferSizeMin = configurationItems.RamBufferSizeMin;
                this.indexSetting.RamBufferSizeMax = configurationItems.RamBufferSizeMax;
                this.indexSetting.DefaultConnectorName = configurationItems.DefaultConnectorName;
                this.indexSetting.NullValue = configurationItems.NullValue;
                this.indexSetting.Shards = configurationItems.Shards;
                this.indexSetting.CommitTime = configurationItems.CommitTime;
                this.indexSetting.RefreshTime = configurationItems.RefreshTime;
                this.indexSetting.IsDatabaseStyleIndex = configurationItems.IsDatabaseStyleIndex;

                // Check if the default connector exists in the system
                Tuple<IConnectorMetaData, IConnector> connector;
                if (!ServerBase.Connectors.TryGetValue(configurationItems.DefaultConnectorName, out connector))
                {
                    throw new Exception(
                        string.Format(
                            "The specified connector: {0} is not defined in the system.", 
                            configurationItems.DefaultConnectorName));
                }

                // Initialize fields
                foreach (IndexSettingFile.Field configurationItem in configurationItems.Fields)
                {
                    // Check if the passed analyzer exists in the system.
                    try
                    {
                        AnalyzerFactory.GetAnalyzer(configurationItem.Analyzer);
                    }
                    catch
                    {
                        throw new Exception(
                            string.Format(
                                "The specified analyzer: {0} is not defined in the system. Please specify the custom analyzer using analyzer configuration file.", 
                                configurationItem.Analyzer));
                    }

                    var field = new IndexField(
                        configurationItem.Name, 
                        configurationItem.DisplayName, 
                        configurationItem.IsPrimary, 
                        configurationItem.Analyzer, 
                        configurationItem.SearchAnalyzer, 
                        configurationItem.BaseField, 
                        configurationItem.DataType, 
                        false, 
                        configurationItem.Store);

                    // Addd the base field
                    this.indexSetting.AllFields.Add(field);
                }

                if (configurationItems.ComputedFields == null)
                {
                    return;
                }

                // Initialize fields
                foreach (IndexSettingFile.Field configurationItem in configurationItems.ComputedFields)
                {
                    IndexField baseField =
                        this.indexSetting.AllFields.FirstOrDefault(
                            b => string.Equals(b.Name, configurationItem.BaseField, StringComparison.OrdinalIgnoreCase));

                    if (baseField != null)
                    {
                        var field = new IndexField(
                            configurationItem.Name, 
                            configurationItem.DisplayName, 
                            false, 
                            configurationItem.Analyzer, 
                            configurationItem.SearchAnalyzer, 
                            configurationItem.BaseField, 
                            baseField.DataType, 
                            true, 
                            configurationItem.Store);

                        // Addd the base field
                        this.indexSetting.AllFields.Add(field);
                    }
                    else
                    {
                        throw new Exception(
                            string.Format(
                                "The base field: {0} for the computed field: {1} is not defined in the index configuration file.", 
                                configurationItem.BaseField, 
                                configurationItem.Name));
                    }
                }
            }

            /// <summary>
            /// The get computed configuration.
            /// </summary>
            /// <param name="configurationItems">
            /// The configuration items.
            /// </param>
            private void GetSearchProfilesConfiguration(SearchProfileSettingFile configurationItems)
            {
                this.indexSetting.SearchProfiles =
                    new Dictionary<string, SearchProfileSetting>(StringComparer.OrdinalIgnoreCase);
                foreach (SearchProfileSettingFile.SeachProfileSetting searchProfileSetting in
                    configurationItems.SearchProfiles)
                {
                    var searchProfile = new SearchProfileSetting(
                        searchProfileSetting.ProfileName, 
                        searchProfileSetting.MissingDataStrategy, 
                        searchProfileSetting.MatchTemplate, 
                        searchProfileSetting.RelativeCutOff, 
                        this.indexSetting.AllFields);
                    this.indexSetting.SearchProfiles.Add(searchProfile.ProfileName, searchProfile);
                }
            }

            /// <summary>
            /// The load security settings.
            /// </summary>
            /// <param name="directoryPath">
            /// The directory path.
            /// </param>
            private void LoadSecuritySettings(string directoryPath)
            {
                if (File.Exists(directoryPath + "\\security_read.txt"))
                {
                    this.indexSetting.ReadPermissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    using (var file = new StreamReader(directoryPath + "\\security_read.txt"))
                    {
                        string line;
                        while ((line = file.ReadLine()) != null)
                        {
                            this.indexSetting.ReadPermissions.Add(line);
                        }
                    }
                }
                else
                {
                    throw new Exception(
                        "'security_read.txt' is not present in the directory: " + directoryPath + "\\security_read.txt."
                        + "The file is required as the server is set to use an authentication mode.");
                }

                if (File.Exists(directoryPath + "\\security_write.txt"))
                {
                    this.indexSetting.WritePermissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    using (var file = new StreamReader(directoryPath + "\\security_write.txt"))
                    {
                        string line;
                        while ((line = file.ReadLine()) != null)
                        {
                            this.indexSetting.WritePermissions.Add(line);
                        }
                    }
                }
                else
                {
                    throw new Exception(
                        "'security_write.txt' is not present in the directory: " + directoryPath
                        + "\\security_write.txt."
                        + "The file is required as the server is set to use an authentication mode.");
                }
            }

            /// <summary>
            /// The validate configuration.
            /// </summary>
            /// <param name="configurationPath">
            /// The configuration Path.
            /// </param>
            private void ValidateIndexConfiguration(string configurationPath)
            {
                IndexSettingFile configurationItems = JsonSettingBase<IndexSettingFile>.LoadFromFile(configurationPath);
                this.GetIndexConfiguration(configurationItems);
            }

            /// <summary>
            /// The validate configuration.
            /// </summary>
            /// <param name="configurationPath">
            /// The configuration Path.
            /// </param>
            private void ValidateSearchProfilesConfiguration(string configurationPath)
            {
                SearchProfileSettingFile configurationItems =
                    JsonSettingBase<SearchProfileSettingFile>.LoadFromFile(configurationPath);
                this.GetSearchProfilesConfiguration(configurationItems);
            }

            #endregion
        }
    }
}