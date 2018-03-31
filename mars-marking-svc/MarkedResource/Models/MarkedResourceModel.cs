using Newtonsoft.Json;

namespace mars_marking_svc.MarkedResource.Models
{
    public class MarkedResourceModel
    {
        [JsonProperty("resourceType")]
        public string ResourceType { get; set; }

        [JsonProperty("resourceId")]
        public string ResourceId { get; set; }

        [JsonProperty("previousState", NullValueHandling = NullValueHandling.Ignore)]
        public string PreviousState { get; set; }

        public MarkedResourceModel(string resourceType, string resourceId)
        {
            ResourceType = resourceType;
            ResourceId = resourceId;
        }

        public override string ToString()
        {
            return $"{ResourceType} with id: {ResourceId}";
        }
    }
}