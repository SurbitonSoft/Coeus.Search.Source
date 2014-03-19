// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchLogger.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The Search logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Messages;

    /// <summary>
    /// The Search logger.
    /// </summary>
    internal sealed class SearchLogger : ILogger
    {
        #region Static Fields

        /// <summary>
        /// The instance.
        /// </summary>
        private static readonly SearchLogger Instance = new SearchLogger();

        /// <summary>
        /// The disposed.
        /// </summary>
        private static bool Disposed;

        #endregion

        #region Fields

        /// <summary>
        /// The callback.
        /// </summary>
        private readonly TimerCallback callback = Callback;

        /// <summary>
        /// The m_writer block.
        /// </summary>
        private readonly ActionBlock<LoggerMessage> m_writerBlock;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Prevents a default instance of the <see cref="SearchLogger"/> class from being created.
        /// </summary>
        private SearchLogger()
        {
            var writer = new StreamWriter("c:\\temp\\logtester.log", true);
            this.m_writerBlock = new ActionBlock<LoggerMessage>(
                s => writer.WriteLineAsync(s.ToString()), 
                new ExecutionDataflowBlockOptions
                    {
                       MaxDegreeOfParallelism = 1, BoundedCapacity = 1000, SingleProducerConstrained = true 
                    });

            this.m_writerBlock.Completion.ContinueWith(_ => writer.Dispose());

            // Code to handle rotation
            var t = new Timer(this.callback);

            // Figure how much time until 4:00
            DateTime now = DateTime.Now;
            DateTime fourOClock = DateTime.Today.AddHours(16.0);

            // If it's already past 4:00, wait until 4:00 tomorrow    
            if (now > fourOClock)
            {
                fourOClock = fourOClock.AddDays(1.0);
            }

            var msUntilFour = (int)(fourOClock - now).TotalMilliseconds;

            // Set the timer to elapse only once, at 4:00.
            t.Change(msUntilFour, Timeout.Infinite);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SearchLogger"/> class. 
        /// </summary>
        ~SearchLogger()
        {
            this.Dispose(false);
        }

        #endregion

        #region Delegates

        /// <summary>
        /// The write message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        private delegate void WriteMessage(string message);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets the logger name.
        /// </summary>
        public string LoggerName { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The foo.
        /// </summary>
        /// <returns>
        /// The System.Threading.Tasks.Task.
        /// </returns>
        public async Task Foo()
        {
            // Do some work
            await Task.Delay(5000);

            // Do some more work 5 seconds later
        }

        /// <summary>
        /// The log debug.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void LogDebug(string title)
        {
            this.m_writerBlock.SendAsync(
                new LoggerMessage { Message = title, EntryType = LoggerEntryType.Log, TimeStamp = DateTime.Now });
        }

        /// <summary>
        /// The log error.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void LogError(string title)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The log exception.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        public void LogException(Exception e)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// The log fatal.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void LogFatal(string title)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The log message.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void LogMessage(string title)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The log warning.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        public void LogWarning(string title)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The callback.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private static void Callback(object state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                this.m_writerBlock.Complete();
                this.m_writerBlock.Completion.Wait();
            }

            Disposed = true;
        }

        #endregion
    }
}