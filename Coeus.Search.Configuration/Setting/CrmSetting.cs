// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmSetting.cs" company="Coeus Application Services">
//   Coeus Application Services 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Configuration.Setting
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// The crm setting.
    /// </summary>
    public class CrmSetting
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the admin user domain name.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string AdminUserDomainName { get; set; }

        /// <summary>
        /// Gets or sets the admin user guid.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public Guid AdminUserGuid { get; set; }

        /// <summary>
        /// Gets or sets the entities.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public List<CrmEntity> Entities { get; set; }

        /// <summary>
        /// Gets or sets the organisation name.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string OrganisationName { get; set; }

        /// <summary>
        /// Gets or sets the sql connection string.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string SqlConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use database impersonation.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public bool UseDatabaseImpersonation { get; set; }

        #endregion

        /// <summary>
        /// The entity.
        /// </summary>
        public class CrmEntity
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets the display name.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public string DisplayName { get; set; }

            /// <summary>
            /// Gets or sets the logical name.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public string LogicalName { get; set; }

            /// <summary>
            /// Gets or sets the object code.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public int ObjectCode { get; set; }

            /// <summary>
            /// Gets or sets the primary key.
            /// </summary>
            [JsonProperty(Required = Required.Always)]
            public string PrimaryKey { get; set; }

            #endregion
        }
    }
}