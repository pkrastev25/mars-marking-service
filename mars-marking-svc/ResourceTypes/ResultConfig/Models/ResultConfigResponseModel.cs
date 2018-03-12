using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.ResultConfig.Models
{
    public class ResultConfigResponseModel
    {
        [JsonProperty("OutputConfig")]
        public ResultConfigModel ResultConfigModel { get; set; }
    }
}