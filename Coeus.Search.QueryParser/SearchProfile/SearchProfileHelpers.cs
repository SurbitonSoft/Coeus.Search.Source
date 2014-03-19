// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchProfileHelpers.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.QueryParser.SearchProfile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The search profile helpers.
    /// </summary>
    public class SearchProfileHelpers
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create search string.
        /// </summary>
        /// <param name="tokens">
        /// The tokens.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="missingDataStrategy">
        /// The missing data strategy.
        /// </param>
        /// <param name="nullValue">
        /// The null value.
        /// </param>
        /// <returns>
        /// The System.String.
        /// </returns>
        public static string CreateSearchString(
            List<Token> tokens, Dictionary<string, string> values, int missingDataStrategy, string nullValue)
        {
            if (!values.Any())
            {
                throw new ArgumentException("No values passed to the parser.");
            }

            var field = new StringBuilder();
            var command = new List<string>();
            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case Token.TokenType.GroupStart:
                        switch (token.Condition)
                        {
                            case Token.MatchCondition.Must:
                                command.Add("+(");
                                break;
                            case Token.MatchCondition.MustNot:
                                command.Add("-(");
                                break;
                            case Token.MatchCondition.Should:
                                command.Add("(");
                                break;
                        }

                        break;

                    case Token.TokenType.GroupEnd:
                        if (token.HasBoost)
                        {
                            command.Add(")" + "^" + token.Boost);
                        }
                        else
                        {
                            command.Add(")");
                        }

                        break;
                    case Token.TokenType.Field:
                        GenerateFieldString(values, missingDataStrategy, nullValue, field, token, command);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            // Remove empty groups
            var result = command.Aggregate((i, j) => i + " " + j);
            result = result.Replace("+( )", string.Empty);
            result = result.Replace("-( )", string.Empty);
            return result.Replace("( )", string.Empty);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The generate field string.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="missingDataStrategy">
        /// The missing data strategy.
        /// </param>
        /// <param name="nullValue">
        /// The null value.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        private static void GenerateFieldString(
            Dictionary<string, string> values,
            int missingDataStrategy,
            string nullValue,
            StringBuilder field,
            Token token,
            List<string> command)
        {
            field.Clear();
            switch (token.Condition)
            {
                case Token.MatchCondition.Must:
                    field.Append("+").Append(token.FieldName).Append(":");
                    break;
                case Token.MatchCondition.MustNot:
                    field.Append("-").Append(token.FieldName).Append(":");
                    break;
                case Token.MatchCondition.Should:
                    field.Append(token.FieldName).Append(":");
                    break;
            }

            if (token.IsConstant)
            {
                field.Append("(").Append(token.FieldValue).Append(")");
                command.Add(field.ToString());
                return;
            }

            string fieldValue;
            if (values.TryGetValue(token.FieldValue, out fieldValue))
            {
                if (string.IsNullOrWhiteSpace(fieldValue))
                {
                    MissingDataStrategy(missingDataStrategy, nullValue, token, field);
                    return;
                }

                fieldValue = fieldValue.Replace("+", "\\+");
                fieldValue = fieldValue.Replace("-", "\\-");
                fieldValue = fieldValue.Replace("(", "\\(");
                fieldValue = fieldValue.Replace(")", "\\)");

                if (token.IsExact)
                {
                    field.Append("(").Append("\"").Append(fieldValue).Append("\"");
                }
                else
                {
                    field.Append("(").Append(fieldValue);
                }

                // Exact ,wild card and fuzzy don't work together
                if (token.HasWildCard && !token.IsExact && !token.HasFuzzy)
                {
                    field.Append("*");
                }

                if (token.HasFuzzy)
                {
                    field.Append("~").Append(token.FuzzyMatch);
                }

                if (token.HasBoost)
                {
                    field.Append("^").Append(token.Boost);
                }

                field.Append(")");
                command.Add(field.ToString());
            }
            else
            {
                if (missingDataStrategy == 1)
                {
                    return;
                }

                MissingDataStrategy(missingDataStrategy, nullValue, token, field);
                command.Add(field.ToString());
            }
        }

        /// <summary>
        /// The missing data strategy method
        /// </summary>
        /// <param name="missingDataStrategy">
        /// The missing data strategy.
        /// </param>
        /// <param name="nullValue">
        /// The null value.
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="sb">
        /// The string builder
        /// </param>
        private static void MissingDataStrategy(
            int missingDataStrategy, string nullValue, Token token, StringBuilder sb)
        {
            switch (missingDataStrategy)
            {
                case 0:

                    // Error
                    throw new ArgumentException(
                        "A parameter required for the duplicate detection condition is missing. Parameter name: "
                        + token.FieldName);

                case 1:

                    // Ignore
                    break;
                case 2:

                    // Treat as null
                    sb.Append(nullValue);
                    break;
            }
        }

        #endregion
    }
}