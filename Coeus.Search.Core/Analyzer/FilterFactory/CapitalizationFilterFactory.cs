// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CapitalizationFilterFactory.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The capitalization filter factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using java.util;

namespace Coeus.Search.Core.Analyzer.FilterFactory
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Coeus.Search.Sdk.Base;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.miscellaneous;

    /// <summary>
    /// The capitalization filter factory.
    /// </summary>
    [Export(typeof(TokenFilterFactoryBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("FilterName", "CapitalizationFilter")]
    internal class CapitalizationFilterFactory : TokenFilterFactoryBase
    {
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
            return new CapitalizationFilter(ts);
        }

        #endregion
    }
}