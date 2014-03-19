// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchProfileMissingDataStrategy.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Coeus.Search.Sdk.Settings
{
    /// <summary>
    /// The duplicate detection missing data strategy.
    /// </summary>
    public enum SearchProfileMissingDataStrategy
    {
        /// <summary>
        /// The error.
        /// </summary>
        Error, 

        /// <summary>
        /// The ignore.
        /// </summary>
        Ignore, 

        /// <summary>
        /// The treat as null.
        /// </summary>
        TreatAsNull
    }
}