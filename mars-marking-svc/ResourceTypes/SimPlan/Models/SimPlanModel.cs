using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.SimPlan.Models
{
    public class SimPlanModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("OwnerId")]
        public string OwnerId { get; set; }

        [JsonProperty("GroupId")]
        public string GroupId { get; set; }

        [JsonProperty("ProjectId")]
        public string ProjectId { get; set; }

        [JsonProperty("ScenarioDescriptionId")]
        public string ScenarioDescriptionId { get; set; }

        [JsonProperty("ResultConfigurationId")]
        public string ResultConfigurationId { get; set; }

        [JsonProperty("ExecutionConfigurationId")]
        public string ExecutionConfigurationId { get; set; }

        [JsonProperty("DockerImageName")]
        public string DockerImageName { get; set; }

        [JsonProperty("DockerImageCreated")]
        public string DockerImageCreated { get; set; }

        [JsonProperty("ToBeDeleted")]
        public bool ToBeDeleted { get; set; }
    }
}