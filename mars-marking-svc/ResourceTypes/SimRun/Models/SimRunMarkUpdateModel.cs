using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.SimRun.Models
{
    public class SimRunMarkUpdateModel
    {
        [JsonProperty("SimRunId")]
        public string SimRunId { get; set; }

        [JsonProperty("ToBeDeleted")]
        public bool ToBeDeleted { get; set; }

        public SimRunMarkUpdateModel(
            string simRunId,
            bool toBeDeleted
        )
        {
            SimRunId = simRunId;
            ToBeDeleted = toBeDeleted;
        }
    }
}