// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerBase.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Threading;
    using System.Web.Http;
    using System.Web.Http.SelfHost;

    using Coeus.Search.Configuration.Setting;
    using Coeus.Search.Core.Analyzer;
    using Coeus.Search.Core.Configuration;
    using Coeus.Search.Core.Handlers;
    using Coeus.Search.Core.Index;
    using Coeus.Search.Core.Utils;
    using Coeus.Search.Core.WCF;
    using Coeus.Search.Core.WebServices;
    using Coeus.Search.Sdk.Helpers;
    using Coeus.Search.Sdk.Interface;

    /// <summary>
    /// Base class to start and stop server operations
    /// </summary>
    internal class ServerBase
    {
        #region Fields

        /// <summary>
        /// The log.
        /// </summary>
        private readonly ILogger logger = MefBootstrapper.Resolve<ILogger>();

        /// <summary>
        /// The configuration path.
        /// </summary>
        private string confPath;

        /// <summary>
        /// The data path.
        /// </summary>
        private string dataPath;

        /// <summary>
        /// The extensions path.
        /// </summary>
        private string extensionsPath;

        /// <summary>
        /// The logs path.
        /// </summary>
        private string logsPath;

        /// <summary>
        /// The server.
        /// </summary>
        private HttpSelfHostServer server;

        /// <summary>
        /// The soap WCF host.
        /// </summary>
        private ServiceHost wcfHost;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the configuration store.
        /// </summary>
        public static ConfigurationStore ConfigurationStore { get; private set; }

        /// <summary>
        /// Gets the connectors.
        /// </summary>
        public static ConcurrentDictionary<string, Tuple<IConnectorMetaData, IConnector>> Connectors { get; private set; }

        /// <summary>
        /// Gets the global setting.
        /// </summary>
        public static GlobalSettingFile GlobalSetting { get; private set; }

        /// <summary>
        /// Gets the operations.
        /// </summary>
        public static ConcurrentDictionary<string, Tuple<IOperationMetaData, IOperation>> Operations { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the connectors factory.
        /// </summary>
        [ImportMany]
        private ExportFactory<IConnector, IConnectorMetaData>[] ConnectorsFactory { get; set; }

        /// <summary>
        /// Gets or sets the operations factory.
        /// </summary>
        [ImportMany]
        private ExportFactory<IOperation, IOperationMetaData>[] OperationsFactory { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            try
            {
                ConfigurationStore = new ConfigurationStore();
                this.InitializeServer();
            }
            catch (Exception e)
            {
                this.logger.LogException(e);
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.logger.LogMessage("Received server shutdown request.");
            if (GlobalSetting.EnableWcfEndPoint)
            {
                this.wcfHost.Close();
                this.logger.LogMessage("Soap services offline.");
            }

            try
            {
                this.server.CloseAsync().Dispose();
            }
            catch
            {

            }

            this.logger.LogMessage("Rest web services offline.");

            foreach (var indexingEngine in ConfigurationStore.IndexingEngineCollection)
            {
                var indexEngine = indexingEngine.Value as IndexEngine;
                if (indexEngine != null)
                {
                    indexEngine.CloseIndex();
                }
            }

            this.logger.LogMessage("Server shutdown completed.");
        }

        #endregion

        #region Methods

        /// <summary>
        /// The catalog composer.
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        private bool CatalogComposer()
        {
            try
            {
                MefBootstrapper.ComposeParts(this);
                if (!this.ConnectorsFactory.Any())
                {
                    Console.WriteLine(
                        "Terminating Searcher due to a fatal error. Root cause: No connector extensions found in Extensions directory. Please check if the necessary extension dlls are placed in the folder.");

                    this.logger.LogFatal(
                        "Terminating Searcher due to a fatal error. Root cause: No connector extensions found in Extensions directory. Please check if the necessary extension dlls are placed in the folder.");
                    return false;
                }

                if (!this.OperationsFactory.Any())
                {
                    Console.WriteLine(
                        "Terminating Searcher due to a fatal error. Root cause: No operation extensions found in Extensions directory. Please check if the necessary extension dlls are placed in the folder.");

                    this.logger.LogFatal(
                        "Terminating Searcher due to a fatal error. Root cause: No operation extensions found in Extensions directory. Please check if the necessary extension dlls are placed in the folder.");
                    return false;
                }

                this.logger.LogMessage(string.Format("Connectors (Count): {0}", this.ConnectorsFactory.Count()));
                foreach (var connector in this.ConnectorsFactory)
                {
                    Connectors.TryAdd(
                        connector.Metadata.Name, 
                        new Tuple<IConnectorMetaData, IConnector>(connector.Metadata, connector.CreateExport().Value));
                    this.logger.LogMessage(string.Format("Loading connector: {0}", connector.Metadata.Name));
                }

                this.logger.LogMessage(string.Format("Operations (Count): {0}", this.OperationsFactory.Count()));
                foreach (var operation in this.OperationsFactory)
                {
                    Operations.TryAdd(
                        operation.Metadata.OperationName, 
                        new Tuple<IOperationMetaData, IOperation>(operation.Metadata, operation.CreateExport().Value));
                    this.logger.LogMessage(string.Format("Loading operation: {0}", operation.Metadata.OperationName));
                }

                this.ConnectorsFactory = null;
                this.OperationsFactory = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Terminating Searcher due to a fatal error. Root cause: Critical error occured while accessing extensions. Please refer to error log for details.");
                this.logger.LogFatal(
                    "Terminating Searcher due to a fatal error. Root cause: Critical error occured while accessing extensions. Please refer to error log for details.");
                this.logger.LogException(ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check all local file system permissions and paths
        /// </summary>
        /// <returns>operation status</returns>
        private bool CheckPathsAndPermissions()
        {
            if (
                !FileSystemPermissions.CheckPathAndPermissions(
                    GlobalSetting.DataPath, "Data", out this.dataPath, this.logger))
            {
                return false;
            }

            if (
                !FileSystemPermissions.CheckPathAndPermissions(
                    ".\\Plugins", "Plugins", out this.extensionsPath, this.logger))
            {
                return false;
            }

            if (!FileSystemPermissions.CheckPathAndPermissions(".\\Logs", "Logs", out this.logsPath, this.logger))
            {
                return false;
            }

            if (!FileSystemPermissions.CheckPathAndPermissions(".\\Conf", "Conf", out this.confPath, this.logger))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// The initialize server.
        /// </summary>
        private void InitializeServer()
        {
            if (!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Conf\\global.json"))
            {
                Console.WriteLine(
                    "Terminating Searcher due to a fatal error. Root cause: Missing 'global.json' in Conf folder.");
                this.logger.LogFatal(
                    "Terminating Searcher due to a fatal error. Root cause: Missing 'global.json' in Conf folder.");
                return;
            }

            try
            {
                GlobalSetting =
                    JsonSettingBase<GlobalSettingFile>.LoadFromFile(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Conf\\global.json");
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Terminating Searcher due to a fatal error. Root cause: Error loading 'global.json' from Conf folder. Error Details: {0}", 
                    e.Message);
                this.logger.LogFatal(
                    string.Format(
                        "Terminating Searcher due to a fatal error. Root cause: Error loading 'global.json' from Conf folder. Error Details: {0}", 
                        e.Message));
                return;
            }

            Console.WriteLine("Server started in interactive mode.");
            Console.WriteLine("Coeus Search Version: " + Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("Coeus Search Cluster: " + GlobalSetting.ClusterName);
            Console.WriteLine("Coeus Search Node: " + GlobalSetting.NodeName);

            // Initialization checks
            Connectors =
                new ConcurrentDictionary<string, Tuple<IConnectorMetaData, IConnector>>(
                    StringComparer.OrdinalIgnoreCase);
            Operations =
                new ConcurrentDictionary<string, Tuple<IOperationMetaData, IOperation>>(
                    StringComparer.OrdinalIgnoreCase);

            // Check if local administrator
            if (!AdministratorPrivledgesCheck.CheckIfAdministrator())
            {
                Console.WriteLine(
                    "Terminating Coeus Search due to a fatal error. Root cause: Coeus Search requires local administrative privledges to run. Please make sure you are running Coeus Search under a local administrator account.");
                this.logger.LogFatal(
                    "Terminating Coeus Search due to a fatal error. Root cause: Coeus Search requires local administrative privledges to run. Please make sure you are running Coeus Search under a local administrator account.");
                return;
            }

            // Check file system permissions
            if (!this.CheckPathsAndPermissions())
            {
                Console.WriteLine(
                    "Terminating Coeus Search due to a fatal error. Root cause: File system permissions failure.");
                this.logger.LogFatal(
                    "Terminating Coeus Search due to a fatal error. Root cause: File system permissions failure.");
                return;
            }

            // Check extension folder
            if (!this.CatalogComposer())
            {
                return;
            }

            // InitialilizeConfigurationStore
            ConfigurationStore.dataPath = this.dataPath;
            ConfigurationStore.pluginsPath = this.extensionsPath;
            ConfigurationStore.confPath = this.confPath;

            // Initialze Analyzers
            try
            {
                AnalyzerFactory.InitialzeAnalyzers();
            }
            catch (Exception e)
            {
                this.logger.LogException(e);
                Console.WriteLine("Terminating Coeus Search due to a fatal error. Root cause: Analyzer loading failure.");
                this.logger.LogFatal("Terminating Coeus Search due to a fatal error. Root cause: Analyzer loading failure.");
                return;
            }

            // Initialize index configuration
            try
            {
                IndexConfigurationLoader.LoadConfiguration();
            }
            catch (Exception e)
            {
                this.logger.LogException(e);

                Console.WriteLine(
                    "Terminating Coeus Search due to a fatal error. Root cause: Index configuration loading failure.");
                this.logger.LogFatal(
                    "Terminating Coeus Search due to a fatal error. Root cause: Index configuration loading failure.");
                return;
            }

            // Initialize operations
            foreach (var operation in Operations)
            {
                try
                {
                    if (!operation.Value.Item2.InitializeOperation())
                    {
                        Console.WriteLine(
                            "Terminating Coeus Search due to a fatal error. Root cause: Unable to initialize operation: "
                            + operation.Value.Item1.OperationName);
                        this.logger.LogFatal(
                            "Terminating Coeus Search due to a fatal error. Root cause: Unable to initialize operation: "
                            + operation.Value.Item1.OperationName);
                        return;
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogException(e);
                    Console.WriteLine(
                        "Terminating Coeus Search due to a fatal error. Root cause: Unable to initialize operation: "
                        + operation.Value.Item1.OperationName);
                    this.logger.LogFatal(
                        "Terminating Coeus Search due to a fatal error. Root cause: Unable to initialize operation: "
                        + operation.Value.Item1.OperationName);
                    return;
                }
            }

            this.logger.LogMessage("All operations initialized successfully.");

            // Initialize connectors
            // Initialize only the connectors which are used
            // Ensure that all connectors are only initialized once
            List<string> connectorNames =
                ConfigurationStore.IndexSettingsCollection.Select(
                    indexSetting => indexSetting.Value.DefaultConnectorName).ToList();

            foreach (string connectorName in connectorNames.Distinct())
            {
                Tuple<IConnectorMetaData, IConnector> connector;
                if (!Connectors.TryGetValue(connectorName, out connector))
                {
                    Console.WriteLine(
                        "Terminating Coeus Search due to a fatal error. Root cause: Unable to find the connector: {0}.", 
                        connectorName);
                    this.logger.LogFatal(
                        string.Format(
                            "Terminating Coeus Search due to a fatal error. Root cause: Unable to find the connector: {0}.", 
                            connectorName));
                    return;
                }

                try
                {
                    if (connector.Item2.InitializeConnector())
                    {
                        continue;
                    }

                    Console.WriteLine(
                        "Terminating Coeus Search due to a fatal error. Root cause: Unable to initialize connector: "
                        + connectorName);
                    this.logger.LogFatal(
                        "Terminating Coeus Search due to a fatal error. Root cause: Unable to initialize connector: "
                        + connectorName);
                    return;
                }
                catch (Exception e)
                {
                    this.logger.LogException(e);
                    Console.WriteLine(
                        "Terminating Coeus Search due to a fatal error. Root cause: Unable to initialize operation: "
                        + connectorName);
                    this.logger.LogFatal(
                        "Terminating Coeus Search due to a fatal error. Root cause: Unable to initialize operation: "
                        + connectorName);
                    return;
                }
            }

            this.logger.LogMessage("All connectors initialized successfully.");

            // Compose Home Controller once
            var homeController = new HomeController();
            MefBootstrapper.ComposeParts(homeController);

            var httpServerThread = new Thread(this.StartHttpEndPoint) { IsBackground = false, Name = "Http Server" };
            httpServerThread.Start();

            if (GlobalSetting.EnableWcfEndPoint)
            {
                var wcfServerThread = new Thread(this.StartWcfEndPoint) { IsBackground = false, Name = "WCF Server" };

                wcfServerThread.Start();
            }
        }

        /// <summary>
        /// The start http end point.
        /// </summary>
        private void StartHttpEndPoint()
        {
            try
            {
                IPAddress.Parse(GlobalSetting.NetworkHost);
            }
            catch (FormatException e)
            {
                this.logger.LogException(e);
                this.logger.LogFatal(
                    string.Format(
                        "The configured ip address for the http server is not in correct format: {0}.", 
                        GlobalSetting.NetworkHost));
                Console.WriteLine(
                    "The configured ip address for the http server is not in correct format: {0}.", 
                    GlobalSetting.NetworkHost);
            }

            var selfHostConfiguration =
                new HttpSelfHostConfiguration(
                    string.Format(@"http:\\{0}:{1}", GlobalSetting.NetworkHost, GlobalSetting.NetworkPort));

            // Routes configuration
            selfHostConfiguration.Routes.MapHttpRoute(
                "DefaultApiRoute", "{indexname}/{operation}", new { controller = "home" });

            selfHostConfiguration.Formatters.Insert(0, new JsonpMediaTypeFormatter());

            // Enable CORS support
            if (GlobalSetting.EnableCorsHandler)
            {
                SimpleCorsHandler.AllowOriginList = GlobalSetting.CorsAllowOriginList;
                selfHostConfiguration.MessageHandlers.Add(new SimpleCorsHandler());
            }

            // Enable ip address
            if (GlobalSetting.IpAddressAllowList != null)
            {
                IpAddressHandler.IpAddress = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string address in GlobalSetting.IpAddressAllowList)
                {
                    IpAddressHandler.IpAddress.Add(address);
                }

                selfHostConfiguration.MessageHandlers.Add(new IpAddressHandler());
            }

            selfHostConfiguration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            selfHostConfiguration.ClientCredentialType = GlobalSetting.ClientCredentialType;

            try
            {
                this.server = new HttpSelfHostServer(selfHostConfiguration);
                this.server.OpenAsync().Wait();
                this.logger.LogMessage("Coeus Search http webservices hosted at " + selfHostConfiguration.BaseAddress);
                Console.WriteLine("Coeus Search http webservices hosted at " + selfHostConfiguration.BaseAddress);
            }
            catch (Exception e)
            {
                this.logger.LogException(
                    "Terminating Coeus Search due to a fatal error. Root cause: A critical error occured while initializing webservice endpoint.", 
                    e);
            }
        }

        /// <summary>
        /// The start WCF end point.
        /// </summary>
        private void StartWcfEndPoint()
        {
            try
            {
                this.wcfHost = new ServiceHost(typeof(WcfSearch));

                // Enable metadata publishing.
                var smb = new ServiceMetadataBehavior
                    {
                       HttpGetEnabled = true, MetadataExporter = {
                                                                      PolicyVersion = PolicyVersion.Policy15 
                                                                  } 
                    };

                this.wcfHost.Description.Behaviors.Add(smb);

                this.wcfHost.AddServiceEndpoint(
                    ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                this.wcfHost.Open();

                Console.WriteLine("Coeus Search wcf webservices hosted at {0}", this.wcfHost.BaseAddresses.First());
                this.logger.LogMessage(
                    string.Format("Coeus Search wcf webservices hosted at {0}", this.wcfHost.BaseAddresses.First()));
            }
            catch (Exception e)
            {
                this.logger.LogException(
                    "Terminating Coeus Search due to a fatal error. Root cause: A critical error occured while initializing webservice endpoint.", 
                    e);
            }
        }

        #endregion
    }
}