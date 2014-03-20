// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenizerType.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Settings
{
    /// <summary>
    /// The tokenizer type.
    /// </summary>
    public enum TokenizerType
    {
        /// <summary>
        /// The keyword tokenizer.
        /// </summary>
        KeywordTokenizer, 

        /// <summary>
        /// The letter tokenizer.
        /// </summary>
        LetterTokenizer, 

        /// <summary>
        /// The whitespace tokenizer.
        /// </summary>
        WhitespaceTokenizer, 

        /// <summary>
        /// The lower case tokenizer.
        /// </summary>
        LowerCaseTokenizer, 

        /// <summary>
        /// The standard tokenizer.
        /// </summary>
        StandardTokenizer, 

        /// <summary>
        /// The ua x 29 url email tokenizer.
        /// </summary>
        Uax29UrlEmailTokenizer, 

        /// <summary>
        /// The pattern tokenizer.
        /// </summary>
        PatternTokenizer, 

        /// <summary>
        /// The path hierarchy tokenizer.
        /// </summary>
        PathHierarchyTokenizer
    }
}