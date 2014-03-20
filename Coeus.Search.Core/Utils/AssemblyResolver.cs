// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyResolver.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   Resolves all assembly requests
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Utils
{
    using System;
    using System.IO;
    using System.Reflection;

    using Coeus.Search.Sdk.Interface;

    /// <summary>
    /// Resolves all assembly requests
    /// </summary>
    internal class AssemblyResolver
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILogger Logger = MefBootstrapper.Resolve<ILogger>();

        #endregion

        #region Methods

        /// <summary>
        /// The my resolve event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The System.Reflection.Assembly.
        /// </returns>
        internal static Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            string strTempAssmbPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Lib\"
                                      + args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal))
                                      + ".dll";

            try
            {
                // Load the assembly from the specified path
                return Assembly.LoadFrom(strTempAssmbPath);
            }
            catch (Exception e)
            {
                Logger.LogException(
                    string.Format(
                        "Unable to load the requested dll {0} from 'ThirdPartyDlls' folder. Please ensure that the requested dll is present at the location.", 
                        args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal)) + ".dll"), 
                    e);
            }

            return null;
        }

        #endregion
    }
}