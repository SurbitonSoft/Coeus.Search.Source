// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefBootstrapper.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   Mef Bootstrapper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    /// <summary>Mef Bootstrapper class.</summary>
    internal static class MefBootstrapper
    {
        #region Static Fields

        /// <summary>Composition IOC/DI container.</summary>
        internal static readonly CompositionContainer Container;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes static members of the MefBootstrapper class.</summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", 
            Justification = "The catalog lives as long as the container lives")]
        static MefBootstrapper()
        {
            // A catalog that can aggregate other catalogs  
            var aggrCatalog = new AggregateCatalog();

            // A directory catalog, to load parts from dlls in the Extensions folder  
            var dirCatalog =
                new DirectoryCatalog(
                    Path.GetDirectoryName(Path.GetFullPath(Assembly.GetExecutingAssembly().Location)) + "\\Plugins", 
                    "*.dll");

            // An assembly catalog to load information about part from this assembly  
            var asmCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

            aggrCatalog.Catalogs.Add(dirCatalog);
            aggrCatalog.Catalogs.Add(asmCatalog);

            //Fill the imports of this object  
            Container = new CompositionContainer(
                aggrCatalog, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);
            Container.ComposeParts();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Composes the parts of the application
        /// </summary>
        /// <param name="attributedPart">
        /// Attributed part to compose
        /// </param>
        public static void ComposeParts(object attributedPart)
        {
            Container.ComposeParts(attributedPart);
        }

        /// <summary>
        /// Composes the parts of the application
        /// </summary>
        /// <param name="attributedParts">
        /// Attributed parts to compose
        /// </param>
        public static void ComposeParts(object[] attributedParts)
        {
            Container.ComposeParts(attributedParts);
        }

        /// <summary>
        /// Releases an export
        /// </summary>
        /// <typeparam name="T">
        /// Type to resolve
        /// </typeparam>
        /// <param name="export">
        /// Object instance to release
        /// </param>
        public static void Release<T>(Lazy<T> export)
        {
            Container.ReleaseExport(export);
        }

        /// <summary>
        /// Releases an export
        /// </summary>
        /// <typeparam name="T">
        /// Type to resolve
        /// </typeparam>
        /// <typeparam name="U">
        /// Metadata for the exported type
        /// </typeparam>
        /// <param name="export">
        /// Object instance to release
        /// </param>
        public static void Release<T, U>(Lazy<T, U> export)
        {
            Container.ReleaseExport(export);
        }

        /// <summary>
        /// Releases many exports
        /// </summary>
        /// <typeparam name="T">
        /// Type to resolve
        /// </typeparam>
        /// <param name="exports">
        /// Object instances to release
        /// </param>
        public static void ReleaseMany<T>(IEnumerable<Lazy<T>> exports)
        {
            Container.ReleaseExports(exports);
        }

        /// <summary>
        /// Releases many exports
        /// </summary>
        /// <typeparam name="T">
        /// Type to resolve
        /// </typeparam>
        /// <typeparam name="U">
        /// Metadata for the exported type
        /// </typeparam>
        /// <param name="exports">
        /// Object instances to release
        /// </param>
        public static void ReleaseMany<T, U>(IEnumerable<Lazy<T, U>> exports)
        {
            Container.ReleaseExports<T>(exports);
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Returns an instance of the type</returns>
        public static T Resolve<T>()
        {
            try
            {
                return Container.GetExportedValue<T>();

            }
            catch (ReflectionTypeLoadException ex)
            {
                var errMsg = "Error loading assembly! Assemblies which could not be loaded:" + Environment.NewLine;

                foreach (var loaderException in ex.LoaderExceptions)
                {
                    errMsg += loaderException.Message;
                }

                throw new  Exception(
                    errMsg, ex);
            }
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">
        /// Type to resolve
        /// </typeparam>
        /// <param name="contractName">
        /// Contract name
        /// </param>
        /// <returns>
        /// Returns an instance of the type
        /// </returns>
        public static T Resolve<T>(string contractName)
        {
            return Container.GetExportedValue<T>(contractName);
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Returns an instance of the type</returns>
        public static Lazy<T> ResolveLazy<T>()
        {
            return Container.GetExport<T>();
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">
        /// Type to resolve
        /// </typeparam>
        /// <param name="contractName">
        /// Contract name
        /// </param>
        /// <returns>
        /// Returns an instance of the type
        /// </returns>
        public static Lazy<T> ResolveLazy<T>(string contractName)
        {
            return Container.GetExport<T>(contractName);
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <typeparam name="U">Metadata for the exported type</typeparam>
        /// <returns>Returns an instance of the type</returns>
        public static Lazy<T, U> ResolveLazy<T, U>()
        {
            return Container.GetExport<T, U>();
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">
        /// Type to resolve
        /// </typeparam>
        /// <typeparam name="U">
        /// Metadata for the exported type
        /// </typeparam>
        /// <param name="contractName">
        /// Contract name
        /// </param>
        /// <returns>
        /// Returns an instance of the type
        /// </returns>
        public static Lazy<T, U> ResolveLazy<T, U>(string contractName)
        {
            return Container.GetExport<T, U>(contractName);
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Returns one or more instances of the type</returns>
        public static IEnumerable<T> ResolveMany<T>()
        {
            return Container.GetExportedValues<T>();
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">
        /// Type to resolve
        /// </typeparam>
        /// <param name="contractName">
        /// Contract name
        /// </param>
        /// <returns>
        /// Returns one or more instances of the type
        /// </returns>
        public static IEnumerable<T> ResolveMany<T>(string contractName)
        {
            return Container.GetExportedValues<T>(contractName);
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <returns>Returns one or more instances of the type</returns>
        public static IEnumerable<Lazy<T>> ResolveManyLazy<T>()
        {
            return Container.GetExports<T>();
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">
        /// Type to resolve
        /// </typeparam>
        /// <param name="contractName">
        /// Contract name
        /// </param>
        /// <returns>
        /// Returns one or more instances of the type
        /// </returns>
        public static IEnumerable<Lazy<T>> ResolveManyLazy<T>(string contractName)
        {
            return Container.GetExports<T>(contractName);
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">Type to resolve</typeparam>
        /// <typeparam name="U">Metadata for the exported type</typeparam>
        /// <returns>Returns one or more instances of the type</returns>
        public static IEnumerable<Lazy<T, U>> ResolveManyLazy<T, U>()
        {
            return Container.GetExports<T, U>();
        }

        /// <summary>
        /// Resolves an instance of the type
        /// </summary>
        /// <typeparam name="T">
        /// Type to resolve
        /// </typeparam>
        /// <typeparam name="U">
        /// Metadata for the exported type
        /// </typeparam>
        /// <param name="contractName">
        /// Contract name
        /// </param>
        /// <returns>
        /// Returns one or more instances of the type
        /// </returns>
        public static IEnumerable<Lazy<T, U>> ResolveManyLazy<T, U>(string contractName)
        {
            return Container.GetExports<T, U>(contractName);
        }

        #endregion
    }
}