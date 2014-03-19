// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexConfigurationLoader.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The configuration loader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Configuration
{
    using System;
    using System.IO;
    using System.Reflection;

    using Coeus.Search.Core.Index;
    using Coeus.Search.Sdk.Interface;

    /// <summary>
    /// The configuration loader.
    /// </summary>
    internal class IndexConfigurationLoader
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILogger Logger = MefBootstrapper.Resolve<ILogger>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The load configuration.
        /// </summary>
        public static void LoadConfiguration()
        {
            if (!Directory.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Conf\\Indices"))
            {
                throw new Exception(
                    string.Format(
                        "Terminating Searcher due to a fatal error. Root cause: Indices configuration directory does not exist at the given path: {0}", 
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Conf\\Indices"));
            }

            foreach (string directoryPath in
                Directory.EnumerateDirectories(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Conf\\Indices"))
            {
                try
                {
                    var builder = new IndexSetting.Builder();
                    builder.IndexConfigurationFile(directoryPath);
                    builder.SearchProfiles(directoryPath);
                    IndexSetting indexSetting = builder.Build();
                    var indexEngine = new IndexEngine(indexSetting.IndexName);
                    MefBootstrapper.ComposeParts(indexEngine);

                    if (!indexEngine.InitializeIndex(indexSetting))
                    {
                        Logger.LogFatal(
                            string.Format(
                                "Index: {0} could not be initialized. The index will not be available for any operations.", 
                                indexSetting.IndexName));
                        continue;
                    }

                    ConfigurationStore.IndexSettingsCollection.TryAdd(indexSetting.IndexName, indexSetting);
                    ConfigurationStore.IndexConnectorsCollection.TryAdd(
                        indexSetting.IndexName, indexSetting.DefaultConnectorName);
                    ConfigurationStore.IndexingEngineCollection.TryAdd(indexSetting.IndexName, indexEngine);
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                    Logger.LogFatal(
                        string.Format(
                            "Index at location: {0} could not be initialized. The index will not be available for any operations. Root cause: {1}", 
                            directoryPath, 
                            e.Message));
                }
            }
        }

        #endregion
    }
}