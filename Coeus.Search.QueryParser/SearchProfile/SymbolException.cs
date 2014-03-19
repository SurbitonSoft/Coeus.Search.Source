// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolException.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Coeus.Search.QueryParser.SearchProfile
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The symbol exception.
    /// </summary>
    [Serializable]
    public class SymbolException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public SymbolException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public SymbolException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected SymbolException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}