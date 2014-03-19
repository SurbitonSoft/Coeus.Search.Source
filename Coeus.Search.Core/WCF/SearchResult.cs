// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResult.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The search result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.WCF
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// The search result.
    /// </summary>
    [DataContract]
    [Obfuscation(Exclude = true)]
    public class SearchResult
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the search results.
        /// </summary>
        [DataMember]
        public List<KeyValuePair<string, string>> SearchResults { get; set; }

        #endregion
    }
}