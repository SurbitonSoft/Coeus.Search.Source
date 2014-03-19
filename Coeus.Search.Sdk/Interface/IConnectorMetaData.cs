// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectorMetaData.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Interface
{
    using System.ComponentModel;

    /// <summary>
    /// The i connector meta data.
    /// </summary>
    public interface IConnectorMetaData
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether bulk indexing is supported by connector.
        /// </summary>
        [DefaultValue(false)]
        bool BulkIndexSupported { get; }

        /// <summary>
        /// Gets the connector name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the connector name.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets a value indicating whether single document indexing  is supported by connector.
        /// </summary>
        [DefaultValue(false)]
        bool IncrementalIndexingSupported { get; }

        #endregion
    }
}