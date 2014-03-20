// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperation.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Interface
{
    using System.Collections.Generic;
    using System.Net.Http;

    /// <summary>
    /// The Operation interface.
    /// </summary>
    public interface IOperation
    {
        #region Public Methods and Operators

        /// <summary>
        /// The execute method which will be called by Coeus Search.
        /// The calss will not be initialized per request but be cached by
        /// Coeus Search Server for performance reason. So please ensure that the code does
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
        HttpResponseMessage Execute(string indexName, Dictionary<string, string> parameters, HttpVerbs verb, string callback);

        /// <summary>
        /// The initialize connector.
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        bool InitializeOperation();

        #endregion
    }

    /// <summary>
    /// The http verbs.
    /// </summary>
    public enum HttpVerbs
    {
        /// <summary>
        /// The get.
        /// </summary>
        Get, 

        /// <summary>
        /// The post.
        /// </summary>
        Post, 

        /// <summary>
        /// The delete.
        /// </summary>
        Delete
    }
}