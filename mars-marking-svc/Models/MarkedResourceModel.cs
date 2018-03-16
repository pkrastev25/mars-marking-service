using Newtonsoft.Json;

namespace mars_marking_svc.Models
{
    public class MarkedResourceModel
    {
        [JsonProperty("ResourceType")]
        public string ResourceType { get; set; }

        [JsonProperty("ResourceId")]
        public string ResourceId { get; set; }
    }
}