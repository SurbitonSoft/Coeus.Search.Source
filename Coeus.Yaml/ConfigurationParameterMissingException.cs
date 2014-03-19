// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationParameterMissingException.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Yaml
{
    using System;

    /// <summary>
    /// The configuration parameter missing exception.
    /// </summary>
    [Serializable]
    internal class ConfigurationParameterMissingException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationParameterMissingException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public ConfigurationParameterMissingException(string message)
            : base(message)
        {
        }

        #endregion
    }
}