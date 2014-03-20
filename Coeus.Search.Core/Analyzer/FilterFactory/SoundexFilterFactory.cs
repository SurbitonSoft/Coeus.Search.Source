// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoundexFilterFactory.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The soundex filter factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using java.util;

namespace Coeus.Search.Core.Analyzer.FilterFactory
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Coeus.Search.Sdk.Base;

    using org.apache.commons.codec.language;
    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.phonetic;

    /// <summary>
    /// The soundex filter factory.
    /// </summary>
    [Export(typeof(TokenFilterFactoryBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("FilterName", "SoundexFilter")]
    internal class SoundexFilterFactory : TokenFilterFactoryBase
    {
        #region Static Fields

        /// <summary>
        /// The soundex.
        /// </summary>
        private static readonly Soundex Soundex = new Soundex();

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
            return new PhoneticFilter(ts, Soundex, false);
        }

        #endregion
    }
}