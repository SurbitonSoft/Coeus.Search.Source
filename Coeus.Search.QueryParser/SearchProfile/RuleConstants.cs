// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleConstants.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   Defines the RuleConstants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Coeus.Search.QueryParser.SearchProfile
{
    enum RuleConstants : int
    {
        RULE_GROUPS = 0, // <Groups> ::= <Group>
        RULE_GROUPS2 = 1, // <Groups> ::= <Group> <Groups>
        RULE_GROUP_LPARAN_RPARAN = 2, // <Group> ::= <GroupLogic> '(' <Expression> ')'
        RULE_GROUP_LPARAN_RPARAN_BOOST = 3, // <Group> ::= <GroupLogic> '(' <Expression> ')' BOOST <BoostValue>
        RULE_EXPRESSION = 4, // <Expression> ::= <Condition>
        RULE_EXPRESSION2 = 5, // <Expression> ::= <Condition> <Expression>
        RULE_LOGIC_MUST = 6, // <Logic> ::= MUST
        RULE_LOGIC_MUSTNOT = 7, // <Logic> ::= MUSTNOT
        RULE_LOGIC_SHOULD = 8, // <Logic> ::= SHOULD
        RULE_LOGIC = 9, // <Logic> ::= 
        RULE_GROUPLOGIC_MUST = 10, // <GroupLogic> ::= MUST
        RULE_GROUPLOGIC_MUSTNOT = 11, // <GroupLogic> ::= MUSTNOT
        RULE_GROUPLOGIC_SHOULD = 12, // <GroupLogic> ::= SHOULD
        RULE_GROUPLOGIC = 13, // <GroupLogic> ::= 
        RULE_CONDITION_LT_GT = 14, // <Condition> ::= <Logic> <FieldName> '<' <FieldValue> '>' <ConditonProperties>
        RULE_CONDITONPROPERTIES_BOOST = 15, // <ConditonProperties> ::= BOOST <BoostValue> <ConditonProperties>
        RULE_CONDITONPROPERTIES_FUZZY = 16, // <ConditonProperties> ::= FUZZY <FuzzyValue> <ConditonProperties>
        RULE_CONDITONPROPERTIES_CONSTANT = 17, // <ConditonProperties> ::= CONSTANT <ConditonProperties>
        RULE_CONDITONPROPERTIES_EXACT = 18, // <ConditonProperties> ::= EXACT <ConditonProperties>
        RULE_CONDITONPROPERTIES_WILDCARD = 19, // <ConditonProperties> ::= WILDCARD <ConditonProperties>
        RULE_CONDITONPROPERTIES = 20, // <ConditonProperties> ::= 
        RULE_FIELDNAME_IDENTIFIER = 21, // <FieldName> ::= Identifier
        RULE_FIELDVALUE_NUMBER = 22, // <FieldValue> ::= Number
        RULE_FIELDVALUE_IDENTIFIER = 23, // <FieldValue> ::= Identifier
        RULE_BOOSTVALUE_NUMBER = 24, // <BoostValue> ::= Number
        RULE_FUZZYVALUE_NUMBER = 25  // <FuzzyValue> ::= Number
    };
}