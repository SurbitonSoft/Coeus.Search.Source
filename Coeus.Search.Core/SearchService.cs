// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchService.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Coeus.Search.Core
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Threading;

    using Coeus.Search.Core.Utils;

    using Gurock.SmartInspect;

    /// <summary>
    /// The Search service.
    /// </summary>
    [Obfuscation(Exclude = true)]
    public class SearchService : ServiceBase
    {

        /// <summary>
        /// The server base.
        /// </summary>
        private ServerBase serverBase;

        /// <summary>
        /// The on start.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected override void OnStart(string[] args)
        {
            this.serverBase = new ServerBase();
            this.serverBase.Start();
        }

        /// <summary>
        /// The on stop.
        /// </summary>
        protected override void OnStop()
        {
            this.serverBase.Stop();
        }


    }
}