// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSettingFile.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting Ltd. 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Configuration.Setting
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ServiceModel;

    using Newtonsoft.Json;

    /// <summary>
    /// The global setting file.
    /// </summary>
    public class GlobalSettingFile
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether use windows authentication.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue(HttpClientCredentialType.None)]
        public HttpClientCredentialType ClientCredentialType { get; set; }

        /// <summary>
        /// Gets or sets the cluster name.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue("SearchCluster")]
        public string ClusterName { get; set; }

        /// <summary>
        /// Gets or sets the cors allow origin list.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue("*")]
        public string CorsAllowOriginList { get; set; }

        /// <summary>
        /// Gets or sets the data path.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue(".\\Data")]
        public string DataPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable cors handler.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue(false)]
        public bool EnableCorsHandler { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether enable wcf end point.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue(false)]
        public bool EnableWcfEndPoint { get; set; }

        /// <summary>
        /// Gets or sets the ip address allow list.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue(null)]
        public List<string> IpAddressAllowList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether master node.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue(true)]
        public bool MasterNode { get; set; }

        /// <summary>
        /// Gets or sets the network host.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue("127.0.0.1")]
        public string NetworkHost { get; set; }

        /// <summary>
        /// Gets or sets the network port.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue(9800)]
        public int NetworkPort { get; set; }

        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        [JsonProperty(Required = Required.Default)]
        [DefaultValue("SearchNode")]
        public string NodeName { get; set; }

        #endregion
    }
}