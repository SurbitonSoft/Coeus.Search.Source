// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvConnector.cs" company="Coeus Application Services">
//   Coeus Application Services 2012
// </copyright>
// <summary>
//   The CSV connector.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Connector.Csv
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks.Dataflow;

    using Coeus.Search.Configuration.Setting;
    using Coeus.Search.Sdk.Base;
    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Messages;
    using Coeus.Search.Sdk.Settings;

    using Microsoft.VisualBasic.FileIO;

    /// <summary>
    /// The CSV connector.
    /// </summary>
    [Export(typeof(IConnector))]
    [ExportMetadata("Name", "csv")]
    [ExportMetadata("DisplayName", "CSV Data Connector")]
    [ExportMetadata("BulkIndexSupported", true)]
    public class CsvConnector : ConnectorBase
    {
        #region Fields

        /// <summary>
        /// The CSV settings.
        /// </summary>
        private Dictionary<string, CsvSetting.CsvConfiguration> csvSettings;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The initialize connector.
        /// </summary>
        /// <returns>
        /// The System.Boolean.
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
            bool errors = false;
            CsvSetting.CsvConfiguration csvSetting;
            if (!this.csvSettings.TryGetValue(request.IndexName, out csvSetting))
            {
                this.ConfigurationStore.UpdateJobStatus(
                    request.RequestId, 
                    new StatusMessage
                        {
                            StatusMessageType = StatusType.Error, 
                            MessageHeading = "No csv setting is associated with the given index.", 
                            ProcessedRecords = 0
                        });
                return;
            }

            if (!Directory.Exists(csvSetting.CsvFolderPath))
            {
                this.ConfigurationStore.UpdateJobStatus(
                    request.RequestId, 
                    new StatusMessage
                        {
                            StatusMessageType = StatusType.Error, 
                            MessageHeading = "The configured csv directory does not exist.", 
                            ProcessedRecords = 0
                        });
                return;
            }

            if (!Directory.EnumerateFiles(csvSetting.CsvFolderPath, "*.*").Any())
            {
                this.ConfigurationStore.UpdateJobStatus(
                    request.RequestId, 
                    new StatusMessage
                        {
                            StatusMessageType = StatusType.Error, 
                            MessageHeading = "The passed directory does not contain any file.", 
                            ProcessedRecords = 0
                        });
                return;
            }

            IIndexSetting indexSetting;
            if (!this.ConfigurationStore.GetIndexSetting(request.IndexName, out indexSetting))
            {
                Debug.Fail("Csv connector: No configuration found for index");
            }

            request.IndexEngine.BulkIndexing(true);
            int fileCount = 0;
            foreach (string filePath in Directory.EnumerateFiles(csvSetting.CsvFolderPath, "*.*"))
            {
                this.ConfigurationStore.UpdateJobStatus(
                    request.RequestId, 
                    new StatusMessage
                        {
                            StatusMessageType = StatusType.InProgress, 
                            MessageHeading = "Indexing CSV file at " + filePath, 
                            ProcessedRecords = fileCount
                        });

                fileCount++;

                using (var textFieldParser = new TextFieldParser(filePath))
                {
                    try
                    {
                        switch (csvSetting.TextFieldType)
                        {
                            case CsvSetting.CsvTextFieldType.Delimited:
                                textFieldParser.TextFieldType = FieldType.Delimited;
                                break;
                            case CsvSetting.CsvTextFieldType.FixedWidth:

                                // TODO: Check if field width is specified or not 
                                // If not then throw an error
                                textFieldParser.TextFieldType = FieldType.FixedWidth;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        textFieldParser.Delimiters = new[] { csvSetting.Delimiter };

                        textFieldParser.HasFieldsEnclosedInQuotes = csvSetting.HasFieldsEnclosedInQuotes;
                        textFieldParser.TrimWhiteSpace = csvSetting.TrimWhiteSpace;
                        string[] currentRow;
                        int i = 0;
                        while (!textFieldParser.EndOfData)
                        {
                            if (i < csvSetting.StartingLinesToIgnore)
                            {
                                i++;
                                continue;
                            }

                            currentRow = textFieldParser.ReadFields();
                            if (currentRow == null)
                            {
                                continue;
                            }

                            try
                            {
                                // This is really import to perform 1 to 1 mapping
                                // Alternate versions of the field will go into additional analysis
                                // if (currentRow.Count() != indexSetting.BaseFields.Count())
                                // {
                                // continue;
                                // }
                                var doc = new Dictionary<string, string>();

                                for (int index = 0; index < currentRow.Count(); index++)
                                {
                                    if (csvSetting.FirstColumnIsKey && index == 0)
                                    {
                                        continue;
                                    }

                                    if (indexSetting.BaseFields.Count >= index)
                                    {
                                        if (csvSetting.FirstColumnIsKey)
                                        {
                                            string fieldName = indexSetting.BaseFields.ElementAt(index - 1).Name;
                                            doc.Add(fieldName, currentRow[index]);
                                        }
                                        else
                                        {
                                            string fieldName = indexSetting.BaseFields.ElementAt(index - 1).Name;
                                            doc.Add(fieldName, currentRow[index]);
                                        }
                                    }
                                }

                                await
                                    request.IndexEngine.RequestQueue.SendAsync(
                                        new IndexDocumentRequest
                                            {
                                                DocumentId = csvSetting.FirstColumnIsKey ? currentRow[0] : string.Empty, 
                                                Document = doc, 
                                                DocumentRequestType = DocumentRequestType.Create
                                            });
                            }
                            catch (MalformedLineException malformedLineException)
                            {
                                this.Logger.LogError(
                                    "Line " + malformedLineException.Message + " is invalid. File path: " + filePath);
                                errors = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.Logger.LogError(e.Message + " File path: " + filePath);
                        errors = true;
                    }
                }
            }

            if (errors)
            {
                this.ConfigurationStore.UpdateJobStatus(
                    request.RequestId, 
                    new StatusMessage
                        {
                            StatusMessageType = StatusType.FinishedWithErrors, 
                            MessageHeading =
                                "Finished bulk indexing Csv files. Some input files had errors. Please refer to the error logs to get the error details.", 
                            ProcessedRecords = fileCount
                        });
            }
            else
            {
                this.ConfigurationStore.UpdateJobStatus(
                    request.RequestId, 
                    new StatusMessage
                        {
                            StatusMessageType = StatusType.FinshedWithSuccess, 
                            MessageHeading = "Finished bulk indexing Csv files.", 
                            ProcessedRecords = fileCount
                        });
            }

            request.IndexEngine.BulkIndexing(false);
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
            this.csvSettings = new Dictionary<string, CsvSetting.CsvConfiguration>(StringComparer.OrdinalIgnoreCase);
            if (File.Exists(this.ConfigurationStore.ConfPath + "\\Connectors\\csv_connector.json"))
            {
                CsvSetting settings =
                    JsonSettingBase<CsvSetting>.LoadFromFile(
                        this.ConfigurationStore.ConfPath + "\\Connectors\\csv_connector.json");

                foreach (CsvSetting.CsvConfiguration setting in settings.Csv)
                {
                    this.csvSettings.Add(setting.AssociatedIndexName, setting);
                }
            }
            else
            {
                this.Logger.LogFatal("File \\Connectors\\csv_connector.json does not exist under the 'Conf' folder.");
                return false;
            }

            return true;
        }

        #endregion
    }
}