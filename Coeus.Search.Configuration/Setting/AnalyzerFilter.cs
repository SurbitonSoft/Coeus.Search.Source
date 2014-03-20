// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalyzerFilter.cs" company="Coeus Application Services">
//   Coeus Application Services 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Configuration.Setting
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The analyzer filter.
    /// </summary>
    public class AnalyzerFilter
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzerFilter"/> class.
        /// </summary>
        public AnalyzerFilter()
        {
            this.Parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the filter name.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string FilterName { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        #endregion
    }
}