using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coeus.Search.Sdk.Messages
{
    public class StatusMessage
    {
        public StatusType StatusMessageType { get; set; }
        public string MessageHeading { get; set; }
        public string MessageBody { get; set; }

        public int ProcessedRecords { get; set; }
    }

    public class JobStatus
    {
        public StatusType StatusMessageType { get; set; }
        public string MessageHeading { get; set; }
        public string MessageBody { get; set; }
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }

    public struct JobCreationMessage
    {
        public string Message;
        public Guid JobId;
    }

    public enum StatusType
    {
        Preparing,
        Started,
        InProgress,
        Error,
        FinshedWithSuccess,
        FinishedWithErrors,
        TerminatedOnRequest
    }
}
