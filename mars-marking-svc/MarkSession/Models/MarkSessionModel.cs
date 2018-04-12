using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using static mars_marking_svc.Constants.Constants;

namespace mars_marking_svc.MarkedResource.Models
{
    [BsonIgnoreExtraElements(true)]
    public class MarkSessionModel
    {
        public const string MarkingState = "Marking";

        public const string AbortingState = "Aborting";

        public const string DoneState = "Done";

        public string ResourceId { get; set; }

        public string ProjectId { get; set; }

        public string ResourceType { get; set; }

        public string State { get; set; }

        public long MarkSessionExpireTime { get; set; }

        public DependantResourceModel SourceDependency { get; set; }

        public List<DependantResourceModel> DependantResources { get; set; }

        public MarkSessionModel(string resourceId, string projectId, string resourceType)
        {
            ResourceId = resourceId;
            ProjectId = projectId;
            ResourceType = resourceType;
            MarkSessionExpireTime = DateTime.Now.AddTicks(MarkSessionExpireIntervalForUpdateStateTicks).Ticks;
            State = MarkingState;
            DependantResources = new List<DependantResourceModel>();
        }

        public override string ToString()
        {
            return $"mark session for {ResourceType}, id: {ResourceId}, projectId: {ProjectId}, state: {State}";
        }
    }
}