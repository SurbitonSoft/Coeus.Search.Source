// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchEngine.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Search
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Coeus.Search.QueryParser.SearchProfile;
    using Coeus.Search.Sdk.Settings;

    using org.apache.lucene.analysis.miscellaneous;
    using org.apache.lucene.document;
    using org.apache.lucene.queryparser.classic;
    using org.apache.lucene.search;

    using Version = org.apache.lucene.util.Version;

    /// <summary>
    /// The search engine.
    /// </summary>
    public class SearchEngine
    {
        #region Public Methods and Operators

        /// <summary>
        /// The duplicate detection.
        /// </summary>
        /// <param name="keyValue">
        /// The key value.
        /// </param>
        /// <param name="profileName">
        /// The profile name.
        /// </param>
        /// <param name="resultsToReturn">
        /// The results to return.
        /// </param>
        /// <param name="relativeCutOff">
        /// The relative cut off.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="isearcher">
        /// The searcher
        /// </param>
        /// <param name="indexSetting">
        /// The index setting.
        /// </param>
        /// <param name="fieldsToReturn">
        /// The fields To Return.
        /// </param>
        /// <returns>
        /// The System.Collections.Generic.List`1[T -&gt; System.String].
        /// </returns>
        public static List<Dictionary<string, string>> DuplicateDetection(
            Dictionary<string, string> keyValue, 
            string profileName, 
            int resultsToReturn, 
            int relativeCutOff, 
            string id, 
            IndexSearcher[] isearcher, 
            IIndexSetting indexSetting, 
            string[] fieldsToReturn)
        {
            SearchProfileSetting searchProfileSetting;

            if (profileName == "default")
            {
                if (indexSetting.SearchProfiles.Any())
                {
                    searchProfileSetting = indexSetting.SearchProfiles.First().Value;
                }
                else
                {
                    throw new Exception("No 'default' search profile can be found.");
                }
            }
            else if (!indexSetting.SearchProfiles.TryGetValue(profileName, out searchProfileSetting))
            {
                throw new Exception("The passed profile cannot be found.");
            }

            if (relativeCutOff == 0)
            {
                relativeCutOff = searchProfileSetting.RelativeCutOff;
            }

            string searchString = SearchProfileHelpers.CreateSearchString(
                searchProfileSetting.RequiredFields, 
                keyValue, 
                (int)searchProfileSetting.MissingDataStrategy, 
                indexSetting.NullValue);

            return StandardSearcher(
                searchString, 
                resultsToReturn, 
                relativeCutOff, 
                id, 
                isearcher, 
                indexSetting.SearchAnalyzer, 
                indexSetting.AllSearchableFields, 
                fieldsToReturn);
        }

        /// <summary>
        /// The standard searcher.
        /// </summary>
        /// <param name="queryString">
        /// The query string.
        /// </param>
        /// <param name="resultsToReturn">
        /// The results to return.
        /// </param>
        /// <param name="relativeCutOff">
        /// The relative cut off.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="isearcher">
        /// The isearcher.
        /// </param>
        /// <param name="analyzer">
        /// The analyzer.
        /// </param>
        /// <param name="allSearchFields">
        /// The all search fields.
        /// </param>
        /// <param name="fieldsToReturn">
        /// The fields To Return.
        /// </param>
        /// <returns>
        /// The results
        /// </returns>
        public static List<Dictionary<string, string>> StandardSearcher(
            string queryString, 
            int resultsToReturn, 
            int relativeCutOff, 
            string id, 
            IndexSearcher[] isearcher, 
            PerFieldAnalyzerWrapper analyzer, 
            string[] allSearchFields, 
            string[] fieldsToReturn)
        {
            var result = new List<Dictionary<string, string>>();
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return result;
            }

            var searchResults = new ConcurrentDictionary<int, TopDocs>();

            var multiFieldQuery = new MultiFieldQueryParser(Version.LUCENE_40, allSearchFields, analyzer);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(queryString);

            // Escape most common lucene special characters
            stringBuilder.Replace("\\", "\\\\");
            stringBuilder.Replace("&", "\\&");
            stringBuilder.Replace("?", "\\?");
            stringBuilder.Replace("/", "\\/");
            
            Query query = multiFieldQuery.parse(stringBuilder.ToString());

            Parallel.For(
                0, 
                isearcher.Count(), 
                index => searchResults.TryAdd(index, isearcher[index].search(query, resultsToReturn)));
            KeyValuePair<int, TopDocs>[] topDocsCollection = searchResults.OrderBy(a => a.Key).ToArray();
            TopDocs totalDocs = TopDocs.merge(null, resultsToReturn, topDocsCollection.Select(a => a.Value).ToArray());
            ScoreDoc[] hits = totalDocs.scoreDocs;

            if (relativeCutOff > 0)
            {
                float maxScore = totalDocs.getMaxScore();
                foreach (ScoreDoc hit in hits)
                {
                    if (!(hit.score / maxScore * 100 >= relativeCutOff))
                    {
                        continue;
                    }

                    Document document = isearcher[hit.shardIndex].doc(hit.doc);

                    if (!string.IsNullOrWhiteSpace(id)
                        && string.Equals(document.get("id"), id, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var dict = new Dictionary<string, string>
                        {
                           { "id", document.get("id") }, { "type", document.get("type") } 
                        };
                    foreach (string field in fieldsToReturn)
                    {
                        string value = document.get(field);
                        if (value != null)
                        {
                            dict.Add(field, value);
                        }
                    }

                    dict.Add("score", hit.score.ToString(CultureInfo.InvariantCulture));
                    result.Add(dict);
                }
            }
            else
            {
                foreach (ScoreDoc hit in hits)
                {
                    Document document = isearcher[hit.shardIndex].doc(hit.doc);
                    if (!string.IsNullOrWhiteSpace(id)
                        && string.Equals(document.get("id"), id, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var dict = new Dictionary<string, string>
                        {
                           { "id", document.get("id") }, { "type", document.get("type") } 
                        };
                    foreach (string field in fieldsToReturn)
                    {
                        string value = document.get(field);
                        if (value != null)
                        {
                            dict.Add(field, value);
                        }
                    }

                    dict.Add("score", hit.score.ToString(CultureInfo.InvariantCulture));
                    result.Add(dict);
                }
            }

            return result;
        }

        #endregion
    }
}