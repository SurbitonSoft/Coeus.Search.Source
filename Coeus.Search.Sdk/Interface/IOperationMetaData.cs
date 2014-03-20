// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationMetaData.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Coeus.Search.Sdk.Interface
{
    using System.ComponentModel;

    /// <summary>
    /// The OperationMetaData interface.
    /// </summary>
    public interface IOperationMetaData
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether delete verb is supported.
        /// </summary>
        [DefaultValue(false)]
        bool DeleteSupported { get; }

        /// <summary>
        /// Gets a value indicating whether get verb is supported.
        /// </summary>
        [DefaultValue(false)]
        bool GetSupported { get; }

        /// <summary>
        /// Gets the operation name.
        /// </summary>
        string OperationName { get; }

        /// <summary>
        /// Gets a value indicating whether post verb is supported.
        /// </summary>
        [DefaultValue(false)]
        bool PostSupported { get; }

        #endregion
    }
}