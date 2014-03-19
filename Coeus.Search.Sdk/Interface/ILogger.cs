// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Sdk.Interface
{
    using System;
    using System.Drawing;

    /// <summary>
    /// The Logger interface.
    /// </summary>
    public interface ILogger
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// Gets or sets the logger name.
        /// </summary>
        string LoggerName { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The log debug.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        void LogDebug(string title);

        /// <summary>
        /// The log error.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        void LogError(string title);

        /// <summary>
        /// The log exception.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        void LogException(Exception e);

        /// <summary>
        /// The log exception.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void LogException(string title, Exception e);

        /// <summary>
        /// The log fatal.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        void LogFatal(string title);

        /// <summary>
        /// The log message.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        void LogMessage(string title);

        /// <summary>
        /// The log warning.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        void LogWarning(string title);

        #endregion
    }
}