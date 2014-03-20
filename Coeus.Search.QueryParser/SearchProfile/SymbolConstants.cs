// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolConstants.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   Defines the SymbolConstants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Coeus.Search.QueryParser.SearchProfile
{
    internal enum SymbolConstants : int
    {
        SYMBOL_EOF = 0, // (EOF)
        SYMBOL_ERROR = 1, // (Error)
        SYMBOL_WHITESPACE = 2, // Whitespace
        SYMBOL_LPARAN = 3, // '('
        SYMBOL_RPARAN = 4, // ')'
        SYMBOL_LT = 5, // '<'
        SYMBOL_GT = 6, // '>'
        SYMBOL_BOOST = 7, // BOOST
        SYMBOL_CONSTANT = 8, // CONSTANT
        SYMBOL_EXACT = 9, // EXACT
        SYMBOL_FUZZY = 10, // FUZZY
        SYMBOL_IDENTIFIER = 11, // Identifier
        SYMBOL_MUST = 12, // MUST
        SYMBOL_MUSTNOT = 13, // MUSTNOT
        SYMBOL_NUMBER = 14, // Number
        SYMBOL_SHOULD = 15, // SHOULD
        SYMBOL_WILDCARD = 16, // WILDCARD
        SYMBOL_BOOSTVALUE = 17, // <BoostValue>
        SYMBOL_CONDITION = 18, // <Condition>
        SYMBOL_CONDITONPROPERTIES = 19, // <ConditonProperties>
        SYMBOL_EXPRESSION = 20, // <Expression>
        SYMBOL_FIELDNAME = 21, // <FieldName>
        SYMBOL_FIELDVALUE = 22, // <FieldValue>
        SYMBOL_FUZZYVALUE = 23, // <FuzzyValue>
        SYMBOL_GROUP = 24, // <Group>
        SYMBOL_GROUPLOGIC = 25, // <GroupLogic>
        SYMBOL_GROUPS = 26, // <Groups>
        SYMBOL_LOGIC = 27  // <Logic>
    };
}