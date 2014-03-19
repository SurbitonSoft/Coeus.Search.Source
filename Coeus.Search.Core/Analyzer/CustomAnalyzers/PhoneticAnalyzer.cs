// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhoneticAnalyzer.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The phonetic analyzer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Analyzer.CustomAnalyzers
{
    using java.io;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.core;
    using org.apache.lucene.analysis.phonetic;
    using org.apache.lucene.analysis.standard;
    using org.apache.lucene.util;

    /// <summary>
    /// The phonetic analyzer.
    /// </summary>
    internal class PhoneticAnalyzer : Analyzer
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
            Tokenizer source = new StandardTokenizer(Version.LUCENE_40, r);

            // first normalize the StandardTokenizer
            TokenStream result = new StandardFilter(Version.LUCENE_40, source);

            // makes sure everything is lower case
            result = new LowerCaseFilter(Version.LUCENE_40, result);
            result = new DoubleMetaphoneFilter(result, 4, false);
            return new TokenStreamComponents(source, result);
        }

        #endregion
    }
}