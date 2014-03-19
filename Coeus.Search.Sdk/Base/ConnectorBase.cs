// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectorBase.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Base
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks.Dataflow;

    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Messages;

    /// <summary>
    /// The connector base.
    /// </summary>
    public abstract class ConnectorBase : IConnector
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorBase"/> class.
        /// </summary>
        protected ConnectorBase()
        {
            this.RequestQueue = new ActionBlock<IndexRequest>(request => this.ProcessRequests(request));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the connector's default request queue. This request queue will be used by
        /// Index Manager to send indexing request to the connector
        /// </summary>
        public ActionBlock<IndexRequest> RequestQueue { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets configuration store
        /// </summary>
        [Import]
        protected IConfigurationStore ConfigurationStore { get; private set; }

        /// <summary>
        /// Gets logger
        /// </summary>
        [Import]
        protected ILogger Logger { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Retrieve all the meta data associated with the connector.
        /// </summary>
        /// <returns>
        /// The System.Object.
        /// </returns>
        public virtual object AllMetaData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialize Connector. Automatically called by Coeus Search at the time of initialization.
        /// </summary>
        /// <returns>
        /// Operation Status
        /// </returns>
        public virtual bool InitializeConnector()
        {
            return true;
        }

        /// <summary>
        /// Retrieve the meta data associated with the connector
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <returns>
        /// The System.Object.
        /// </returns>
        public virtual object MetaData(string indexName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reconfigure connector. Called by Coeus Search to inform Connector that a configuration change is made at index level and
        /// the connector should reconfigure itself. 
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public virtual bool ReConfigureConnector()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The process requests.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public abstract void ProcessRequests(IndexRequest request);

        #endregion
    }
}