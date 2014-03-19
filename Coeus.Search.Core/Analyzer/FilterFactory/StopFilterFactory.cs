// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopFilterFactory.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The stop filter factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using java.util;

namespace Coeus.Search.Core.Analyzer.FilterFactory
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;

    using Coeus.Search.Sdk.Base;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.core;
    using org.apache.lucene.analysis.util;

    using Version = org.apache.lucene.util.Version;

    /// <summary>
    /// The stop filter factory.
    /// </summary>
    [Export(typeof(TokenFilterFactoryBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("FilterName", "stopfilter")]
    internal sealed class StopFilterFactory : TokenFilterFactoryBase
    {
        #region Fields

        /// <summary>
        /// The stop words.
        /// </summary>
        private CharArraySet stopWords;

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
            string filename;
            if (!configuration.TryGetValue("filename", out filename))
            {
                throw new Exception("'Filename' property is required by the Stopword filter.");
            }

            if (!File.Exists(ConfigurationStore.confPath + "\\analyzers\\" + filename))
            {
                throw new Exception(
                    string.Format(
                        "'Filename' specifed in the Stopword filter configuration does not exist at location: {0}.", 
                        ConfigurationStore.confPath + "\\analyzers\\" + filename));
            }

            this.stopWords = new CharArraySet(Version.LUCENE_40, 100, true);
            using (var sr = new StreamReader(ConfigurationStore.confPath + "\\analyzers\\" + filename))
            {
                while (sr.Peek() >= 0 && !sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    // Ignore blank lines
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    this.stopWords.add(line.Trim());
                }
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
            var stopFilter = new StopFilter(Version.LUCENE_40, ts, this.stopWords);
            stopFilter.setEnablePositionIncrements(false);
            return stopFilter;
        }

        #endregion
    }
}