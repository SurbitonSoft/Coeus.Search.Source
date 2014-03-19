// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationRules.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Yaml
{
    using System.Collections.Generic;

    /// <summary>
    /// The ConfigurationValidator interface.
    /// </summary>
    public interface IConfigurationRules
    {
        #region Public Properties

        /// <summary>
        /// Gets the simple item type.
        /// </summary>
        List<ConfigurationObjectRule> Rules { get; }

        #endregion
    }
}