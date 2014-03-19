// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmOperations.cs" company="">
//   
// </copyright>
// <summary>
//   The crm operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Connector.Crm
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Coeus.Search.Sdk.Settings;

    /// <summary>
    /// The crm operations.
    /// </summary>
    public class CrmOperations
    {
        #region Public Methods and Operators

        /// <summary>
        /// Generated sql command to be used to get data from Crm
        /// </summary>
        /// <param name="primaryKey">
        /// The primary Key.
        /// </param>
        /// <param name="entityLogicalName">
        /// The entity Logical Name.
        /// </param>
        /// <param name="indexSettings">
        /// The index Settings.
        /// </param>
        /// <param name="bulkIndex">
        /// The bulk Index.
        /// </param>
        /// <param name="organisationName">
        /// The organisation Name.
        /// </param>
        /// <param name="useDatabaseImpersonation">
        /// The use Database Impersonation.
        /// </param>
        /// <param name="adminUserGuid">
        /// The admin User Guid.
        /// </param>
        /// <returns>
        /// returns formatted sql command
        /// </returns>
        public static string SqlIndexCommand(
            string primaryKey, 
            string entityLogicalName, 
            IIndexSetting indexSettings, 
            bool bulkIndex, 
            object organisationName, 
            bool useDatabaseImpersonation, 
            string adminUserGuid)
        {
            var selectStmt = new StringBuilder();
            var attributeNames = new List<string>();

            attributeNames.Insert(0, primaryKey);
            foreach (IndexField attribute in indexSettings.AllFields.Where(a => !a.IsAdditionalAnalysisField))
            {
                string attributeName = attribute.Name;

                // Ignore primary field as it will come from the 
                // crm cofiguration file
                if (attribute.IsPrimary)
                {
                    continue;
                }

                // If it s a date type then we want it to return ISO 8601 date
                // yyyy-mm-ddThh:mi:ss.mmmZ
                if (attribute.DataType == IndexFieldDataType.Datetime)
                {
                    attributeName = "CONVERT(nvarchar(30)," + attributeName + ",126) AS " + attributeName;
                }

                attributeNames.Add(attributeName);
            }

            // Create the sql command to index data
            foreach (string attributeName in attributeNames)
            {
                selectStmt.Append(attributeName).Append(",");
            }

            // Remove the last comma
            selectStmt.Remove(selectStmt.Length - 1, 1);

            if (bulkIndex)
            {
                return string.Format(
                    "{0} SELECT {1} FROM {2}_mscrm.dbo.Filtered{3}", 
                    GetUserContext(useDatabaseImpersonation, adminUserGuid), 
                    selectStmt, 
                    organisationName, 
                    entityLogicalName);
            }

            return string.Format(
                "{0} SELECT {1} FROM {2}_mscrm.dbo.Filtered{3} Where {4} = ", 
                GetUserContext(useDatabaseImpersonation, adminUserGuid), 
                selectStmt, 
                organisationName, 
                entityLogicalName, 
                attributeNames[0]);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the correct user context for database impersonation
        /// </summary>
        /// <param name="useDatabaseImpersonation">
        /// The use Database Impersonation.
        /// </param>
        /// <param name="adminUserGuid">
        /// The admin User Guid.
        /// </param>
        /// <returns>
        /// The get user context.
        /// </returns>
        private static string GetUserContext(bool useDatabaseImpersonation, string adminUserGuid)
        {
            if (useDatabaseImpersonation)
            {
                return string.Format(
                    @"DECLARE @userid UNIQUEIDENTIFIER; SET @userid = '{0}'; SET CONTEXT_INFO @userid;", adminUserGuid);
            }

            return string.Empty;
        }

        #endregion
    }
}