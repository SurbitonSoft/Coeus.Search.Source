// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core
{
    using System;
    using System.ComponentModel.Composition;
    using System.Drawing;

    using Coeus.Search.Sdk.Interface;

    using Gurock.SmartInspect;

    /// <summary>
    /// The logger.
    /// </summary>
    [Export(typeof(ILogger))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class Logger : ILogger
    {
        #region Fields

        /// <summary>
        /// The logging session.
        /// </summary>
        private readonly Session loggingSession;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="loggerName">
        /// The logger name.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        public Logger(string loggerName, Color color)
        {
            this.loggingSession = SiAuto.Si.AddSession(loggerName);
            this.LoggerName = loggerName;
            this.Color = color;
            this.loggingSession.Color = this.Color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        public Logger()
        {
            this.loggingSession = SiAuto.Si.AddSession("default");
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public Color Color
        {
            get
            {
                return this.loggingSession.Color;
            }

            set
            {
                this.loggingSession.Color = value;
            }
        }

        /// <summary>
        /// Gets or sets the logger name.
        /// </summary>
        public string LoggerName
        {
            get
            {
                return this.loggingSession.Name;
            }

            set
            {
                this.loggingSession.Name = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The log debug.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void LogDebug(string title)
        {
            this.loggingSession.LogDebug(title);
        }

        /// <summary>
        /// The log error.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void LogError(string title)
        {
            this.loggingSession.LogError(title);
        }

        /// <summary>
        /// The log exception.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public void LogException(Exception e)
        {
            this.loggingSession.LogException(e);
        }

        /// <summary>
        /// The log exception.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        public void LogException(string title, Exception e)
        {
            this.loggingSession.LogException(title, e);
        }

        /// <summary>
        /// The log fatal.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void LogFatal(string title)
        {
            this.loggingSession.LogFatal(title);
        }

        /// <summary>
        /// The log message.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void LogMessage(string title)
        {
            this.loggingSession.LogMessage(title);
        }

        /// <summary>
        /// The log warning.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void LogWarning(string title)
        {
            this.loggingSession.LogWarning(title);
        }

        #endregion
    }
}