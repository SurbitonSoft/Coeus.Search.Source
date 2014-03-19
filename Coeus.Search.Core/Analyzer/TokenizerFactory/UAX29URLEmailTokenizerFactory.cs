// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UAX29URLEmailTokenizerFactory.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The email tokenizer factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using java.util;

namespace Coeus.Search.Core.Analyzer.TokenizerFactory
{
    using java.io;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.standard;
    using org.apache.lucene.analysis.util;
    using org.apache.lucene.util;

    /// <summary>
    /// The email tokenizer factory.
    /// </summary>
    internal class Uax29UrlEmailTokenizerFactory : TokenizerFactory
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
            return new UAX29URLEmailTokenizer(Version.LUCENE_40, r);
        }

        #endregion
    }
}