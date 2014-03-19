// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexCommitter.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The index committer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Index
{
    using Coeus.Search.Core.Utils;

    using org.apache.lucene.index;
    using org.apache.lucene.store;

    /// <summary>
    /// The index committer.
    /// </summary>
    internal class IndexCommitter
    {
        #region Fields

        /// <summary>
        /// The index writer.
        /// </summary>
        private readonly IndexWriter indexWriter;

        /// <summary>
        /// The target seconds.
        /// </summary>
        private readonly int targetSeconds;

        /// <summary>
        /// The index reader.
        /// </summary>
        private DirectoryReader indexReader;

        /// <summary>
        /// The stop.
        /// </summary>
        private bool stop;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexCommitter"/> class.
        /// </summary>
        /// <param name="indexWriter">
        /// The index writer.
        /// </param>
        /// <param name="targetSeconds">
        /// The target seconds.
        /// </param>
        public IndexCommitter(IndexWriter indexWriter, int targetSeconds)
        {
            this.targetSeconds = targetSeconds;
            this.indexWriter = indexWriter;
            this.indexReader = DirectoryReader.open(this.indexWriter, true);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            this.stop = false;
            this.PeriodicCommit();
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.stop = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The periodic commit.
        /// </summary>
        private async void PeriodicCommit()
        {
            if (this.stop)
            {
                return;
            }

            try
            {
                if (!this.indexReader.isCurrent())
                {
                    this.indexWriter.commit();
                    this.indexReader = DirectoryReader.openIfChanged(this.indexReader, this.indexWriter, true);
                }
            }
            catch (AlreadyClosedException)
            {
            }

            await this.targetSeconds.Seconds();
            this.PeriodicCommit();
        }

        #endregion
    }
}