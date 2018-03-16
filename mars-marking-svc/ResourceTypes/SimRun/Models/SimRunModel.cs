using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.SimRun.Models
{
    public class SimRunModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("OwnerId")]
        public string OwnerId { get; set; }

        [JsonProperty("GroupId")]
        public string GroupId { get; set; }

        [JsonProperty("ProjectId")]
        public string ProjectId { get; set; }

        [JsonProperty("PodName")]
        public string PodName { get; set; }

        [JsonProperty("SimPlanId")]
        public string SimPlanId { get; set; }

        [JsonProperty("SimulationId")]
        public string SimulationId { get; set; }

        [JsonProperty("ConsoleOutput")]
        public string ConsoleOutput { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("CurrentTick")]
        public int CurrentTick { get; set; }

        [JsonProperty("ToBeDeleted")]
        public bool ToBeDeleted { get; set; }
    }
}