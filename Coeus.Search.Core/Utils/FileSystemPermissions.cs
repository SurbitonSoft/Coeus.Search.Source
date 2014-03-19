// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemPermissions.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   Class responsible for file system permissions
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Utils
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.AccessControl;
    using System.Security.Principal;

    using Coeus.Search.Sdk.Interface;

    /// <summary>
    /// Class responsible for file system permissions
    /// </summary>
    internal class FileSystemPermissions
    {
        #region Methods

        /// <summary>
        /// Checks if all the paths are specified in the configration are correct and 
        /// do we have the permissions to use those paths
        /// </summary>
        /// <param name="folderLocation">
        /// The folder Location.
        /// </param>
        /// <param name="pathName">
        /// The path Name.
        /// </param>
        /// <param name="completePath">
        /// THe complete path to the folder 
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <returns>
        /// The check paths and permissions.
        /// </returns>
        internal static bool CheckPathAndPermissions(
            string folderLocation, string pathName, out string completePath, ILogger logger)
        {
            completePath = string.Empty;

            // Check data folder
            // See if it is a relative or absolute path
            if (string.IsNullOrWhiteSpace(folderLocation))
            {
                logger.LogFatal(
                    string.Format("Coeus Search {0} is not specified in the configuration file. Terminating Coeus Search.", pathName));
                return false;
            }

            try
            {
                string dataPath = string.Empty;
                if (folderLocation.StartsWith("."))
                {
                    // To get the location the assembly normally resides on disk or the install directory
                    dataPath = Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location));
                    dataPath += folderLocation.Substring(1);
                }
                else
                {
                    dataPath += folderLocation;
                }

                if (Directory.Exists(dataPath))
                {
                    bool isWriteAccess = false;

                    try
                    {
                        AuthorizationRuleCollection collection =
                            Directory.GetAccessControl(dataPath).GetAccessRules(true, true, typeof(NTAccount));
                        if (
                            collection.Cast<FileSystemAccessRule>().Any(
                                rule => rule.AccessControlType == AccessControlType.Allow))
                        {
                            isWriteAccess = true;
                        }
                    }
                    catch (UnauthorizedAccessException exception)
                    {
                        logger.LogException(
                            string.Format(
                                "Coeus Search {0} specified in configuration cannot be accessed due to missing permissions. Terminating Coeus Search.", 
                                pathName), 
                            exception);
                        return false;
                    }
                    catch (Exception exception)
                    {
                        logger.LogException(
                            string.Format(
                                "Coeus Search {0} specified in configuration cannot be accessed. Terminating Coeus Search.", pathName), 
                            exception);
                        return false;
                    }

                    if (!isWriteAccess)
                    {
                        logger.LogFatal(
                            string.Format(
                                "Coeus Search {0} specified in configuration cannot be accessed due to missing write permissions. Terminating Coeus Search.", 
                                pathName));
                        return false;
                    }
                }
                else
                {
                    logger.LogFatal(
                        string.Format(
                            "Coeus Search {0} specified in configuration does not exist. Terminating Coeus Search.", pathName));
                    return false;
                }

                completePath = dataPath;
            }
            catch (DirectoryNotFoundException exception)
            {
                logger.LogException(
                    string.Format("Coeus Search {0} specified in configuration does not exist. Terminating Coeus Search.", pathName), 
                    exception);
                return false;
            }
            catch (Exception exception)
            {
                logger.LogException(
                    string.Format(
                        "Coeus Search {0} specified in configuration cannot be accessed. Terminating Coeus Search.", pathName), 
                    exception);
                return false;
            }

            return true;
        }

        #endregion
    }
}