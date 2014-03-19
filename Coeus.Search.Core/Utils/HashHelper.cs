// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashHelper.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The hash helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Utils
{
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// The hash helper.
    /// </summary>
    internal static class HashHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get ascii hash.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetAsciiHash(this string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                int sum = input.ToLower(CultureInfo.InvariantCulture).Aggregate(
                    0, (current, character) => current + Convert.ToInt16(character));
                return sum;
            }

            return 0;
        }

        #endregion
    }
}