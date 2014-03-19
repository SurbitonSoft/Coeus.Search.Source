// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LengthFilterFactory.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The length filter factory.
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
    using org.apache.lucene.analysis.miscellaneous;

    /// <summary>
    /// The length filter factory.
    /// </summary>
    [Export(typeof(TokenFilterFactoryBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("FilterName", "LengthFilter")]
    internal class LengthFilterFactory : TokenFilterFactoryBase
    {
        #region Fields

        /// <summary>
        /// The max.
        /// </summary>
        private int max;

        /// <summary>
        /// The min.
        /// </summary>
        private int min;

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
            string minText;
            if (!configuration.TryGetValue("min", out minText))
            {
                throw new Exception("'min' value is required by Length filter.");
            }

            int minNumber;
            if (!int.TryParse(minText, out minNumber))
            {
                throw new Exception("Passed 'min' value is not a valid integer.");
            }

            this.min = minNumber;

            string maxText;
            if (!configuration.TryGetValue("min", out maxText))
            {
                throw new Exception("'max' value is required by Length filter.");
            }

            int maxNumber;
            if (!int.TryParse(minText, out maxNumber))
            {
                throw new Exception("Passed 'max' value is not a valid integer.");
            }

            this.max = maxNumber;
            if (this.min > this.max)
            {
                throw new Exception("'max' value cannot be lower than 'min' value.");
            }

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
            return new LengthFilter(false, ts, this.min, this.max);
        }

        #endregion
    }
}