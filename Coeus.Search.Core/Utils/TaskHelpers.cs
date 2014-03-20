// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelpers.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The task helpers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Utils
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The task helpers.
    /// </summary>
    internal static class TaskHelpers
    {
        #region Public Methods and Operators

        /// <summary>
        /// The milli seconds.
        /// </summary>
        /// <param name="milliSeconds">
        /// The milli Seconds.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task MilliSeconds(this int milliSeconds)
        {
            return Task.Delay(milliSeconds);
        }

        /// <summary>
        /// The seconds.
        /// </summary>
        /// <param name="seconds">
        /// The seconds.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public static Task Seconds(this int seconds)
        {
            return Task.Delay(new TimeSpan(0, 0, seconds));
        }

        #endregion
    }
}