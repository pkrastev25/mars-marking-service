using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.ResultConfig.Models
{
    public class ResultConfigModel
    {
        [JsonProperty("ModelId")]
        public string ModelId { get; set; }

        [JsonProperty("ConfigId")]
        public string ConfigId { get; set; }
    }
}