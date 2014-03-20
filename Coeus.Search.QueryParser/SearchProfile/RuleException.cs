// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuleException.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Coeus.Search.QueryParser.SearchProfile
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The rule exception.
    /// </summary>
    [Serializable]
    public class RuleException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public RuleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public RuleException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected RuleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}