// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaseInsensitiveKeywordAnalyzer.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The case insensitive keyword analyzer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Analyzer.CustomAnalyzers
{
    using java.io;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.core;
    using org.apache.lucene.util;

    /// <summary>
    /// The case insensitive keyword analyzer.
    /// </summary>
    internal class CaseInsensitiveKeywordAnalyzer : Analyzer
    {
        #region Methods

        /// <summary>
        /// The create components.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <returns>
        /// The org.apache.lucene.analysis.Analyzer+TokenStreamComponents.
        /// </returns>
        protected override TokenStreamComponents createComponents(string str, Reader r)
        {
            Tokenizer source = new KeywordTokenizer(r);
            TokenStream result = new LowerCaseFilter(Version.LUCENE_40, source);
            return new TokenStreamComponents(source, result);
        }

        #endregion
    }
}