// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynonymFilterFactory.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The synonym filter factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using java.util;

namespace Coeus.Search.Core.Analyzer.FilterFactory
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;

    using Coeus.Search.Sdk.Base;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.synonym;
    using org.apache.lucene.util;

    /// <summary>
    /// The synonym filter factory.
    /// </summary>
    [Export(typeof(TokenFilterFactoryBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportMetadata("FilterName", "SynonymFilter")]
    internal class SynonymFilterFactory : TokenFilterFactoryBase
    {
        #region Fields

        /// <summary>
        /// The map.
        /// </summary>
        private SynonymMap map;

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
                throw new Exception("'Filename' property is required by the Synonym filter.");
            }

            if (!File.Exists(ConfigurationStore.confPath + "\\analyzers\\" + filename))
            {
                throw new Exception(
                    string.Format(
                        "'Filename' specifed in the Synonym filter configuration does not exist at location: {0}.", 
                        ConfigurationStore.confPath + "\\analyzers\\" + filename));
            }

            var builder = new SynonymMap.Builder(false);
            using (var sr = new StreamReader(ConfigurationStore.confPath + "\\analyzers\\" + filename))
            {
                while (sr.Peek() >= 0 && !sr.EndOfStream)
                {
                    string readLine = sr.ReadLine();
                    if (readLine == null)
                    {
                        continue;
                    }

                    string line = readLine.ToLowerInvariant();

                    // Ignore blank lines
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    string[] values = line.Split(new[] { ":", "," }, StringSplitOptions.RemoveEmptyEntries);
                    if (!values.Any())
                    {
                        continue;
                    }

                    string key = values[0];
                    foreach (string value in values.Skip(1))
                    {
                        builder.add(new CharsRef(key), new CharsRef(value), true);
                    }
                }
            }

            this.map = builder.build();
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
            return new SynonymFilter(ts, this.map, true);
        }

        #endregion
    }
}