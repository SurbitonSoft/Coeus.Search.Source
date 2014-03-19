// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrmConnector.cs" company="Coeus Consulting Ltd.">
//   Coeus Consulting Ltd. 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Connector.Crm
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Threading.Tasks.Dataflow;

    using Coeus.Search.Configuration.Setting;
    using Coeus.Search.Sdk.Base;
    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Messages;
    using Coeus.Search.Sdk.Settings;

    /// <summary>
    /// The CRM connector.
    /// </summary>
    [Export(typeof(IConnector))]
    [ExportMetadata("Name", "crm")]
    [ExportMetadata("DisplayName", "MS Dynamics CRM Connector")]
    [ExportMetadata("BulkIndexSupported", true)]
    [ExportMetadata("IncrementalIndexingSupported", true)]
    public class CrmConnector : ConnectorBase
    {
        #region Fields

        /// <summary>
        /// The bulk index commands.
        /// </summary>
        private readonly ConcurrentDictionary<string, string> bulkindexCommands =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The connection string.
        /// </summary>
        private readonly ConcurrentDictionary<string, string> connectionString =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The single index commands.
        /// </summary>
        private readonly ConcurrentDictionary<string, string> singleIndexCommands =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Initialize Connector. Automatically called by Searcher at the time of initialization.
        /// </summary>
        /// <returns>
        /// Operation Status
        /// </returns>
        public override bool InitializeConnector()
        {
            return this.LoadSettings();
        }

        /// <summary>
        /// The process requests.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public override async void ProcessRequests(IndexRequest request)
        {
            int totalRows = 0;
            try
            {
                string sqlConnectionString;
                if (!this.connectionString.TryGetValue(request.IndexName, out sqlConnectionString))
                {
                    if (request.RequestId != Guid.Empty)
                    {
                        this.ConfigurationStore.UpdateJobStatus(
                            request.RequestId, 
                            new StatusMessage
                                {
                                    StatusMessageType = StatusType.Error, 
                                    MessageHeading =
                                        "CRM database connection string is not defined for index " + request.IndexName, 
                                    ProcessedRecords = totalRows
                                });
                    }

                    this.Logger.LogFatal("CRM database connection string is not defined for index " + request.IndexName);
                    return;
                }

                string sqlCommand = string.Empty;
                switch (request.RequestType)
                {
                    case RequestType.BulkIndexDocuments:
                        if (!this.bulkindexCommands.TryGetValue(request.IndexName, out sqlCommand))
                        {
                            if (request.RequestId != Guid.Empty)
                            {
                                this.ConfigurationStore.UpdateJobStatus(
                                    request.RequestId, 
                                    new StatusMessage
                                        {
                                            StatusMessageType = StatusType.Error, 
                                            MessageHeading =
                                                "CRM bulk index command is not defined for index " + request.IndexName, 
                                            ProcessedRecords = totalRows
                                        });
                            }

                            this.Logger.LogFatal("CRM bulk index command is not defined for index " + request.IndexName);
                            return;
                        }

                        break;
                    case RequestType.IndexDocument:
                        if (!this.singleIndexCommands.TryGetValue(request.IndexName, out sqlCommand))
                        {
                            this.Logger.LogFatal(
                                "CRM single index command is not defined for index " + request.IndexName);
                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(request.Id))
                        {
                            sqlCommand += "'" + request.Id + "'";
                        }
                        else
                        {
                            this.Logger.LogError("Request id is mandatory for single index command.");
                            return;
                        }

                        break;
                    default:
                        return;
                }

                using (var con = new SqlConnection(sqlConnectionString))
                {
                    using (
                        var cmd = new SqlCommand
                            {
                                CommandText = sqlCommand, 
                                Connection = con, 
                                CommandTimeout = 300, 
                                CommandType = CommandType.Text
                            })
                    {
                        con.Open();
                        int row = 0;

                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (!rdr.HasRows)
                            {
                                this.Logger.LogWarning(
                                    "No data returned from Sql Server. Please check the configured user has permissions to the associated filtered view.");

                                if (request.RequestId != Guid.Empty)
                                {
                                    this.ConfigurationStore.UpdateJobStatus(
                                        request.RequestId, 
                                        new StatusMessage
                                            {
                                                StatusMessageType = StatusType.FinishedWithErrors, 
                                                MessageHeading =
                                                    "No data returned from Sql Server. Please check the configured user has permissions to the associated filtered view.", 
                                                ProcessedRecords = totalRows
                                            });
                                }

                                return;
                            }

                            if (request.RequestType == RequestType.BulkIndexDocuments)
                            {
                                request.IndexEngine.BulkIndexing(true);
                                this.Logger.LogMessage("Starting Bulkindexing operation for index " + request.IndexName);
                                if (request.RequestId != Guid.Empty)
                                {
                                    this.ConfigurationStore.UpdateJobStatus(
                                        request.RequestId, 
                                        new StatusMessage
                                            {
                                                StatusMessageType = StatusType.Preparing, 
                                                MessageHeading = "Waiting for data from Filtered Views", 
                                                ProcessedRecords = 0
                                            });
                                }
                            }

                            while (await rdr.ReadAsync())
                            {
                                var doc = new Dictionary<string, string>
                                    {
                                       { "id", rdr.GetValue(0).ToString() }, { "type", request.IndexName } 
                                    };

                                row++;

                                for (int i = 1; i < rdr.FieldCount; i++)
                                {
                                    if (!rdr.IsDBNull(i))
                                    {
                                        doc.Add(rdr.GetName(i), rdr.GetValue(i).ToString());
                                    }

                                    // doc.Add(rdr.GetName(i), rdr.IsDBNull(i) ? "<null>" : rdr.GetValue(i).ToString());
                                }

                                await
                                    request.IndexEngine.RequestQueue.SendAsync(
                                        new IndexDocumentRequest
                                            {
                                                DocumentId = rdr[0].ToString(), 
                                                Document = doc, 
                                                DocumentRequestType =
                                                    request.RequestType == RequestType.BulkIndexDocuments
                                                        ? DocumentRequestType.Create
                                                        : DocumentRequestType.Update
                                            });

                                if (request.RequestType == RequestType.BulkIndexDocuments)
                                {
                                    if (row / 50000 == 1)
                                    {
                                        totalRows += row;
                                        row = 0;

                                        this.ConfigurationStore.UpdateJobStatus(
                                            request.RequestId, 
                                            new StatusMessage
                                                {
                                                    StatusMessageType = StatusType.InProgress, 
                                                    MessageHeading = "Indexing in progress", 
                                                    ProcessedRecords = totalRows
                                                });
                                    }
                                }
                            }

                            totalRows += row;

                            if (request.RequestType == RequestType.BulkIndexDocuments)
                            {
                                request.IndexEngine.BulkIndexing(false);
                                this.ConfigurationStore.UpdateJobStatus(
                                    request.RequestId, 
                                    new StatusMessage
                                        {
                                            StatusMessageType = StatusType.FinshedWithSuccess, 
                                            MessageHeading =
                                                "Bulkindexing operation finished for index " + request.IndexName, 
                                            ProcessedRecords = totalRows
                                        });

                                this.Logger.LogMessage("Bulkindexing operation finished for index " + request.IndexName);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.Logger.LogException(exception.Message, exception);
                if (request.RequestId == Guid.Empty)
                {
                    return;
                }

                if (totalRows != 0)
                {
                    this.ConfigurationStore.UpdateJobStatus(
                        request.RequestId, 
                        new StatusMessage
                            {
                                StatusMessageType = StatusType.FinishedWithErrors, 
                                MessageHeading = "Finished with errors", 
                                ProcessedRecords = totalRows
                            });
                }
                else
                {
                    this.ConfigurationStore.UpdateJobStatus(
                        request.RequestId, 
                        new StatusMessage
                            {
                                StatusMessageType = StatusType.Error, 
                                MessageHeading = "Job terminated with errrors.", 
                                MessageBody = exception.Message, 
                                ProcessedRecords = totalRows
                            });
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The load settings.
        /// </summary>
        /// <returns>
        /// The System.Boolean.
        /// </returns>
        private bool LoadSettings()
        {
            if (Directory.Exists(this.ConfigurationStore.ConfPath + "\\Connectors\\Crm\\"))
            {
                foreach (string filePath in
                    Directory.EnumerateFiles(this.ConfigurationStore.ConfPath + "\\Connectors\\Crm\\", "*.json"))
                {
                    CrmSetting setting = JsonSettingBase<CrmSetting>.LoadFromFile(filePath);

                    foreach (CrmSetting.CrmEntity entity in setting.Entities)
                    {
                        IIndexSetting indexSetting;
                        if (
                            !this.ConfigurationStore.GetIndexSetting(
                                setting.OrganisationName + entity.LogicalName, out indexSetting))
                        {
                            // TODO: Error Handling
                        }

                        string bulkIndexCommand = CrmOperations.SqlIndexCommand(
                            entity.PrimaryKey, 
                            entity.LogicalName, 
                            indexSetting, 
                            true, 
                            setting.OrganisationName, 
                            setting.UseDatabaseImpersonation, 
                            setting.AdminUserGuid.ToString());

                        string singleIndexCommand = CrmOperations.SqlIndexCommand(
                            entity.PrimaryKey, 
                            entity.LogicalName, 
                            indexSetting, 
                            false, 
                            setting.OrganisationName, 
                            setting.UseDatabaseImpersonation, 
                            setting.AdminUserGuid.ToString());

                        this.singleIndexCommands.TryAdd(
                            setting.OrganisationName + entity.LogicalName, singleIndexCommand);
                        this.bulkindexCommands.TryAdd(setting.OrganisationName + entity.LogicalName, bulkIndexCommand);
                        this.connectionString.TryAdd(
                            setting.OrganisationName + entity.LogicalName, setting.SqlConnectionString);
                    }
                }
            }
            else
            {
                this.Logger.LogFatal("Directory \\Connectors\\Crm\\ does not exist under the 'Conf' folder.");
                return false;
            }

            return true;
        }

        #endregion
    }
}