using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.Scenario.Models
{
    public class ScenarioModel
    {
        [JsonProperty("ScenarioId")]
        public string ScenarioId { get; set; }

        [JsonProperty("ToBeDeleted")]
        public bool ToBeDeleted { get; set; }

        [JsonProperty("ReadOnly")]
        public bool ReadOnly { get; set; }

        public override string ToString()
        {
            return $"scenario model with id: {ScenarioId}";
        }
    }
}