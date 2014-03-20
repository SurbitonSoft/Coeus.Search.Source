// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BeiderMorseFilterFactory.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The Beider Morse filter factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using java.util;

namespace Coeus.Search.Core.Analyzer.FilterFactory
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Coeus.Search.Sdk.Base;

    using org.apache.commons.codec.language.bm;
    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.phonetic;

    /// <summary>
    /// The Beider Morse filter factory.
    /// </summary>
    [Export(typeof(TokenFilterFactoryBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("FilterName", "BeiderMorseFilter")]
    internal class BeiderMorseFilterFactory : TokenFilterFactoryBase
    {
        #region Static Fields

        /// <summary>
        /// The phonetic engine.
        /// </summary>
        private static readonly PhoneticEngine PhoneticEngine = new PhoneticEngine(
            NameType.GENERIC, RuleType.APPROX, true);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="configuration">
        /// The configuration object
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public override bool Initialize(Dictionary<string, string> configuration)
        {
            return true;
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="ts">
        /// The ts.
        /// </param>
        /// <returns>
        /// The org.apache.lucene.analysis.TokenStream.
        /// </returns>
        public override TokenStream create(TokenStream ts)
        {
            return new BeiderMorseFilter(ts, PhoneticEngine);
        }

        #endregion
    }
}