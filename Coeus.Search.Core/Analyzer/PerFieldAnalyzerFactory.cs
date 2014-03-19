// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerFieldAnalyzerFactory.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The per field analyzer factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Analyzer
{
    using System.Collections.Generic;

    using Coeus.Search.Core.Analyzer.CustomAnalyzers;
    using Coeus.Search.Sdk.Settings;

    using java.util;

    using org.apache.lucene.analysis.core;
    using org.apache.lucene.analysis.miscellaneous;
    using org.apache.lucene.util;

    /// <summary>
    /// The per field analyzer factory.
    /// </summary>
    internal class PerFieldAnalyzerFactory
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get per field analyzer.
        /// </summary>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <param name="isIndexAnalyzer">
        /// The is Index Analyzer.
        /// </param>
        /// <returns>
        /// The org.apache.lucene.analysis.miscellaneous.PerFieldAnalyzerWrapper.
        /// </returns>
        public static PerFieldAnalyzerWrapper GetPerFieldAnalyzer(List<IndexField> fields, bool isIndexAnalyzer)
        {
            Map analyzerPerField = new HashMap();
            analyzerPerField.put("id", new CaseInsensitiveKeywordAnalyzer());
            analyzerPerField.put("type", new CaseInsensitiveKeywordAnalyzer());
            foreach (IndexField field in fields)
            {
                analyzerPerField.put(
                    field.Name, 
                    isIndexAnalyzer
                        ? AnalyzerFactory.GetAnalyzer(field.IndexAnalyzerName)
                        : AnalyzerFactory.GetAnalyzer(field.SearchAnalyzerName));
            }

            return new PerFieldAnalyzerWrapper(new SimpleAnalyzer(Version.LUCENE_40), analyzerPerField);
        }

        #endregion
    }
}