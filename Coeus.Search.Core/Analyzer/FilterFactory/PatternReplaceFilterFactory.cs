// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PatternReplaceFilterFactory.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The pattern replace filter factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using java.util;

namespace Coeus.Search.Core.Analyzer.FilterFactory
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Coeus.Search.Sdk.Base;

    using java.util.regex;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.pattern;

    /// <summary>
    /// The pattern replace filter factory.
    /// </summary>
    [Export(typeof(TokenFilterFactoryBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("FilterName", "PatternReplaceFilter")]
    internal class PatternReplaceFilterFactory : TokenFilterFactoryBase
    {
        #region Fields

        /// <summary>
        /// The pattern.
        /// </summary>
        private Pattern pattern;

        /// <summary>
        /// The replace text.
        /// </summary>
        private string replaceText;

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
            string patternText;
            if (!configuration.TryGetValue("pattern", out patternText))
            {
                throw new Exception("'pattern' value is required by Pattern Replace filter.");
            }

            this.pattern = Pattern.compile(patternText, Pattern.DOTALL);
            string replacementText;
            if (!configuration.TryGetValue("replacementtext", out replacementText))
            {
                throw new Exception("'replacementtext' value is required by Pattern Replace filter.");
            }

            this.replaceText = replacementText;
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
            return new PatternReplaceFilter(ts, this.pattern, this.replaceText, true);
        }

        #endregion
    }
}