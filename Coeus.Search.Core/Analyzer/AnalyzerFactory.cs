// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalyzerFactory.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The analyzer factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Analyzer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Coeus.Search.Configuration.Setting;
    using Coeus.Search.Core.Analyzer.CustomAnalyzers;
    using Coeus.Search.Sdk.Settings;

    using org.apache.lucene.analysis;
    using org.apache.lucene.analysis.core;
    using org.apache.lucene.analysis.standard;
    using org.apache.lucene.analysis.util;

    using Version = org.apache.lucene.util.Version;

    /// <summary>
    /// The analyzer factory.
    /// </summary>
    internal class AnalyzerFactory
    {
        #region Static Fields

        /// <summary>
        /// The analyzers.
        /// </summary>
        private static readonly Dictionary<string, Analyzer> Analyzers =
            new Dictionary<string, Analyzer>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get all analyzer names.
        /// </summary>
        /// <returns>
        /// The System.Collections.Generic.List`1[T -&gt; System.String].
        /// </returns>
        public static List<string> GetAllAnalyzerNames()
        {
            return Analyzers.Keys.ToList();
        }

        /// <summary>
        /// The get per field analyzer.
        /// </summary>
        /// <param name="analyzerName">
        /// The fields.
        /// </param>
        /// <returns>
        /// The org.apache.lucene.analysis.AnalyzerWrapper.
        /// </returns>
        public static Analyzer GetAnalyzer(string analyzerName)
        {
            Analyzer analyzer;
            if (Analyzers.TryGetValue(analyzerName, out analyzer))
            {
                return analyzer;
            }

            throw new ArgumentException(string.Format("Requested analyzer {0} does not exist.", analyzerName));
        }

        /// <summary>
        /// The initialze analyzers.
        /// </summary>
        public static void InitialzeAnalyzers()
        {
            // Add standard analyzers to the repository
            Analyzers.Add("simple", new SimpleAnalyzer(Version.LUCENE_40));
            Analyzers.Add("standard", new StandardAnalyzer(Version.LUCENE_40));
            Analyzers.Add("html", new HtmlAnalyzer());
            Analyzers.Add("keyword", new CaseInsensitiveKeywordAnalyzer());
            Analyzers.Add("phonetic", new PhoneticAnalyzer());

            var filterFactory = new FiltersFactory();

            // Add custom analyzers
            if (!Directory.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Conf\\Analyzers"))
            {
                throw new Exception("Analyzers directory does not exist under the folder 'Conf'.");
            }

            foreach (string file in
                Directory.EnumerateFiles(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Conf\\Analyzers", "*.json"))
            {
                try
                {
                    AnalyzerSetting analyzerSetting = JsonSettingBase<AnalyzerSetting>.LoadFromFile(file);
                    GetAnalyzerConfiguration(analyzerSetting, filterFactory);
                }
                catch (Exception e)
                {
                    throw new Exception(
                        string.Format(
                            "Error initializing custom analyzer defined in file: {0}. Root cause: {1}", file, e.Message), 
                        e);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get analyzer configuration.
        /// </summary>
        /// <param name="analyzerSetting">
        /// The analyzer Setting.
        /// </param>
        /// <param name="filterFactory">
        /// </param>
        private static void GetAnalyzerConfiguration(AnalyzerSetting analyzerSetting, FiltersFactory filterFactory)
        {
            string analyzerName = analyzerSetting.AnalyzerName;
            TokenizerType tokenizerType = analyzerSetting.TokenizerType;

            org.apache.lucene.analysis.util.TokenizerFactory tokenizerFactory =
                TokenizersFactory.GetTokenizer(tokenizerType);
            List<TokenFilterFactory> tokenFilterFactories =
                analyzerSetting.Filters.Select(
                    settingFilter => filterFactory.GetFilter(settingFilter.FilterName, settingFilter.Parameters)).Cast
                    <TokenFilterFactory>().ToList();
            var customAnalyzer = new CustomAnalyzer(tokenizerFactory, tokenFilterFactories);
            Analyzers.Add(analyzerName, customAnalyzer);
        }

        #endregion
    }
}