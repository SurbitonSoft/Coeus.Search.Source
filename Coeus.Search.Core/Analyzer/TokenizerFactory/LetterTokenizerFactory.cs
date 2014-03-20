// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LetterTokenizerFactory.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The letter tokenizer factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using java.util;

namespace Coeus.Search.Core.Analyzer.TokenizerFactory
{
    using java.io;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.core;
    using org.apache.lucene.analysis.util;
    using org.apache.lucene.util;

    /// <summary>
    /// The letter tokenizer factory.
    /// </summary>
    internal class LetterTokenizerFactory : TokenizerFactory
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="r">
        /// The r.
        /// </param>
        /// <returns>
        /// The org.apache.lucene.analysis.Tokenizer.
        /// </returns>
        public override Tokenizer create(Reader r)
        {
            return new LetterTokenizer(Version.LUCENE_40, r);
        }

        #endregion
    }
}