// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenFilterFactoryBase.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using java.util;

namespace Coeus.Search.Sdk.Base
{
    using System.Collections.Generic;

    using org.apache.lucene.analysis.util;

    /// <summary>
    /// The token filter factory base.
    /// </summary>
    
    public abstract class TokenFilterFactoryBase : TokenFilterFactory
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
        public abstract bool Initialize(Dictionary<string, string> configuration);

        #endregion
    }
}