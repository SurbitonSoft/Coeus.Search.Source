// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchServiceInstaller.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Coeus.Search.Core
{
    using System.ComponentModel;
    using System.Configuration.Install;
    using System.Reflection;
    using System.ServiceProcess;

    /// <summary>
    /// The Search service installer.
    /// </summary>
    [RunInstaller(true)]
    [Obfuscation(Exclude = true)]
    public class SearchServiceInstaller : Installer
    {
        #region Fields

        /// <summary>
        /// The process installer.
        /// </summary>
        private readonly ServiceProcessInstaller processInstaller;

        /// <summary>
        /// The service installer.
        /// </summary>
        private readonly ServiceInstaller serviceInstaller;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchServiceInstaller"/> class.
        /// </summary>
        public SearchServiceInstaller()
        {
            this.processInstaller = new ServiceProcessInstaller();
            this.serviceInstaller = new ServiceInstaller();
            this.processInstaller.Account = ServiceAccount.LocalSystem;
            this.serviceInstaller.StartType = ServiceStartMode.Automatic;
            this.serviceInstaller.ServiceName = "SearchIndexingServer";
            this.serviceInstaller.DisplayName = "Coeus Search Server";
            this.serviceInstaller.Description = "Search indexing service with http and wcf server";
            this.Installers.Add(this.serviceInstaller);
            this.Installers.Add(this.processInstaller);
        }

        #endregion
    }
}