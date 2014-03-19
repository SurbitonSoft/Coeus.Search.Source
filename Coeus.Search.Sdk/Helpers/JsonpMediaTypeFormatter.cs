// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonpMediaTypeFormatter.cs" company="Coeus Consulting Ltd.">
//   Coeus Search 2012
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Coeus.Search.Sdk.Helpers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    /// <summary>
    /// The jsonp media type formatter.
    /// </summary>
    public class JsonpMediaTypeFormatter : JsonMediaTypeFormatter
    {
        #region Fields

        /// <summary>
        /// Captured name of the Jsonp function that the JSON call
        /// is wrapped in. Set in GetPerRequestFormatter Instance
        /// </summary>
        private readonly string jsonpCallbackFunction;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonpMediaTypeFormatter"/> class.
        /// </summary>
        public JsonpMediaTypeFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

            // MediaTypeMappings.Add(new UriPathExtensionMapping("jsonp", "application/json"));
            this.JsonpParameterName = "callback";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonpMediaTypeFormatter"/> class.
        /// </summary>
        /// <param name="callbackFunction">
        /// The callback function.
        /// </param>
        public JsonpMediaTypeFormatter(string callbackFunction)
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

            // MediaTypeMappings.Add(new UriPathExtensionMapping("jsonp", "application/json"));
            this.jsonpCallbackFunction = callbackFunction;
            this.JsonpParameterName = "callback";
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///  Name of the query string parameter to look for
        ///  the jsonp function name
        /// </summary>
        public string JsonpParameterName { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The can write type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool CanWriteType(Type type)
        {
            return true;
        }

        /// <summary>
        /// Override to wrap existing JSON result with the
        /// JSONP function call
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <param name="value">
        /// </param>
        /// <param name="stream">
        /// </param>
        /// <param name="content">
        /// </param>
        /// <param name="transportContext">
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override Task WriteToStreamAsync(
            Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            if (!string.IsNullOrEmpty(this.jsonpCallbackFunction))
            {
                return Task.Factory.StartNew(
                    () =>
                        {
                            var writer = new StreamWriter(stream);
                            writer.Write(this.jsonpCallbackFunction + "(");
                            writer.Flush();

                            base.WriteToStreamAsync(type, value, stream, content, transportContext).Wait();

                            writer.Write(")");
                            writer.Flush();
                        });
            }

            return base.WriteToStreamAsync(type, value, stream, content, transportContext);
        }

        #endregion
    }
}