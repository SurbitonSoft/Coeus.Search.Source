// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchProfileSettingFile.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting Ltd. 2012
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
    /// The search profile setting file.
    /// </summary>
    public class SearchProfileSettingFile
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the search profiles.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<SeachProfileSetting> SearchProfiles { get; set; }

        #endregion

        /// <summary>
        /// The seach profile setting.
        /// </summary>
        public class SeachProfileSetting
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets a value indicating whether background duplicate check.
            /// </summary>
            [JsonProperty(Required = Required.Default)]
            [DefaultValue(false)]
            public bool BackgroundDuplicateCheck { get; set; }

            /// <summary>
            /// Gets or sets the match template.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public string MatchTemplate { get; set; }

            /// <summary>
            /// Gets or sets the missing data strategy.
            /// </summary>
            [JsonProperty(Required = Required.Default)]
            [JsonConverter(typeof(StringEnumConverter))]
            [DefaultValue(SearchProfileMissingDataStrategy.Ignore)]
            public SearchProfileMissingDataStrategy MissingDataStrategy { get; set; }

            /// <summary>
            /// Gets or sets the profile name.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public string ProfileName { get; set; }

            /// <summary>
            /// Gets or sets the profile name.
            /// </summary>
            [JsonProperty(Required = Required.Default)]
            [DefaultValue(0)]
            public int RelativeCutOff { get; set; }

            #endregion
        }
    }
}