﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardTokenizerFactory.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The standard tokenizer factory.
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
    /// The standard tokenizer factory.
    /// </summary>
    internal class StandardTokenizerFactory : TokenizerFactory
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
            return new StandardTokenizer(Version.LUCENE_40, r);
        }

        #endregion
    }
}