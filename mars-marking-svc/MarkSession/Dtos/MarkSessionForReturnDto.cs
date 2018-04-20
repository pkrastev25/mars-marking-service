using System.Collections.Generic;
using mars_marking_svc.MarkedResource.Models;
using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.MarkedResource.Dtos
{
    public class MarkSessionForReturnDto
    {
        [JsonProperty("markSessionId")]
        public string MarkSessionId { get; set; }

        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("dependantResources")]
        public List<DependantResourceModel> DependantResources { get; set; }
    }
}