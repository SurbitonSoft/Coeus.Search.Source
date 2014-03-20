// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexSettingFile.cs" company="Coeus Application Services">
//   Coeus Application Services 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Configuration.Setting
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Coeus.Search.Sdk.Settings;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The index configuration.
    /// </summary>
    public class IndexSettingFile
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the buffer capacity.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue(1000)]
        public int BufferCapacity { get; set; }

        /// <summary>
        /// Gets or sets the commit time.
        /// </summary>
        [DefaultValue(60)]
        [JsonProperty(Required = Required.Default)]
        public int CommitTime { get; set; }

        /// <summary>
        /// Gets or sets the computed fields.
        /// </summary>
        public List<Field> ComputedFields { get; set; }

        /// <summary>
        /// Gets or sets the default connector name.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string DefaultConnectorName { get; set; }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<Field> Fields { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is database style index.
        /// </summary>
        [DefaultValue(true)]
        [JsonProperty(Required = Required.Default)]
        public bool IsDatabaseStyleIndex { get; set; }

        /// <summary>
        /// Gets or sets the max degree of parallelism.
        /// </summary>
        [DefaultValue(-1)]
        [JsonProperty(Required = Required.Default)]
        public int MaxDegreeOfParallelism { get; set; }

        /// <summary>
        /// Gets or sets the null value.
        /// </summary>
        [DefaultValue("null")]
        [JsonProperty(Required = Required.Default)]
        public string NullValue { get; set; }

        /// <summary>
        /// Gets or sets the ram buffer size max.
        /// </summary>
        [DefaultValue(1000)]
        [JsonProperty(Required = Required.Default)]
        public int RamBufferSizeMax { get; set; }

        /// <summary>
        /// Gets or sets the ram buffer size min.
        /// </summary>
        [DefaultValue(500)]
        [JsonProperty(Required = Required.Default)]
        public int RamBufferSizeMin { get; set; }

        /// <summary>
        /// Gets or sets the refresh time.
        /// </summary>
        [DefaultValue(500)]
        [JsonProperty(Required = Required.Default)]
        public int RefreshTime { get; set; }

        /// <summary>
        /// Gets or sets the shards.
        /// </summary>
        [DefaultValue(1)]
        [JsonProperty(Required = Required.Default)]
        public int Shards { get; set; }

        #endregion

        /// <summary>
        /// The field.
        /// </summary>
        public class Field
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets the analyzer.
            /// </summary>
            [JsonProperty(Required = Required.Default)]
            [DefaultValue("simple")]
            public string Analyzer { get; set; }

            /// <summary>
            /// Gets or sets the base field.
            /// </summary>
            public string BaseField { get; set; }

            /// <summary>
            /// Gets or sets the data type.
            /// </summary>
            [JsonProperty(Required = Required.Default)]
            [JsonConverter(typeof(StringEnumConverter))]
            [DefaultValue(IndexFieldDataType.String)]
            public IndexFieldDataType DataType { get; set; }

            /// <summary>
            /// Gets or sets the display name.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public string DisplayName { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is primary.
            /// </summary>
            [JsonProperty(Required = Required.Default)]
            [DefaultValue(false)]
            public bool IsPrimary { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the analyzer.
            /// </summary>
            public string SearchAnalyzer { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether store.
            /// </summary>
            [JsonProperty(Required = Required.Default)]
            [DefaultValue(true)]
            public bool Store { get; set; }

            #endregion
        }
    }
}