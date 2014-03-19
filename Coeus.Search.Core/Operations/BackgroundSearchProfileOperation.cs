// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSearchProfileOperation.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The background search profile operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Operations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using System.Xml.Serialization;

    using Coeus.Search.Sdk;
    using Coeus.Search.Sdk.Base;
    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Settings;

    using org.apache.lucene.search;

    /// <summary>
    /// The background search profile operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "backgroundsearchprofile")]
    [ExportMetadata("GetSupported", true)]
    internal class BackgroundSearchProfileOperation : OperationBase
    {
        #region Public Methods and Operators

        /// <summary>
        /// The execute method which will be called by Searcher.
        /// The class will not be initialized per request but be cached by
        /// Searcher Server for performance reason. So please ensure that the code does
        /// not rely on constructor or class initialization.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="verb">
        /// The verb.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The System.Net.Http.HttpResponseMessage.
        /// </returns>
        public override HttpResponseMessage Execute(
            string indexName, Dictionary<string, string> parameters, HttpVerbs verb, string callback)
        {
            string profileName;
            if (!parameters.TryGetValue("profileName", out profileName))
            {
                // TODO; throw error
            }

            IIndexEngine indexEngine;
            this.ConfigurationStore.GetIndexingEngineForIndex(indexName, out indexEngine);

            IIndexSetting indexSetting;
            this.ConfigurationStore.GetIndexSetting(indexName, out indexSetting);

            // var isearcher = new IndexSearcher(indexEngine.IndexReader);
            IndexSearcher[] isearcher = indexEngine.IndexSearcher;
            var matchResults = new MatchGroup
                {
                    MatchResults = new List<MatchResult>(), 
                    StartTime = DateTime.Now, 
                    ProfileName = profileName, 
                    FileSequence = 1
                };
            var x = new XmlSerializer(typeof(MatchGroup));
            string filePath = Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location))
                              + "\\Reports\\Match Sessions\\";
            string fileName = indexName + "-" + profileName + "-" + DateTime.Now.Ticks + "-" + "Result Set ";

            var recordWriter = new ActionBlock<MatchResult>(
                request =>
                    {
                        matchResults.MatchResults.Add(request);
                        if (matchResults.MatchResults.Count == 100)
                        {
                            TextWriter textWriter =
                                new StreamWriter(filePath + fileName + matchResults.FileSequence + ".xml");
                            matchResults.FileSequence++;
                            x.Serialize(textWriter, matchResults);
                            matchResults.MatchResults.Clear();
                        }
                    }, 
                new ExecutionDataflowBlockOptions
                    {
                       MaxDegreeOfParallelism = 1, BoundedCapacity = 1000, SingleProducerConstrained = true 
                    });

            Parallel.ForEach(
                indexEngine.AllDocument(0), 
                async document =>
                    {
                        try
                        {
                            // List<string> results = SearchEngine.DuplicateDetection(
                            // document, profileName, 5, 0, null, isearcher, indexSetting);

                            // if (results.Any())
                            // {
                            // var matchResult = new MatchResult
                            // {
                            // MatchRecords = new List<MatchRecord>(), 
                            // Count = results.Count, 
                            // ResultStatus = MatchResultStatus.Proposed, 
                            // RecordValue = JsonConvert.SerializeObject(document), 
                            // PrimaryRecordId = document["id"]
                            // };

                            // foreach (string result in results)
                            // {
                            // int idStart = result.IndexOf("\"id\":", StringComparison.OrdinalIgnoreCase);
                            // int idEnd = result.IndexOf("\"", idStart + 6, StringComparison.OrdinalIgnoreCase);

                            // matchResult.MatchRecords.Add(
                            // new MatchRecord
                            // {
                            // RecordValue = result, 
                            // ResultStatus = MatchRecordStatus.NotMatch, 
                            // MatchRecordId = result.Substring(idStart + 6, idEnd - (idStart + 6))
                            // });
                            // }

                            // await recordWriter.SendAsync(matchResult);
                            // }
                        }
                        catch
                        {
                        }
                    });

            // foreach (var document in indexEngine.AllDocument(0))
            // {
            // if (i == 100)
            // {
            // TextWriter textWriter = new StreamWriter(filePath + fileName + matchResults.FileSequence + ".xml");
            // matchResults.FileSequence++;
            // x.Serialize(textWriter, matchResults);
            // matchResults.MatchResult.Clear();
            // i = 0;
            // }

            // try
            // {
            // List<string> results = SearchEngine.DuplicateDetection(
            // document, profileName, 5, 0, isearcher, indexSetting);

            // if (results.Any())
            // {
            // var matchResult = new MatchResult();
            // matchResult.MatchRecord = new List<MatchRecord>();
            // matchResult.Count = results.Count;
            // matchResult.ResultStatus = MatchResultStatus.Proposed;
            // matchResult.RecordValue = JsonConvert.SerializeObject(document);
            // matchResult.PrimaryRecordId = document["id"];
            // foreach (string result in results)
            // {
            // int idStart = result.IndexOf("\"id\":");
            // int idEnd = result.IndexOf("\"", idStart + 6);

            // matchResult.MatchRecord.Add(
            // new MatchRecord
            // {
            // RecordValue = result, 
            // ResultStatus = MatchRecordStatus.NotMatch, 
            // MatchRecordId = result.Substring(idStart + 6, idEnd - (idStart + 6))
            // });
            // }

            // matchResults.MatchResult.Add(matchResult);
            // i++;
            // }
            // }
            // catch
            // {
            // }
            // }
            return null;
        }

        #endregion
    }
}