// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvSetting.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting Ltd. 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Configuration.Setting
{
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The csv setting.
    /// </summary>
    public class CsvSetting
    {
        #region Enums

        /// <summary>
        /// The csv text field type.
        /// </summary>
        public enum CsvTextFieldType
        {
            /// <summary>
            /// The delimited.
            /// </summary>
            /// <remarks>
            /// </remarks>
            Delimited, 

            /// <summary>
            /// The fixed width.
            /// </summary>
            /// <remarks>
            /// </remarks>
            FixedWidth, 
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the csv.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<CsvConfiguration> Csv { get; set; }

        #endregion

        /// <summary>
        /// The csv configuration.
        /// </summary>
        public class CsvConfiguration
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets the associated index name.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public string AssociatedIndexName { get; set; }

            /// <summary>
            /// Gets or sets the csv folder path.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public string CsvFolderPath { get; set; }

            /// <summary>
            /// Gets or sets the delimiter.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public string Delimiter { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether first column is key.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public bool FirstColumnIsKey { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has fields enclosed in quotes.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public bool HasFieldsEnclosedInQuotes { get; set; }

            /// <summary>
            /// Gets or sets the starting lines to ignore.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public int StartingLinesToIgnore { get; set; }

            /// <summary>
            /// Gets or sets the text field type.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            [JsonConverter(typeof(StringEnumConverter))]
            public CsvTextFieldType TextFieldType { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether trim white space.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public bool TrimWhiteSpace { get; set; }

            #endregion
        }
    }
}