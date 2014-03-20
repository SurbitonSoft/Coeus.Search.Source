// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchProfileParser.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.QueryParser.SearchProfile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using com.calitha.goldparser;

    /// <summary>
    /// The field.
    /// </summary>
    public class Token
    {
        #region Enums

        /// <summary>
        /// The search field condition.
        /// </summary>
        public enum MatchCondition
        {
            /// <summary>
            /// The and.
            /// </summary>
            Must,

            /// <summary>
            /// The or.
            /// </summary>
            MustNot,

            /// <summary>
            /// The not.
            /// </summary>
            Should
        }

        /// <summary>
        /// The token type.
        /// </summary>
        public enum TokenType
        {
            /// <summary>
            /// The group start.
            /// </summary>
            GroupStart,

            /// <summary>
            /// The group end.
            /// </summary>
            GroupEnd,

            /// <summary>
            /// The field.
            /// </summary>
            Field
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the boost.
        /// </summary>
        public int Boost { get; set; }

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        public MatchCondition Condition { get; set; }

        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the field value.
        /// </summary>
        public string FieldValue { get; set; }

        /// <summary>
        /// Gets or sets the fuzzy match.
        /// </summary>
        public int FuzzyMatch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has boost.
        /// </summary>
        public bool HasBoost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has fuzzy.
        /// </summary>
        public bool HasFuzzy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether has wild card.
        /// </summary>
        public bool HasWildCard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is constant.
        /// </summary>
        public bool IsConstant { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is exact.
        /// </summary>
        public bool IsExact { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public TokenType Type { get; set; }

        #endregion
    }

    /// <summary>
    /// The search profile parser.
    /// </summary>
    public class SearchProfileParser
    {
        #region Fields

        /// <summary>
        /// The parser.
        /// </summary>
        private LALRParser parser;

        /// <summary>
        /// The terminal tokens.
        /// </summary>
        private List<TerminalToken> terminalTokens = new List<TerminalToken>();

        /// <summary>
        /// The tokens.
        /// </summary>
        private List<Token> tokens = new List<Token>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchProfileParser"/> class.
        /// </summary>
        public SearchProfileParser()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = null;
            try
            {
                stream = assembly.GetManifestResourceStream("Coeus.Search.QueryParser.SearchProfile.CoeusSearcher.cgt");
                this.Init(stream);

            }
            catch (Exception ex)
            {
                const string err = "Error! Cannot loads GoldParser manifest resource.";
                Console.WriteLine(err);
                throw new Exception(err, ex);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="tokens">
        /// The tokens.
        /// </param>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        public bool Parse(string source, out List<Token> tokens)
        {
            source = source.Replace("\t", string.Empty);
            source = source.Replace("\n", string.Empty);
            NonterminalToken token = this.parser.Parse(source);
            if (token != null)
            {
                this.terminalTokens = new List<TerminalToken>();
                this.tokens = new List<Token>();
                this.GenerateTerminalTokens(token);
                this.GenerateToken();
                tokens = this.tokens;
                return true;
            }

            tokens = default(List<Token>);
            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create object.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="index">
        /// </param>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The System.Object.
        /// </returns>
        /// <summary>
        /// The create object from terminal.
        /// </summary>
        /// <returns>
        /// The System.Object.
        /// </returns>
        private object CreateObjectFromTerminal(TerminalToken token, int index)
        {
            switch (token.Symbol.Id)
            {
                case (int)SymbolConstants.SYMBOL_LPARAN:

                    // '('
                    this.tokens.Add(
                        new Token { Type = Token.TokenType.GroupStart, Condition = Token.MatchCondition.Should });
                    return null;

                case (int)SymbolConstants.SYMBOL_RPARAN:

                    // ')'
                    this.tokens.Add(
                        new Token { Type = Token.TokenType.GroupEnd, Condition = Token.MatchCondition.Should });
                    return null;

                case (int)SymbolConstants.SYMBOL_BOOST:

                    // BOOST
                    this.tokens.Last().HasBoost = true;
                    this.tokens.Last().Boost = Convert.ToInt32(this.terminalTokens[index + 1].Text);
                    return index + 1;

                case (int)SymbolConstants.SYMBOL_CONSTANT:

                    // CONSTANT
                    this.tokens.Last().IsConstant = true;
                    return null;

                case (int)SymbolConstants.SYMBOL_EXACT:

                    // EXACT
                    // Exact and wild card don't work together
                    if (this.tokens.Last().HasWildCard)
                    {
                        throw new Exception("'EXACT' and 'WILDCARD' cannot be specified for the same field.");
                    }

                    this.tokens.Last().IsExact = true;
                    return null;

                case (int)SymbolConstants.SYMBOL_FUZZY:

                    // FUZZY
                    // Fuzzy and wild card don't work together
                    if (this.tokens.Last().HasWildCard)
                    {
                        throw new Exception("'FUZZY' and 'WILDCARD' cannot be specified for the same field.");
                    }

                    this.tokens.Last().HasFuzzy = true;
                    this.tokens.Last().FuzzyMatch = Convert.ToInt32(this.terminalTokens[index + 1].Text);
                    return index + 1;

                case (int)SymbolConstants.SYMBOL_WILDCARD:

                    // WILDCARD
                    // Exact and wild card don't work together
                    if (this.tokens.Last().IsExact || this.tokens.Last().HasFuzzy)
                    {
                        throw new Exception("'EXACT', 'FUZZY' and 'WILDCARD' cannot be specified for the same field.");
                    }

                    this.tokens.Last().HasWildCard = true;
                    return null;

                case (int)SymbolConstants.SYMBOL_IDENTIFIER:

                    // Its a field name identifier
                    this.tokens.Add(
                        new Token
                            {
                                FieldName = token.Text,
                                FieldValue = this.terminalTokens[index + 2].Text,
                                Type = Token.TokenType.Field,
                                Condition = Token.MatchCondition.Should
                            });
                    return index + 3;

                case (int)SymbolConstants.SYMBOL_MUST:

                    // MUST
                    // This is the must symbol before a group
                    if (this.terminalTokens[index + 1].Text == "(")
                    {
                        this.tokens.Add(
                            new Token { Type = Token.TokenType.GroupStart, Condition = Token.MatchCondition.Must });

                        return index + 1;
                    }

                    this.tokens.Add(
                        new Token
                            {
                                FieldName = this.terminalTokens[index + 1].Text,
                                FieldValue = this.terminalTokens[index + 3].Text,
                                Type = Token.TokenType.Field,
                                Condition = Token.MatchCondition.Must
                            });

                    return index + 4;

                case (int)SymbolConstants.SYMBOL_MUSTNOT:

                    // MUSTNOT
                    // This is the mustnot symbol before a group
                    if (this.terminalTokens[index + 1].Text == "(")
                    {
                        this.tokens.Add(
                            new Token { Type = Token.TokenType.GroupStart, Condition = Token.MatchCondition.MustNot });

                        return index + 1;
                    }

                    this.tokens.Add(
                        new Token
                            {
                                FieldName = this.terminalTokens[index + 1].Text,
                                FieldValue = this.terminalTokens[index + 3].Text,
                                Type = Token.TokenType.Field,
                                Condition = Token.MatchCondition.MustNot
                            });

                    return index + 4;

                case (int)SymbolConstants.SYMBOL_SHOULD:

                    // SHOULD
                    // This is the must symbol before a group
                    if (this.terminalTokens[index + 1].Text == "(")
                    {
                        this.tokens.Add(
                            new Token { Type = Token.TokenType.GroupStart, Condition = Token.MatchCondition.Should });

                        return index + 1;
                    }

                    this.tokens.Add(
                        new Token
                            {
                                FieldName = this.terminalTokens[index + 1].Text,
                                FieldValue = this.terminalTokens[index + 3].Text,
                                Type = Token.TokenType.Field,
                                Condition = Token.MatchCondition.Should
                            });

                    return index + 4;
            }

            return null;
        }

        /// <summary>
        /// The generate terminal tokens.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        private void GenerateTerminalTokens(NonterminalToken token)
        {
            foreach (com.calitha.goldparser.Token terminalToken in token.Tokens)
            {
                if (terminalToken is TerminalToken)
                {
                    var tkn = terminalToken as TerminalToken;
                    this.terminalTokens.Add(terminalToken as TerminalToken);
                }

                if (terminalToken is NonterminalToken)
                {
                    var ttoken = terminalToken as NonterminalToken;
                    if (ttoken.Tokens.Any())
                    {
                        this.GenerateTerminalTokens(terminalToken as NonterminalToken);
                    }
                }
            }
        }

        /// <summary>
        /// The generate token.
        /// </summary>
        private void GenerateToken()
        {
            for (int index = 0; index < this.terminalTokens.Count; index++)
            {
                TerminalToken terminalToken = this.terminalTokens[index];
                object i = this.CreateObjectFromTerminal(terminalToken, index);
                if (i != null)
                {
                    index = (int)i;
                }
            }
        }

        /// <summary>
        /// The init.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        private void Init(Stream stream)
        {
            var reader = new CGTReader(stream);
            this.parser = reader.CreateNewParser();
            this.parser.TrimReductions = false;
            this.parser.StoreTokens = LALRParser.StoreTokensMode.NoUserObject;

            this.parser.OnTokenError += this.TokenErrorEvent;
            this.parser.OnParseError += this.ParseErrorEvent;
        }

        /// <summary>
        /// The parse error event.
        /// </summary>
        /// <param name="parser">
        /// The parser.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void ParseErrorEvent(LALRParser parser, ParseErrorEventArgs args)
        {
            this.Message = "Parse error caused by token: '" + args.UnexpectedToken + "' at "
                           + args.UnexpectedToken.Location + ". Expected " + args.ExpectedTokens;
        }

        /// <summary>
        /// The token error event.
        /// </summary>
        /// <param name="parser">
        /// The parser.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void TokenErrorEvent(LALRParser parser, TokenErrorEventArgs args)
        {
            this.Message = "Token error with input: '" + args.Token + "'";
        }

        #endregion
    }
}