// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlAnalyzer.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The html analyzer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Analyzer.CustomAnalyzers
{
    using java.io;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.charfilter;
    using org.apache.lucene.analysis.standard;
    using org.apache.lucene.util;

    /// <summary>
    /// The html analyzer.
    /// </summary>
    internal class HtmlAnalyzer : Analyzer
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
            CharFilter charFilter = new HTMLStripCharFilter(r);
            Tokenizer source = new StandardTokenizer(Version.LUCENE_40, charFilter);
            TokenStream result = new StandardFilter(Version.LUCENE_40, source);
            return new TokenStreamComponents(source, result);
        }

        #endregion
    }
}