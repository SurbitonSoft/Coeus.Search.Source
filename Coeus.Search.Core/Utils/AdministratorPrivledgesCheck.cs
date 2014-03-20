// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdministratorPrivledgesCheck.cs" company="Coeus Application Services">
//   Coeus Search 2012
// </copyright>
// <summary>
//   Deals with checking if the local admin privledges
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Utils
{
    using System.Security.Principal;

    /// <summary>
    /// Deals with checking if the local admin privledges
    /// </summary>
    internal class AdministratorPrivledgesCheck
    {
        #region Methods

        /// <summary>
        /// The check if administrator.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal static bool CheckIfAdministrator()
        {
            WindowsIdentity currentUser = WindowsIdentity.GetCurrent();
            if (currentUser != null)
            {
                var wp = new WindowsPrincipal(currentUser);
                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }

            return false;
        }

        #endregion
    }
}