// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeywordTokenizerFactory.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The keyword tokenizer factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using java.util;
using org.apache.lucene.util;

namespace Coeus.Search.Core.Analyzer.TokenizerFactory
{
    using java.io;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.core;
    using org.apache.lucene.analysis.util;

    /// <summary>
    /// The keyword tokenizer factory.
    /// </summary>
    internal class KeywordTokenizerFactory : TokenizerFactory
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
            return new KeywordTokenizer(r);
        }

        #endregion
    }
}