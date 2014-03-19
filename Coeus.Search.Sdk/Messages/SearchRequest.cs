namespace Coeus.Search.Sdk.Messages
{
    using System.Runtime.Serialization;

    [DataContract]
    public class SearchRequest
    {
        [DataMember]
        public string QueryString { get; set; }

        [DataMember]
        public int RecordsToReturn { get; set; }

        [DataMember]
        public int RelativeCutOffScore { get; set; }

        [DataMember]
        public bool UseAdvanceParser { get; set; }
    }
}
