// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeepWordsFilterFactory.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The keep filter factory.
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
    using org.apache.lucene.analysis.miscellaneous;
    using org.apache.lucene.analysis.util;

    using Version = org.apache.lucene.util.Version;

    /// <summary>
    /// The keep filter factory.
    /// </summary>
    [Export(typeof(TokenFilterFactoryBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("FilterName", "KeepWordsFilter")]
    internal class KeepWordsFilterFactory : TokenFilterFactoryBase
    {
        #region Fields

        /// <summary>
        /// The stop words.
        /// </summary>
        private CharArraySet keepWords;

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
                throw new Exception("'Filename' property is required by the Keepword filter.");
            }

            if (!File.Exists(ConfigurationStore.confPath + "\\analyzers\\" + filename))
            {
                throw new Exception(
                    string.Format(
                        "'Filename' specifed in the Keepword filter configuration does not exist at location: {0}.", 
                        ConfigurationStore.confPath + "\\analyzers\\" + filename));
            }

            this.keepWords = new CharArraySet(Version.LUCENE_40, 100, true);

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

                    this.keepWords.add(line.Trim());
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
            var keepFilter = new KeepWordFilter(false, ts, this.keepWords);
            keepFilter.setEnablePositionIncrements(false);
            return keepFilter;
        }

        #endregion
    }
}