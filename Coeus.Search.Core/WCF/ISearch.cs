// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISearch.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The Search interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.WCF
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.ServiceModel;

    /// <summary>
    /// The Search interface.
    /// </summary>
    [ServiceContract]
    [Obfuscation(Exclude = true)]
    public interface ISearch
    {
        #region Public Methods and Operators

        /// <summary>
        /// The search.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The System.Collections.Generic.List`1[T -&gt; System.Collections.Generic.List`1[T -&gt; System.Collections.Generic.KeyValuePair`2[TKey -&gt; System.String, TValue -&gt; System.String]]].
        /// </returns>
        [OperationContract]
        List<List<KeyValuePair<string, string>>> Search(
            string indexName, string operationName, Dictionary<string, string> input);

        #endregion

        // SearchRequest message);
    }
}