// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchProfiles.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Settings
{
    using System.Collections.Generic;

    using Coeus.Search.Sdk.Interface;

    /// <summary>
    /// The search profiles.
    /// </summary>
    public class SearchProfiles : IComputedConfiguration
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchProfiles"/> class.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="profiles">
        /// The profiles.
        /// </param>
        public SearchProfiles(string indexName, Dictionary<string, SearchProfileSetting> profiles)
        {
            this.Profiles = profiles;
            this.IndexName = indexName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the index name.
        /// </summary>
        public string IndexName { get; private set; }

        /// <summary>
        /// Gets the profiles.
        /// </summary>
        public Dictionary<string, SearchProfileSetting> Profiles { get; private set; }

        #endregion
    }
}