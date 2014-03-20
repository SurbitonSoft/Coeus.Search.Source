// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomAnalyzer.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The custom analyzer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Analyzer
{
    using System.Collections.Generic;
    using System.Linq;

    using java.io;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.util;

    /// <summary>
    /// The custom analyzer.
    /// </summary>
    internal class CustomAnalyzer : Analyzer
    {
        #region Fields

        /// <summary>
        /// The token filters.
        /// </summary>
        private readonly List<TokenFilterFactory> tokenFilters;

        /// <summary>
        /// The tokenizer factory.
        /// </summary>
        private readonly org.apache.lucene.analysis.util.TokenizerFactory tokenizerFactory;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAnalyzer"/> class.
        /// </summary>
        /// <param name="tokenizerFactory">
        /// The tokenizer factory.
        /// </param>
        /// <param name="tokenFilters">
        /// The token filters.
        /// </param>
        public CustomAnalyzer(
            org.apache.lucene.analysis.util.TokenizerFactory tokenizerFactory, List<TokenFilterFactory> tokenFilters)
        {
            this.tokenizerFactory = tokenizerFactory;
            this.tokenFilters = tokenFilters;
        }

        #endregion

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
            Tokenizer source = this.tokenizerFactory.create(r);
            TokenStream result = this.tokenFilters.First().create(source);

            result = this.tokenFilters.Aggregate(result, (current, tokenFilter) => tokenFilter.create(current));

            return new TokenStreamComponents(source, result);
        }

        #endregion
    }
}