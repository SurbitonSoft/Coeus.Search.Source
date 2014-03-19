// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MalformedInputException.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Yaml
{
    using System;

    /// <summary>
    /// The malformed input exception.
    /// </summary>
    [Serializable]
    public class MalformedInputException : ArgumentException
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MalformedInputException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line Number.
        /// </param>
        /// <param name="parsedText">
        /// The parsed Text.
        /// </param>
        public MalformedInputException(string message, int lineNumber, string parsedText)
            : base(message)
        {
            this.LineNumber = lineNumber;
            this.ParsedText = parsedText;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The line number.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// The parsed text.
        /// </summary>
        public string ParsedText { get; private set; }

        #endregion
    }
}