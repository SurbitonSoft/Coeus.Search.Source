// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITokenFilterFactoryMetaData.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Interface
{
    /// <summary>
    /// The TokenFilterFactoryMetaData interface.
    /// </summary>
    public interface ITokenFilterFactoryMetaData
    {
        #region Public Properties

        /// <summary>
        /// Gets the filter name.
        /// </summary>
        string FilterName { get; }

        #endregion
    }
}