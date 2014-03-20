// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexEngine.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   Class responsible for managing index and all related operations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Index
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    using Coeus.Search.Core.Configuration;
    using Coeus.Search.Core.Utils;
    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Messages;
    using Coeus.Search.Sdk.Settings;

    using org.apache.lucene.document;
    using org.apache.lucene.search;

    /// <summary>
    /// Class responsible for managing index and all related operations
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IIndexEngine))]
    internal class IndexEngine : IIndexEngine
    {
        #region Fields

        /// <summary>
        /// The cancellation token source.
        /// </summary>
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Gets or sets a value indicating whether bulk indexing in progress.
        /// </summary>
        private bool bulkIndexingInProgress;

        /// <summary>
        /// The configuration store.
        /// </summary>
        [Import]
        private IConfigurationStore configurationStore;

        /// <summary>
        /// The computed settings.
        /// </summary>
        private IndexSetting indexSettings;

        /// <summary>
        /// The logger.
        /// </summary>
        [Import]
        private ILogger logger;

        /// <summary>
        /// Gets or sets a value indicating whether commit in progress.
        /// </summary>
        private bool processingInProgress;

        /// <summary>
        /// The shards.
        /// </summary>
        private List<ShardWriter> shards;

        /// <summary>
        /// This block is responsible for taking the input from buffer and generating index document from it.
        /// </summary>
        private ActionBlock<IndexDocumentRequest> transformProcessor;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexEngine"/> class.
        /// </summary>
        /// <param name="indexName">
        /// The index Name.
        /// </param>
        public IndexEngine(string indexName)
        {
            this.IndexName = indexName;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the deleted documents.
        /// </summary>
        public int DeletedDocuments
        {
            get
            {
                return this.shards.Sum(t => t.DeletedDocuments);
            }
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public string IndexName { get; private set; }

        /// <summary>
        /// Gets the index searcher.
        /// </summary>
        public IndexSearcher[] IndexSearcher
        {
            get
            {
                var indexSearchers = new IndexSearcher[this.indexSettings.Shards];
                for (int index = 0; index < this.shards.Count; index++)
                {
                    indexSearchers[index] = this.shards[index].IndexSearcher;
                }

                return indexSearchers;
            }
        }

        /// <summary>
        /// Gets the ram documents.
        /// </summary>
        public int RamDocuments
        {
            get
            {
                return this.shards.Sum(t => t.RamDocuments);
            }
        }

        /// <summary>
        /// Gets the request queue.
        /// </summary>
        public BufferBlock<IndexDocumentRequest> RequestQueue { get; private set; }

        /// <summary>
        /// Gets the total documents.
        /// </summary>
        public int TotalDocuments
        {
            get
            {
                return this.shards.Sum(t => t.TotalDocuments);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The all document.
        /// </summary>
        /// <param name="totalRecordsToReturn">
        /// The total records to return.
        /// </param>
        /// <returns>
        /// Dictionary containing document terms
        /// </returns>
        public IEnumerable<Dictionary<string, string>> AllDocument(int totalRecordsToReturn)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The bulk indexing.
        /// </summary>
        /// <param name="prepareForBulkIndexing">
        /// The prepare for bulk indexing.
        /// </param>
        public void BulkIndexing(bool prepareForBulkIndexing)
        {
            this.processingInProgress = true;
            foreach (ShardWriter shardWriter in this.shards)
            {
                if (prepareForBulkIndexing)
                {
                    this.bulkIndexingInProgress = true;
                    shardWriter.BulkIndex(true);
                }
                else
                {
                    shardWriter.BulkIndex(false);
                    this.bulkIndexingInProgress = false;
                }
            }

            this.processingInProgress = false;
        }

        /// <summary>
        /// The bulk indexing in progress.
        /// </summary>
        /// <returns>
        /// The System.Threading.Tasks.Task`1[TResult -&gt; System.Boolean].
        /// </returns>
        public async Task<bool> BulkIndexingInProgress()
        {
            if (this.cancellationTokenSource.IsCancellationRequested)
            {
                return true;
            }

            while (this.bulkIndexingInProgress)
            {
                await 10.Seconds();
            }

            return true;
        }

        /// <summary>
        /// The close index.
        /// </summary>
        public void CloseIndex()
        {
            foreach (ShardWriter shardWriter in this.shards)
            {
                shardWriter.CloseIndex();
            }
        }

        /// <summary>
        /// The commit.
        /// </summary>
        public void Commit()
        {
            foreach (ShardWriter shardWriter in this.shards)
            {
                shardWriter.Commit();
            }
        }

        /// <summary>
        /// The initialize index.
        /// </summary>
        /// <param name="indexSetting">
        /// Current index settings
        /// </param>
        /// <returns>
        /// Initialization status
        /// </returns>
        public bool InitializeIndex(IndexSetting indexSetting)
        {
            this.indexSettings = indexSetting;
            this.InitializeQueues();
            if (indexSetting.Shards == 0)
            {
                this.logger.LogFatal(
                    string.Format("Minimum 1 shard has to be specified for the index: {0}", indexSetting.Shards));
                return false;
            }

            this.shards = new List<ShardWriter>(indexSetting.Shards);
            for (int i = 0; i < indexSetting.Shards; i++)
            {
                try
                {
                    this.shards.Add(new ShardWriter(this.IndexName, i, this.indexSettings, this.configurationStore));
                }
                catch (Exception e)
                {
                    this.logger.LogException(e);
                    this.logger.LogFatal(
                        string.Format("Error initializing Shard: {0} for index: {1}.", i, this.IndexName));
                    return false;
                }
            }

            this.PeriodicCommit();
            return true;
        }

        /// <summary>
        /// The commit in progress.
        /// </summary>
        /// <returns>
        /// The System.Threading.Tasks.Task`1[TResult -&gt; System.Boolean].
        /// </returns>
        public async Task<bool> ProcessingInProgress()
        {
            while (this.processingInProgress)
            {
                await 1.Seconds();
            }

            return true;
        }

        /// <summary>
        /// The re configure index.
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public bool ReConfigureIndex()
        {
            return true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The initialize queues.
        /// </summary>
        private void InitializeQueues()
        {
            this.RequestQueue =
                new BufferBlock<IndexDocumentRequest>(
                    new ExecutionDataflowBlockOptions
                        {
                            MaxDegreeOfParallelism = 1, 
                            BoundedCapacity = 1000, 
                            SingleProducerConstrained = false, 
                            CancellationToken = this.cancellationTokenSource.Token
                        });

            this.transformProcessor = new ActionBlock<IndexDocumentRequest>(
                request => this.TransformRequests(request), 
                new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded, 
                        BoundedCapacity = 50, 
                        SingleProducerConstrained = true, 
                        CancellationToken = this.cancellationTokenSource.Token
                    });

            this.RequestQueue.LinkTo(this.transformProcessor);
        }

        /// <summary>
        /// The periodic commit.
        /// </summary>
        private async void PeriodicCommit()
        {
            while (!this.cancellationTokenSource.IsCancellationRequested)
            {
                await 60.Seconds();
                this.Commit();
            }
        }

        /// <summary>
        /// The transform requests.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        private async void TransformRequests(IndexDocumentRequest request)
        {
            await this.ProcessingInProgress();
            switch (request.DocumentRequestType)
            {
                case DocumentRequestType.Create:
                case DocumentRequestType.Update:
                    break;
                case DocumentRequestType.Delete:
                    await
                        this.shards[request.DocumentId.GetAsciiHash() % this.indexSettings.Shards].IndexingProcessor.SendAsync(
                            new Tuple<DocumentRequestType, string, Document>(
                                DocumentRequestType.Delete, request.DocumentId, null));
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var document = new Document();
            List<IndexField>.Enumerator listEnumeratornumerator = this.indexSettings.AllFields.GetEnumerator();
            document.add(new StringField("id", request.DocumentId, Field.Store.YES));
            document.add(new StringField("type", this.IndexName, Field.Store.YES));
            request.Document["id"] = request.DocumentId;
            request.Document["type"] = this.IndexName;

            while (listEnumeratornumerator.MoveNext())
            {
                if (listEnumeratornumerator.Current == null)
                {
                    continue;
                }

                if (ReferenceEquals(listEnumeratornumerator.Current.Name, "id")
                    || ReferenceEquals(listEnumeratornumerator.Current.Name, "type")
                    || listEnumeratornumerator.Current.IsPrimary)
                {
                    // Remove the field from document with the name of primary key as
                    // primary key will always be called id
                    // request.Document.Remove(listEnumeratornumerator.Current.Name);
                    continue;
                }

                string fieldValue;
                string fieldName = listEnumeratornumerator.Current.IsAdditionalAnalysisField
                                       ? listEnumeratornumerator.Current.BaseFieldName
                                       : listEnumeratornumerator.Current.Name;

                if (request.Document.TryGetValue(fieldName, out fieldValue))
                {
                    if (string.IsNullOrWhiteSpace(fieldValue))
                    {
                        fieldValue = this.indexSettings.NullValue;
                    }
                }
                else
                {
                    // This field is not passed along with the document so 
                    // index it as null
                    fieldValue = this.indexSettings.NullValue;
                    request.Document.Add(fieldName, fieldValue);
                }

                if (listEnumeratornumerator.Current.DoNotIndexField)
                {
                    document.add(new StoredField(fieldName, fieldValue));
                    continue;
                }

                if (listEnumeratornumerator.Current.IsAdditionalAnalysisField)
                {
                    document.add(new TextField(listEnumeratornumerator.Current.Name, fieldValue, Field.Store.NO));
                    continue;
                }

                document.add(
                    listEnumeratornumerator.Current.Store
                        ? new TextField(listEnumeratornumerator.Current.Name, fieldValue, Field.Store.YES)
                        : new TextField(listEnumeratornumerator.Current.Name, fieldValue, Field.Store.NO));
            }

            int shardNumber = request.DocumentId.GetAsciiHash() % this.indexSettings.Shards;
            await
                this.shards[shardNumber].IndexingProcessor.SendAsync(
                    new Tuple<DocumentRequestType, string, Document>(
                        request.DocumentRequestType, request.DocumentId, document));
        }

        #endregion
    }
}