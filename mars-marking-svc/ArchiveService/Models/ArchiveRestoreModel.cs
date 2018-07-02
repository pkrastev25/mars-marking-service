using Newtonsoft.Json;

namespace mars_marking_svc.ArchiveService.Models
{
    public class ArchiveRestoreModel
    {
        public const string ArchiveRestoreMarkSessionType = "REQUIRED_ONLY_FOR_ARCHIVE";
        public const string ArchiveRestoreProcessingState = "PROCESSING";

        [JsonProperty("Status")]
        public string Status { get; set; }
        
        [JsonProperty("MarkSessionId")]
        public string MarkSessionId { get; set; }
    }
}