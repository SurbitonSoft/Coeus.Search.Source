// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WordDelimiterFilterFactory.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The word delimiter filter factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using java.util;

namespace Coeus.Search.Core.Analyzer.FilterFactory
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Coeus.Search.Sdk.Base;

    using org.apache.lucene.analysis;

    /// <summary>
    /// The word delimiter filter factory.
    /// </summary>
    [Export(typeof(TokenFilterFactoryBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("FilterName", "WordDelimiterFilter")]
    internal class WordDelimiterFilterFactory : TokenFilterFactoryBase
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override TokenStream create(TokenStream ts)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}