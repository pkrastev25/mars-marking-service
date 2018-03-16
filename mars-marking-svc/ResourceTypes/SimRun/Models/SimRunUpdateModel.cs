using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.SimRun.Models
{
    public class SimRunUpdateModel
    {
        public const string AbortVerb = "ABORT";
        
        [JsonProperty("SimRunId")]
        public string SimRunId { get; set; }
        
        [JsonProperty("Verb")]
        public string Verb { get; set; }
    }
}