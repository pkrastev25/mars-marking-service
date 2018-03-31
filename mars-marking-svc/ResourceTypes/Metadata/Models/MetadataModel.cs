using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.Metadata.Models
{
    public class MetadataModel
    {
        public const string FinishedState = "FINISHED";

        public const string FailedState = "FAILED";

        public const string ToBeDeletedState = "TO_BE_DELETED";

        [JsonProperty("dataId")]
        public string DataId { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }
}