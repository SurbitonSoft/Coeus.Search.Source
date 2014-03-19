// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnector.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Interface
{
    using System.Threading.Tasks.Dataflow;

    using Coeus.Search.Sdk.Messages;

    /// <summary>
    /// The DataSource interface.
    /// </summary>
    public interface IConnector
    {
        #region Public Properties

        /// <summary>
        /// Gets the connector's default request queue. This request queue will be used by
        /// Index Manager to send indexing request to the connector
        /// </summary>
        ActionBlock<IndexRequest> RequestQueue { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Retrieve all the meta data associated with the connector.
        /// </summary>
        /// <returns>
        /// The System.Object.
        /// </returns>
        object AllMetaData();

        /// <summary>
        /// Initialize Connector. Automatically called by Searcher at the time of initialization.
        /// </summary>
        /// <returns>
        /// Operation Status
        /// </returns>
        bool InitializeConnector();

        /// <summary>
        /// Retrieve the meta data associated with the connector
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <returns>
        /// The System.Object.
        /// </returns>
        object MetaData(string indexName);

        /// <summary>
        /// Reconfigure connector. Called by Coeus Search to inform Connector that a configuration change is made at index level and
        /// the connector should reconfigure itself. 
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        bool ReConfigureConnector();

        #endregion
    }
}