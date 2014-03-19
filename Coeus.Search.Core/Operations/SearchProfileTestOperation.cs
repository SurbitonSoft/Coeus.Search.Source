// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchProfileTestOperation.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// <summary>
//   The search profile test operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Coeus.Search.Core.Operations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;

    using Coeus.Search.Core.Operations.Response;
    using Coeus.Search.Sdk.Base;
    using Coeus.Search.Sdk.Helpers;
    using Coeus.Search.Sdk.Interface;
    using Coeus.Search.Sdk.Search;
    using Coeus.Search.Sdk.Settings;

    using Microsoft.VisualBasic.FileIO;

    /// <summary>
    /// The search profile test operation.
    /// </summary>
    [Export(typeof(IOperation))]
    [ExportMetadata("OperationName", "searchprofiletest")]
    [ExportMetadata("GetSupported", true)]
    internal class SearchProfileTestOperation : OperationBase
    {
        #region Constants

        /// <summary>
        /// The html begin tag.
        /// </summary>
        private const string HtmlBeginTag =
            "<html><title>Search Profile Report</title><style type=\"text/css\">"
            + "h1 { font-size: 20px; font-weight:bold; padding: 10px 1px;;color: #039;margin: 45px; }"
            +
            "body {font-family: \"Lucida Sans Unicode\", \"Lucida Grande\", Sans-Serif;font-size: 12px;background: #fff;margin: 45px;}"
            +
            "#hor-minimalist{font-family: \"Lucida Sans Unicode\", \"Lucida Grande\", Sans-Serif;font-size: 12px;background: #fff;margin: 45px;width: 100%;border-collapse: collapse;text-align: center;}"
            +
            "#hor-minimalist th{font-size: 14px;font-weight: normal;color: #039;padding: 10px 1px;border-bottom: 2px solid #6678b1;}"
            + "#hor-minimalist td{border-bottom: 1px solid #ccc;color: #669;padding: 6px 8px;}"
            + "#hor-minimalist tbody tr:hover td{color: #009;}" + "</style></head><body>";

        /// <summary>
        /// The table begin tag.
        /// </summary>
        private const string TableBeginTag = "<TABLE id=\"hor-minimalist\">";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The execute method which will be called by Searcher.
        /// The calss will not be initialized per request but be cached by
        /// Search Server for performance reason. So please ensure that the code does
        /// not rely on constructor or class initialization.
        /// </summary>
        /// <param name="indexName">
        /// The index name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="verb">
        /// The verb.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The System.Net.Http.HttpResponseMessage.
        /// </returns>
        public override HttpResponseMessage Execute(
            string indexName, Dictionary<string, string> parameters, HttpVerbs verb, string callback)
        {
            string profilenamestring;
            if (!parameters.TryGetValue("profilename", out profilenamestring))
            {
                profilenamestring = "default";
            }

            // Check if it contains multiple profile names
            string[] profiles = profilenamestring.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            string inputfilepath;
            if (!parameters.TryGetValue("inputfilepath", out inputfilepath))
            {
                return HttpResponseStore.MissingParameterResponse(callback, "inputfilepath");
            }

            if (!File.Exists(inputfilepath))
            {
                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<string>(
                            "The file does not exist.",
                            callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)),
                        StatusCode = HttpStatusCode.BadRequest
                    };
            }

            IIndexEngine engine;
            if (!this.ConfigurationStore.GetIndexingEngineForIndex(indexName, out engine))
            {
                return HttpResponseStore.MissingIndexingEngineResponse(callback);
            }

            IIndexSetting indexSetting;
            if (!this.ConfigurationStore.GetIndexSetting(indexName, out indexSetting))
            {
                return HttpResponseStore.MissingIndexSettingResponse(callback);
            }

            string resultstoreturn;
            int totalResults = 10;
            if (parameters.TryGetValue("resultstoreturn", out resultstoreturn))
            {
                if (!int.TryParse(resultstoreturn, out totalResults))
                {
                    totalResults = 10;
                }
            }

            string relativeCutOff;
            var relativeCutOffScores = new List<int>();
            if (parameters.TryGetValue("relativecutoff", out relativeCutOff))
            {
                int relativeCutOffScore;
                if (!int.TryParse(relativeCutOff, out relativeCutOffScore))
                {
                    // This can mean it could be a comma separated list
                    // Check if it contains multiple profile names
                    string[] cutoffs = relativeCutOff.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string cutoff in cutoffs)
                    {
                        int i;
                        if (int.TryParse(cutoff, out i))
                        {
                            relativeCutOffScores.Add(i);
                        }
                    }
                }
                else
                {
                    relativeCutOffScores.Add(relativeCutOffScore);
                }
            }
            else
            {
                relativeCutOffScores.Add(0);
            }

            string returnfields;
            string[] fieldsToReturn = parameters.TryGetValue("fieldsToReturn", out returnfields)
                                          ? returnfields.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                          : indexSetting.AllSearchableFields;

            try
            {
                if (engine != null)
                {
                    // Load file
                    string[] currentRow;
                    string[] headerRow = default(string[]);
                    var keyValue = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    var html = new StringBuilder();

                    html.Append(HtmlBeginTag);
                    html.Append("Report Runtime: ").Append(DateTime.Now).Append("</br>");

                    using (var textFieldParser = new TextFieldParser(inputfilepath))
                    {
                        textFieldParser.TextFieldType = FieldType.Delimited;
                        textFieldParser.Delimiters = new[] { "," };
                        textFieldParser.HasFieldsEnclosedInQuotes = false;
                        textFieldParser.TrimWhiteSpace = true;
                        int i = 0;
                        while (!textFieldParser.EndOfData)
                        {
                            currentRow = textFieldParser.ReadFields();
                            if (currentRow == null)
                            {
                                continue;
                            }

                            if (i == 0)
                            {
                                // Process header row
                                headerRow = currentRow;
                                i++;
                                continue;
                            }

                            if (headerRow == null)
                            {
                                continue;
                            }

                            keyValue.Clear();
                            for (int index = 0; index < currentRow.Length; index++)
                            {
                                keyValue.Add(headerRow[index], currentRow[index]);
                            }

                            html.Append("<h1>Record:").Append(i).Append("</h1>");
                            i++;

                            // Input value table
                            html.Append(TableBeginTag).Append("<thead><tr>");
                            foreach (var s in keyValue)
                            {
                                html.Append("<th>").Append(s.Key).Append("</th>");
                            }

                            html.Append("</tr></thead>");
                            html.Append("<tbody><tr>");
                            foreach (var val in keyValue)
                            {
                                html.Append("<TD>").Append(val.Value).Append("</TD>");
                            }

                            html.Append("</tr></tbody></table>");
                            html.Append("</br>");

                            foreach (string profile in profiles)
                            {
                                foreach (int relativeCutOffScore in relativeCutOffScores)
                                {
                                    GetResultBody(
                                        keyValue,
                                        totalResults,
                                        engine,
                                        indexSetting,
                                        fieldsToReturn,
                                        html,
                                        profile,
                                        relativeCutOffScore);
                                    html.Append("</br>");
                                }
                            }
                        }

                        html.Append("</body></html>");

                        using (var file = new StreamWriter(inputfilepath + DateTime.Now.Ticks + ".html", true))
                        {
                            file.Write(html);
                        }
                    }
                }

                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<string>(
                            "Requested report is generated.",
                            callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)),
                        StatusCode = HttpStatusCode.OK
                    };
            }
            catch (Exception e)
            {
                return new HttpResponseMessage
                    {
                        Content =
                            new ObjectContent<string>(
                            e.Message,
                            callback == null ? new JsonMediaTypeFormatter() : new JsonpMediaTypeFormatter(callback)),
                        StatusCode = HttpStatusCode.BadRequest
                    };
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get result body.
        /// </summary>
        /// <param name="keyValue">
        /// The key value.
        /// </param>
        /// <param name="totalResults">
        /// The total results.
        /// </param>
        /// <param name="engine">
        /// The engine.
        /// </param>
        /// <param name="indexSetting">
        /// The index setting.
        /// </param>
        /// <param name="fieldsToReturn">
        /// The fields to return.
        /// </param>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="profilename">
        /// The profilename.
        /// </param>
        /// <param name="relativeCutOffScore">
        /// The relative cut off score.
        /// </param>
        private static void GetResultBody(
            Dictionary<string, string> keyValue,
            int totalResults,
            IIndexEngine engine,
            IIndexSetting indexSetting,
            string[] fieldsToReturn,
            StringBuilder html,
            string profilename,
            int relativeCutOffScore)
        {
            List<Dictionary<string, string>> result;
            result = SearchEngine.DuplicateDetection(
                keyValue,
                profilename,
                totalResults,
                relativeCutOffScore,
                null,
                engine.IndexSearcher,
                indexSetting,
                fieldsToReturn);

            // Result table
            html.Append(TableBeginTag).Append("<thead><tr>");

            if (!result.Any())
            {
                html.Append("No Results").Append("</tr></thead>").Append("<tfoot><tr>").Append("Profile Name: ").Append(
                    profilename).Append(" Relative Cutoff: ").Append(relativeCutOffScore).Append(" </tr></tfoot>").
                    Append("</table>");
                return;
            }

            foreach (var s in result.First())
            {
                html.Append("<th>").Append(s.Key).Append("</th>");
            }

            html.Append("</tr></thead>").Append("<tfoot><tr>").Append("Profile Name: ").Append(profilename).Append(
                " Relative Cutoff: ").Append(relativeCutOffScore).Append(" </tr></tfoot>");

            html.Append("<tbody>");

            // Add results to out html string
            foreach (var res in result)
            {
                html.Append("<tr>");
                foreach (var keyVal in res)
                {
                    string value;
                    if (keyValue.TryGetValue(keyVal.Key, out value))
                    {
                        if (string.Equals(value, keyVal.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            html.Append("<td>").Append(keyVal.Value).Append("</td>");
                        }
                        else
                        {
                            html.Append("<td><font style=\"background-color: yellow\">").Append(keyVal.Value).Append("</font></td>");
                        }
                    }
                    else
                    {
                        html.Append("<td>").Append(keyVal.Value).Append("</td>");
                    }
                }

                html.Append("</tr>");
            }

            html.Append("</tr></tbody></table>");
        }

        #endregion
    }
}