// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WcfSearch.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The wcf search.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.WCF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Search;
    using Coeus.Search.Sdk.Settings;

    /// <summary>
    /// The wcf search.
    /// </summary>
    [Obfuscation(Exclude = true)]
    public class WcfSearch : ISearch
    {
        #region Public Methods and Operators

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The System.Collections.Generic.List`1[T -&gt; System.Collections.Generic.List`1[T -&gt; System.Collections.Generic.KeyValuePair`2[TKey -&gt; System.String, TValue -&gt; System.String]]].
        /// </returns>
        public List<List<KeyValuePair<string, string>>> Search(
            string indexName, string operationName, Dictionary<string, string> parameters)
        {
            var soapResults = new List<List<KeyValuePair<string, string>>>();
            string queryString;
            if (!parameters.TryGetValue("querystring", out queryString))
            {
                return null;
            }

            var inputKeyValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (operationName == "searchprofile")
            {
                string[] splits = queryString.Split(new[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string split in splits)
                {
                    if (split.IndexOf(":", StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        inputKeyValue.Add(
                            split.Substring(0, split.IndexOf(":", StringComparison.OrdinalIgnoreCase)).Trim(), 
                            split.Substring(split.IndexOf(":", StringComparison.OrdinalIgnoreCase) + 1).Trim());
                    }
                }
            }

            IIndexEngine engine;
            if (!ServerBase.ConfigurationStore.GetIndexingEngineForIndex(indexName, out engine))
            {
                // TODO: Error Handling
            }

            IIndexSetting indexSetting;
            if (!ServerBase.ConfigurationStore.GetIndexSetting(indexName, out indexSetting))
            {
                // TODO: Error Handling
            }

            var results = new List<Dictionary<string, string>>();

            string resultstoreturn;
            int totalResults = 10;
            if (parameters.TryGetValue("resultstoreturn", out resultstoreturn))
            {
                if (!int.TryParse(resultstoreturn, out totalResults))
                {
                    totalResults = 10;
                }
            }

            string relativeCutOff;
            int relativeCutOffScore = 0;
            if (parameters.TryGetValue("relativecutoff", out relativeCutOff))
            {
                if (!int.TryParse(resultstoreturn, out relativeCutOffScore))
                {
                    relativeCutOffScore = 0;
                }
            }

            string profilename;
            if (!parameters.TryGetValue("profilename", out profilename))
            {
                profilename = "default";
            }

            string id;
            parameters.TryGetValue("id", out id);

            string returnfields;
            string[] fieldsToReturn = parameters.TryGetValue("fieldstoreturn", out returnfields)
                                          ? returnfields.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                          : indexSetting.AllSearchableFields;

            if (engine != null)
            {
                if (operationName == "searchprofile")
                {
                    // TODO: Handle profile names
                    results = SearchEngine.DuplicateDetection(
                        inputKeyValue, 
                        profilename, 
                        totalResults, 
                        relativeCutOffScore, 
                        id, 
                        engine.IndexSearcher, 
                        indexSetting, 
                        fieldsToReturn);
                }
                else
                {
                    results = SearchEngine.StandardSearcher(
                        queryString, 
                        totalResults, 
                        relativeCutOffScore, 
                        null, 
                        engine.IndexSearcher, 
                        indexSetting.SearchAnalyzer, 
                        indexSetting.AllSearchableFields, 
                        fieldsToReturn);
                }
            }

            if (results.Count != 0)
            {
                soapResults.AddRange(
                    results.Select(
                        result =>
                        result.Select(keyValue => new KeyValuePair<string, string>(keyValue.Key, keyValue.Value)).ToList
                            ()));
            }

            return soapResults;
        }

        #endregion
    }
}