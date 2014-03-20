// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenizersFactory.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The tokenizer factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Analyzer
{
    using System;

    using Coeus.Search.Core.Analyzer.TokenizerFactory;
    using Coeus.Search.Sdk.Settings;

    /// <summary>
    /// The tokenizer factory.
    /// </summary>
    internal class TokenizersFactory
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get filter.
        /// </summary>
        /// <param name="tokenizerName">
        /// The tokenizer name.
        /// </param>
        /// <returns>
        /// The org.apache.lucene.analysis.Tokenizer.
        /// </returns>
        public static org.apache.lucene.analysis.util.TokenizerFactory GetTokenizer(TokenizerType tokenizerName)
        {
            switch (tokenizerName)
            {
                case TokenizerType.KeywordTokenizer:
                    return new KeywordTokenizerFactory();
                case TokenizerType.LetterTokenizer:
                    return new LetterTokenizerFactory();
                case TokenizerType.WhitespaceTokenizer:
                    return new WhitespaceTokenizerFactory();
                case TokenizerType.LowerCaseTokenizer:
                    return new LowerCaseTokenizerFactory();
                case TokenizerType.StandardTokenizer:
                    return new StandardTokenizerFactory();
                case TokenizerType.Uax29UrlEmailTokenizer:
                    return new Uax29UrlEmailTokenizerFactory();
                case TokenizerType.PatternTokenizer:
                    throw new NotSupportedException();
                case TokenizerType.PathHierarchyTokenizer:
                    throw new NotSupportedException();
                default:
                    throw new ArgumentOutOfRangeException("tokenizerName");
            }
        }

        #endregion
    }
}