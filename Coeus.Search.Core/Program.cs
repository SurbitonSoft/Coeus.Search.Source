// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;

    using Coeus.Search.Core.Utils;

    using Gurock.SmartInspect;

    using Topshelf;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        /// The main.
        /// </summary>
        private static void Main()
        {
            SiAuto.Si.Enabled = true;
            SiAuto.Si.AppName = "Coeus Search";
            if (!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Conf\\logging.sic"))
            {
                Console.WriteLine("Missing logging.sic in conf folder.");
                return;
            }

            SiAuto.Si.LoadConfiguration(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Conf\\logging.sic");

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += AssemblyResolver.AssemblyResolveEventHandler;

            currentDomain.UnhandledException += (sender, eventArgs) =>
                {
                    var e = (Exception)eventArgs.ExceptionObject;
                    var logger = new Logger("UnhandledException", Color.Red);
                    logger.LogFatal(e.Message);
                    logger.LogException(e);
                };

            HostFactory.Run(
                x =>
                {
                    x.Service<ServerBase>(
                        s =>
                        {
                            s.ConstructUsing(name => new ServerBase());
                            s.WhenStarted(tc => tc.Start());
                            s.WhenStopped(tc => tc.Stop());
                        });
                    x.RunAsLocalSystem();
                    x.SetDescription("Search indexing service with http and wcf server");
                    x.SetDisplayName("Coeus Search Server");
                    x.SetServiceName("SearchIndexingServer");
                    x.EnableServiceRecovery(
                        rc => rc.RestartService(1));
                });
        }

        #endregion
    }
}