// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShardWriter.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The shard writer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Index
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks.Dataflow;

    using Coeus.Search.Core.Configuration;
    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Messages;
    using Coeus.Search.Sdk.Settings;

    using java.io;

    using org.apache.lucene.document;
    using org.apache.lucene.index;
    using org.apache.lucene.search;
    using org.apache.lucene.store;

    using Version = org.apache.lucene.util.Version;

    /// <summary>
    /// The shard writer.
    /// </summary>
    internal class ShardWriter
    {
        #region Fields

        /// <summary>
        /// The index committer.
        /// </summary>
        private readonly IndexCommitter indexCommitter;

        /// <summary>
        /// The computed settings.
        /// </summary>
        private readonly IIndexSetting indexSettings;

        /// <summary>
        /// The index writer.
        /// </summary>
        private readonly IndexWriter indexWriter;

        /// <summary>
        /// The memory lock.
        /// </summary>
        private readonly object memoryLock = new object();

        /// <summary>
        /// The Near real time manager.
        /// </summary>
        private readonly NRTManager nrtManager;

        /// <summary>
        /// The Near real time manager reopen thread.
        /// </summary>
        private readonly NrtManagerSyn nrtManagerSyn;

        /// <summary>
        /// The tracking index writer.
        /// </summary>
        private readonly NRTManager.TrackingIndexWriter trackingIndexWriter;

        /// <summary>
        /// The configuration store.
        /// </summary>
        [Import]
        private IConfigurationStore configurationStore;

        /// <summary>
        /// The directory.
        /// </summary>
        private Directory directory;

        /// <summary>
        /// The index reader.
        /// </summary>
        private DirectoryReader indexReader;

        /// <summary>
        /// The indexing buffer size.
        /// </summary>
        private int indexingBufferSize;

        /// <summary>
        /// The logger.
        /// </summary>
        [Import]
        private ILogger logger;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShardWriter"/> class. 
        /// </summary>
        /// <param name="indexName">
        /// The index Name.
        /// </param>
        /// <param name="shardNumber">
        /// The shard Number.
        /// </param>
        /// <param name="indexSetting">
        /// The index Setting.
        /// </param>
        /// <param name="store">
        /// Configuration store
        /// </param>
        public ShardWriter(string indexName, int shardNumber, IndexSetting indexSetting, IConfigurationStore store)
        {
            this.IndexName = indexName;
            this.configurationStore = store;
            this.indexSettings = indexSetting;
            this.InitializeQueues();
            var file =
                new File(string.Format(@"{0}\{1}\{2}", this.configurationStore.DataPath, this.IndexName, shardNumber));
            if (!this.GetDirectoryLock(file))
            {
                throw new Exception(
                    string.Format(
                        "Unable to lock the index shard: {0} {1} at location: {2}.",
                        this.IndexName,
                        shardNumber,
                        string.Format(@"{0}\{1}", this.configurationStore.DataPath, this.IndexName)));
            }

            this.indexingBufferSize = this.indexSettings.RamBufferSizeMax / this.indexSettings.Shards;

            // Configuration information
            var config = new IndexWriterConfig(Version.LUCENE_40, this.indexSettings.IndexAnalyzer);
            config.setRAMBufferSizeMB(this.indexSettings.RamBufferSizeMin);
            config.setOpenMode(IndexWriterConfig.OpenMode.CREATE_OR_APPEND);

            // Initialize readers and writers
            this.indexWriter = new IndexWriter(this.directory, config);

            this.indexCommitter = new IndexCommitter(this.indexWriter, this.indexSettings.CommitTime);
            this.indexCommitter.Start();

            this.indexReader = DirectoryReader.open(this.indexWriter, true);
            this.trackingIndexWriter = new NRTManager.TrackingIndexWriter(this.indexWriter);
            this.nrtManager = new NRTManager(this.trackingIndexWriter, new SearcherFactory(), true);
            this.nrtManagerSyn = new NrtManagerSyn(this.nrtManager, this.indexSettings.RefreshTime);
            this.nrtManagerSyn.Start();
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
                return this.indexReader.numDeletedDocs();
            }
        }

        /// <summary>
        /// Gets or sets the index buffer size.
        /// </summary>
        public int IndexBufferSize
        {
            get
            {
                return this.indexingBufferSize;
            }

            set
            {
                lock (this.memoryLock)
                {
                    try
                    {
                        this.indexingBufferSize = value;
                        if (this.indexWriter != null)
                        {
                            this.indexWriter.getConfig().setRAMBufferSizeMB(this.indexingBufferSize);
                        }
                    }
                    catch (Exception exception)
                    {
                        this.logger.LogException(string.Format("Unable to lock index {0}", this.IndexName), exception);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public string IndexName { get; private set; }

        /// <summary>
        /// Gets the index reader.
        /// </summary>
        public DirectoryReader IndexReader
        {
            get
            {
                if (!this.indexReader.isCurrent())
                {
                    this.indexReader = DirectoryReader.openIfChanged(this.indexReader, this.indexWriter, false);
                }

                return this.indexReader;
            }
        }

        /// <summary>
        /// Gets the index searcher.
        /// </summary>
        public IndexSearcher IndexSearcher
        {
            get
            {
                return (IndexSearcher)this.nrtManager.acquire();
            }
        }

        /// <summary>
        /// Gets the indexing processor.
        /// </summary>
        public ActionBlock<Tuple<DocumentRequestType, string, Document>> IndexingProcessor { get; private set; }

        /// <summary>
        /// Gets the ram documents.
        /// </summary>
        public int RamDocuments
        {
            get
            {
                return this.indexWriter.numRamDocs();
            }
        }

        /// <summary>
        /// Gets the total documents.
        /// </summary>
        public int TotalDocuments
        {
            get
            {
                return this.indexWriter.numDocs();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The bulk index.
        /// </summary>
        /// <param name="startBulkIndexing">
        /// The start bulk indexing.
        /// </param>
        public void BulkIndex(bool startBulkIndexing)
        {
            if (startBulkIndexing)
            {
                // Do not process any other bulkindexing request if one is already in
                // progress
                // this.nrtManagerReopen.close();
                this.nrtManagerSyn.Stop();
                this.indexCommitter.Stop();
                this.trackingIndexWriter.deleteAll();
                this.Commit();
            }
            else
            {
                this.Commit();
                this.indexCommitter.Start();
                this.nrtManagerSyn.Start();
            }
        }

        /// <summary>
        /// The close index.
        /// </summary>
        public void CloseIndex()
        {
            try
            {
                // this.nrtManagerReopen.close();
                this.nrtManagerSyn.Stop();
                this.nrtManager.close();
                this.indexWriter.commit();
                this.indexWriter.close();
                this.indexReader.close();
            }
            catch (AlreadyClosedException)
            {
            }
            catch (Exception e)
            {
                this.logger.LogException(e);
            }
            finally
            {
                // If an Exception is hit during close, eg due to disk full or some other reason, then both 
                // the on-disk index and the internal state of the IndexWriter instance will be consistent. 
                // However, the close will not be complete even though part of it (flushing buffered documents) 
                // may have succeeded, so the write lock will still be held.
                if (IndexWriter.isLocked(this.directory))
                {
                    IndexWriter.unlock(this.directory);
                }
            }
        }

        /// <summary>
        /// The commit.
        /// </summary>
        public void Commit()
        {
            try
            {
                this.indexWriter.commit();
            }
            catch (AlreadyClosedException)
            {
            }
        }

        /// <summary>
        /// The commit.
        /// </summary>
        public void DeleteAll()
        {
            this.trackingIndexWriter.deleteAll();
            this.Commit();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get directory lock.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        private bool GetDirectoryLock(File file)
        {
            LockFactory lockFactory = new NativeFSLockFactory();
            try
            {
                this.directory = FSDirectory.open(file, lockFactory);
            }
            catch (Exception exception)
            {
                this.logger.LogException(
                    string.Format("Unable to open index at location: {0}", file.getPath()), exception);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The initialize queues.
        /// </summary>
        private void InitializeQueues()
        {
            this.IndexingProcessor =
                new ActionBlock<Tuple<DocumentRequestType, string, Document>>(
                    request => this.ProcessRequests(request),
                    new ExecutionDataflowBlockOptions
                        {
                            MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
                            BoundedCapacity = 50,
                            SingleProducerConstrained = true
                        });
        }

        /// <summary>
        /// Processes all requests
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        private void ProcessRequests(Tuple<DocumentRequestType, string, Document> request)
        {
            switch (request.Item1)
            {
                case DocumentRequestType.Create:
                    this.trackingIndexWriter.addDocument(request.Item3);
                    break;
                case DocumentRequestType.Update:
                    this.trackingIndexWriter.updateDocument(new Term("id", request.Item2), request.Item3);
                    break;
                case DocumentRequestType.Delete:
                    this.trackingIndexWriter.deleteDocuments(new Term("id", request.Item2));
                    break;
            }
        }

        #endregion
    }
}