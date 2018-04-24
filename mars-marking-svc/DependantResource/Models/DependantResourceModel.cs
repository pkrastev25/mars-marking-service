using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace mars_marking_svc.MarkedResource.Models
{
    // TODO: Consider splitting into a DTO
    public class DependantResourceModel
    {
        [JsonProperty("resourceType")]
        [BsonElement("resourceType")]
        public string ResourceType { get; set; }

        [JsonProperty("resourceId")]
        [BsonElement("resourceId")]
        public string ResourceId { get; set; }

        [JsonIgnore]
        [BsonElement("previousState")]
        public string PreviousState { get; set; }

        public DependantResourceModel(
            string resourceType,
            string resourceId
        )
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