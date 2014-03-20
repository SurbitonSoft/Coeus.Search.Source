// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationBase.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Base
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net.Http;

    using Coeus.Search.Sdk.Interface;

    /// <summary>
    /// The operation base.
    /// </summary>
    public abstract class OperationBase : IOperation
    {

        /// <summary>
        /// Gets configuration store
        /// </summary>
        [Import]
        protected IConfigurationStore ConfigurationStore { get; private set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        [Import]
        protected ILogger Logger { get; private set; }

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
        public abstract HttpResponseMessage Execute(string indexName, Dictionary<string, string> parameters, HttpVerbs verb, string callback);

        /// <summary>
        /// The initialize connector.
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public virtual bool InitializeOperation()
        {
            return true;
        }

    }
}