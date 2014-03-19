// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NRTManagerSyn.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The nrt manager syn.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Index
{
    using System;

    using Coeus.Search.Core.Utils;

    using org.apache.lucene.search;
    using org.apache.lucene.store;

    /// <summary>
    /// The nrt manager syn.
    /// </summary>
    internal class NrtManagerSyn : NRTManager.WaitingListener
    {
        #region Fields

        /// <summary>
        /// The manager.
        /// </summary>
        private readonly NRTManager manager;

        /// <summary>
        /// The target milli seconds.
        /// </summary>
        private readonly int targetMilliSeconds;

        /// <summary>
        /// The stop.
        /// </summary>
        private bool stop;

        /// <summary>
        /// The waiting gen.
        /// </summary>
        private long waitingGen;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NrtManagerSyn"/> class.
        /// </summary>
        /// <param name="manager">
        /// The manager.
        /// </param>
        /// <param name="targetMilliSeconds">
        /// The target Milli Seconds.
        /// </param>
        public NrtManagerSyn(NRTManager manager, int targetMilliSeconds)
        {
            this.targetMilliSeconds = targetMilliSeconds;
            this.manager = manager;

            // manager.addWaitingListener(this);
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

        /// <summary>
        /// The waiting.
        /// </summary>
        /// <param name="targetGen">
        /// The target Gen.
        /// </param>
        public void waiting(long targetGen)
        {
            this.waitingGen = Math.Max(this.waitingGen, targetGen);
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

            await this.targetMilliSeconds.MilliSeconds();

            try
            {
                this.manager.maybeRefresh();
            }
            catch (AlreadyClosedException)
            {
                this.stop = true;
            }

            this.PeriodicCommit();
        }

        #endregion
    }
}