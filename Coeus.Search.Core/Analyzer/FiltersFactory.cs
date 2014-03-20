// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FiltersFactory.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The filter factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Analyzer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Coeus.Search.Sdk.Base;
    using Coeus.Search.Sdk.Interface;

    /// <summary>
    /// The filter factory.
    /// </summary>
    internal class FiltersFactory
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersFactory"/> class.
        /// </summary>
        public FiltersFactory()
        {
            MefBootstrapper.ComposeParts(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the factory.
        /// </summary>
        [ImportMany]
        private ExportFactory<TokenFilterFactoryBase, ITokenFilterFactoryMetaData>[] Factory { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get filter.
        /// </summary>
        /// <param name="filterName">
        /// The filter Name.
        /// </param>
        /// <param name="filterSetting">
        /// The filter Setting.
        /// </param>
        /// <returns>
        /// Token Filter
        /// </returns>
        public TokenFilterFactoryBase GetFilter(string filterName, Dictionary<string, string> filterSetting)
        {
            // We have to do this as we need a new instance of Token factory per initialization
            ExportFactory<TokenFilterFactoryBase, ITokenFilterFactoryMetaData> filterMeta =
                this.Factory.FirstOrDefault(
                    a => string.Equals(a.Metadata.FilterName, filterName, StringComparison.OrdinalIgnoreCase));

            if (filterMeta != null)
            {
                ExportLifetimeContext<TokenFilterFactoryBase> filter = filterMeta.CreateExport();

                if (filter != null)
                {
                    if (filter.Value.Initialize(filterSetting))
                    {
                        return filter.Value;
                    }

                    throw new Exception(
                        string.Format(
                            "The requested Filter: {0} cannot be initialized. Please check the filter parameters in the configuration file.", 
                            filterName));
                }
            }

            throw new Exception(
                string.Format(
                    "The requested Filter: {0} does not exist in the system. Please check the filter name in the configuration file.", 
                    filterName));
        }

        #endregion
    }
}