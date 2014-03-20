// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexField.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Settings
{
    using System;

    /// <summary>
    /// The index fields.
    /// </summary>
    public class IndexField
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexField"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="displayName">
        /// The display name.
        /// </param>
        /// <param name="isPrimary">
        /// The is primary.
        /// </param>
        /// <param name="indexAnalyzerName">
        /// The analyzer name.
        /// </param>
        /// <param name="searchAnalyzerName">
        /// Search Analyzer name 
        /// </param>
        /// <param name="baseFieldName">
        /// The base field name.
        /// </param>
        /// <param name="columnOrder">
        /// The column order.
        /// </param>
        /// <param name="dataType">
        /// The data type.
        /// </param>
        /// <param name="isAdditionalAnalysisField">
        /// The is Additional Analysis Field.
        /// </param>
        /// <param name="store">
        /// The store.
        /// </param>
        public IndexField(
            string name,
            string displayName,
            bool isPrimary,
            string indexAnalyzerName,
            string searchAnalyzerName,
            string baseFieldName,
            IndexFieldDataType dataType,
            bool isAdditionalAnalysisField,
            bool store)
        {
            this.IsAdditionalAnalysisField = isAdditionalAnalysisField;
            this.Name = name;
            this.IsPrimary = isPrimary;
            this.DisplayName = displayName;
            this.DataType = dataType;
            this.BaseFieldName = baseFieldName;
            this.IndexAnalyzerName = indexAnalyzerName;
            this.Store = store;
            if (string.Equals(indexAnalyzerName, "noanalysis", StringComparison.OrdinalIgnoreCase))
            {
                this.DoNotIndexField = true;
            }

            this.SearchAnalyzerName = string.IsNullOrWhiteSpace(searchAnalyzerName)
                                          ? this.IndexAnalyzerName
                                          : searchAnalyzerName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the base field name.
        /// </summary>
        public string BaseFieldName { get; private set; }

        /// <summary>
        /// Gets the data type.
        /// </summary>
        public IndexFieldDataType DataType { get; private set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether do not analyze field.
        /// </summary>
        public bool DoNotAnalyzeField { get; private set; }

        /// <summary>
        /// Gets a value indicating whether do not index field.
        /// </summary>
        public bool DoNotIndexField { get; private set; }

        /// <summary>
        /// Gets the analyzer name.
        /// </summary>
        public string IndexAnalyzerName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is additional analysis field.
        /// </summary>
        public bool IsAdditionalAnalysisField { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is primary.
        /// </summary>
        public bool IsPrimary { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the analyzer name.
        /// </summary>
        public string SearchAnalyzerName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether store.
        /// </summary>
        public bool Store { get; private set; }

        #endregion
    }
}