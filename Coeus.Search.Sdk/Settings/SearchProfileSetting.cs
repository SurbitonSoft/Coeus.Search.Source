// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchProfileSetting.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Coeus.Search.QueryParser.SearchProfile;

    using Newtonsoft.Json;

    /// <summary>
    /// The search profile setting.
    /// </summary>
    public class SearchProfileSetting
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchProfileSetting"/> class.
        /// </summary>
        /// <param name="profileName">
        /// The profile name.
        /// </param>
        /// <param name="missingDataStrategy">
        /// The missing data strategy.
        /// </param>
        /// <param name="matchTemplate">
        /// The match template.
        /// </param>
        /// <param name="relativeCutOff">
        /// The relative Cut Off.
        /// </param>
        /// <param name="allIndexedFields">
        /// The all Indexed Fields.
        /// </param>
        public SearchProfileSetting(
            string profileName,
            SearchProfileMissingDataStrategy missingDataStrategy,
            string matchTemplate,
            int relativeCutOff,
            List<IndexField> allIndexedFields)
        {
            this.ProfileName = profileName;
            this.MissingDataStrategy = missingDataStrategy;
            this.MatchTemplate = matchTemplate;
            this.RelativeCutOff = relativeCutOff;
            this.GenerateRequiredFields();
            this.GenerateRequiredFieldsDetails();
            this.AllFieldsDetails = allIndexedFields;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the all fields details.
        /// </summary>
        public List<IndexField> AllFieldsDetails { get; private set; }

        /// <summary>
        /// Gets the match template.
        /// </summary>
        public string MatchTemplate { get; private set; }

        /// <summary>
        /// Gets the missing data strategy.
        /// </summary>
        public SearchProfileMissingDataStrategy MissingDataStrategy { get; private set; }

        /// <summary>
        /// Gets the profile name.
        /// </summary>
        public string ProfileName { get; private set; }

        /// <summary>
        /// Gets or sets the relative cut off.
        /// </summary>
        public int RelativeCutOff { get; set; }

        /// <summary>
        /// Gets the required fields.
        /// </summary>
        [JsonIgnore]
        public List<Token> RequiredFields { get; private set; }

        /// <summary>
        /// Gets the display fields
        /// </summary>
        public List<string> RequiredFieldsDetails { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The generate required fields.
        /// </summary>
        private void GenerateRequiredFields()
        {
            // Generate the required fields
            var searchProfileParser = new SearchProfileParser();
            List<Token> tokens;
            if (searchProfileParser.Parse(this.MatchTemplate, out tokens))
            {
                this.RequiredFields = tokens;
            }
            else
            {
                // TODO; Capture better error handling
                throw new ArgumentException("Match string is wrong");
            }
        }

        /// <summary>
        /// The generate required fields details.
        /// </summary>
        private void GenerateRequiredFieldsDetails()
        {
            this.RequiredFieldsDetails = new List<string>();
            foreach (var requiredField in this.RequiredFields.Where(a => a.Type == Token.TokenType.Field).Select(b => b.FieldName).Distinct())
            {
                this.RequiredFieldsDetails.Add(requiredField);
            }
        }

        #endregion
    }
}