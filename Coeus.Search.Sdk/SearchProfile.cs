// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchProfile.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The match record status.
    /// </summary>
    public enum MatchRecordStatus
    {
        /// <summary>
        /// The match.
        /// </summary>
        Match, 

        /// <summary>
        /// The not match.
        /// </summary>
        NotMatch
    }

    /// <summary>
    /// The match result status.
    /// </summary>
    public enum MatchResultStatus
    {
        /// <summary>
        /// The proposed.
        /// </summary>
        Proposed, 

        /// <summary>
        /// The reviewed.
        /// </summary>
        Reviewed, 

        /// <summary>
        /// The processed.
        /// </summary>
        Processed, 

        /// <summary>
        /// The false detection.
        /// </summary>
        FalseDetection, 

        /// <summary>
        /// The time lapsed.
        /// </summary>
        TimeLapsed
    }

    /// <summary>
    /// The match results.
    /// </summary>
    public class MatchGroup
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the file sequence.
        /// </summary>
        public int FileSequence { get; set; }

        /// <summary>
        /// Gets or sets the match result.
        /// </summary>
        public List<MatchResult> MatchResults { get; set; }

        /// <summary>
        /// Gets or sets the profile name.
        /// </summary>
        public string ProfileName { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime StartTime { get; set; }

        #endregion
    }

    /// <summary>
    /// The match record.
    /// </summary>
    public class MatchRecord
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the match record id.
        /// </summary>
        public string MatchRecordId { get; set; }

        /// <summary>
        /// Gets or sets the record value.
        /// </summary>
        public string RecordValue { get; set; }

        /// <summary>
        /// Gets or sets the result status.
        /// </summary>
        public MatchRecordStatus ResultStatus { get; set; }

        #endregion
    }

    /// <summary>
    /// The match result.
    /// </summary>
    public class MatchResult
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the match record.
        /// </summary>
        public List<MatchRecord> MatchRecords { get; set; }

        /// <summary>
        /// Gets or sets the primary record id.
        /// </summary>
        public string PrimaryRecordId { get; set; }

        /// <summary>
        /// Gets or sets the record value.
        /// </summary>
        public string RecordValue { get; set; }

        /// <summary>
        /// Gets or sets the result status.
        /// </summary>
        public MatchResultStatus ResultStatus { get; set; }

        #endregion
    }
}