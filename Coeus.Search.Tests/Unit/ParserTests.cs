// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParserTests.cs" company="">
//   
// </copyright>
// <summary>
//   The parser tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Tests.Unit
{
    using System;
    using System.Collections.Generic;

    using Coeus.Search.QueryParser.SearchProfile;

    using NUnit.Framework;

    /// <summary>
    /// The parser tests.
    /// </summary>
    [TestFixture]
    internal class ParserTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The parse field test 3.
        /// </summary>
        [Test]
        public void ParseTests()
        {
            var searchProfileParser = new SearchProfileParser();
            List<Token> tokens;

            // Case 1
            bool result =
                searchProfileParser.Parse(
                    "(MUST firstname <firstname> BOOST 23 lastname <lastname> CONSTANT FUZZY 5)", out tokens);
            Assert.AreEqual(result, true);
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                   { "firstname", "test" }, { "lastname", "test" } 
                };
            string searchString = SearchProfileHelpers.CreateSearchString(tokens, dictionary, 2, "null");
            Assert.AreEqual(searchString, "( +firstname:(test^23) lastname:(lastname) )");

            dictionary.Clear();
            dictionary.Add("lastname", "test");
            searchString = SearchProfileHelpers.CreateSearchString(tokens, dictionary, 1, "null");
            Assert.AreEqual(searchString, "( lastname:(lastname) )");

            // Case 2
            result =
                searchProfileParser.Parse(
                    "(firstname <firstname> BOOST 23 MUST lastname <lastname> FUZZY 5) MUST (id <id>) SHOULD (address1 <asasa> CONSTANT)", 
                    out tokens);
            Assert.AreEqual(result, true);

            dictionary.Clear();
            dictionary.Add("firstname", "test");
            dictionary.Add("lastname", "test");
            dictionary.Add("id", "test");
            dictionary.Add("address1", "test");

            searchString = SearchProfileHelpers.CreateSearchString(tokens, dictionary, 1, "null");
            Assert.AreEqual(
                searchString, "( firstname:(test^23) +lastname:(test~5) ) +( id:(test) ) ( address1:(asasa) )");

            // Case 3
            result = searchProfileParser.Parse("(firstname <firstname>)", out tokens);
            Assert.AreEqual(result, true);

            searchString = SearchProfileHelpers.CreateSearchString(tokens, dictionary, 1, "null");
            Assert.AreEqual(searchString, "( firstname:(test) )");

            // Case 4
            result =
                searchProfileParser.Parse(
                    "(firstname <firstname> BOOST 23 MUSTNOT lastname <lastname> CONSTANT FUZZY 5)", out tokens);
            Assert.AreEqual(result, true);
            searchString = SearchProfileHelpers.CreateSearchString(tokens, dictionary, 1, "null");
            Assert.AreEqual(searchString, "( firstname:(test^23) -lastname:(lastname) )");

            result =
                searchProfileParser.Parse(
                    "(firstname <firstname> BOOST 23 SHOULD lastname <lastname> CONSTANT FUZZY 5) MUSTNOT (id <id>) MUST (address1 <asasa> CONSTANT)", 
                    out tokens);
            Assert.AreEqual(result, true);
            searchString = SearchProfileHelpers.CreateSearchString(tokens, dictionary, 1, "null");
            Assert.AreEqual(
                searchString, "( firstname:(test^23) lastname:(lastname) ) -( id:(test) ) +( address1:(asasa) )");

            dictionary.Remove("id");
            searchString = SearchProfileHelpers.CreateSearchString(tokens, dictionary, 1, "null");
            Assert.AreEqual(searchString, "( firstname:(test^23) lastname:(lastname) )  +( address1:(asasa) )");
        }

        [Test]
        public void ParseTests2()
        {
            var searchProfileParser = new SearchProfileParser();
            List<Token> tokens;

            bool result =
                searchProfileParser.Parse(
                    "(MUST firstname <firstname> WILDCARD lastname <lastname> CONSTANT FUZZY 5)", out tokens);
            Assert.AreEqual(result, true);

            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                   { "firstname", "test" }, { "lastname", "test" } 
                };
            string searchString = SearchProfileHelpers.CreateSearchString(tokens, dictionary, 2, "null");
            Assert.AreEqual(searchString, "( +firstname:(test*) lastname:(lastname) )");
        }

        #endregion
    }
}