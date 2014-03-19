using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coeus.Search.Sdk.Messages
{
    using System.Drawing;

    public sealed class LoggerMessage
    {
        public LoggerEntryType EntryType { get; set; }

        public LogLevel LogLevel { get; set; }

        public string Message { get; set; }

        public DateTime TimeStamp { get; set; }

        public TimeSpan TimeSpan { get; set; }

        public string Data { get; set; }

        public Color Color { get; set; }

        public string OperationName { get; set; }

        public string UserId { get; set; }

        public override string ToString()
        {
            var message = new StringBuilder();
            message.Append(this.EntryType).Append(",").Append(LogLevel).Append(",").Append(this.Message.Replace(",", string.Empty)).
                Append(",").Append(this.TimeStamp).Append(",").Append(TimeSpan).Append(",").Append(this.Data.Replace(",", string.Empty)).
                Append(",").Append(this.OperationName).Append(",").Append(this.UserId).Append(Color);

            return message.ToString();

        }

    }

    public enum LoggerEntryType
    {
        Log,
        Performance,
        Security,
        Checkpoint,
        WebRequest,
        ServerStart,
        ServerStop
    }

    public enum LogLevel
    {
        Debug,
        Trace,
        Info,
        Warning,
        Error,
        Exception
    }
}
