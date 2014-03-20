// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalyzerSetting.cs" company="Coeus Application Services">
//   Coeus Application Services 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Configuration.Setting
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Coeus.Search.Sdk.Settings;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The analyzer setting.
    /// </summary>
    [Serializable]
    public class AnalyzerSetting
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzerSetting"/> class.
        /// </summary>
        public AnalyzerSetting()
        {
            this.Filters = new List<AnalyzerFilter>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the analyzer name.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AnalyzerName { get; set; }

        /// <summary>
        /// Gets or sets the filters.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<AnalyzerFilter> Filters { get; set; }

        /// <summary>
        /// Gets or sets the tokenizer type.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(TokenizerType.StandardTokenizer)]
        public TokenizerType TokenizerType { get; set; }

        #endregion
    }
}