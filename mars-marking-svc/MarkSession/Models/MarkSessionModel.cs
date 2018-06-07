using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mars_marking_svc.MarkedResource.Models
{
    public class MarkSessionModel
    {
        public const string StateMarking = "Marking";
        public const string StateUnmarking = "Unmarking";
        public const string StateComplete = "Complete";

        public const string BsomElementDefinitionId = "_id";
        public const string BsonElementDefinitionResourceId = "resourceId";

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement(BsonElementDefinitionResourceId)]
        public string ResourceId { get; set; }

        [BsonElement("projectId")]
        public string ProjectId { get; set; }

        [BsonElement("resourceType")]
        public string ResourceType { get; set; }

        [BsonElement("markSessionType")]
        public string MarkSessionType { get; set; }

        [BsonElement("state")]
        public string State { get; set; }

        [BsonElement("latestUpdateTimestampInTicks")]
        public long LatestUpdateTimestampInTicks { get; set; }

        [BsonElement("sourceDependency")]
        public DependantResourceModel SourceDependency { get; set; }

        [BsonElement("dependantResources")]
        public List<DependantResourceModel> DependantResources { get; set; }

        public MarkSessionModel(
            string resourceId,
            string projectId,
            string resourceType,
            string markSessionType
        )
        {
            ResourceId = resourceId;
            ProjectId = projectId;
            ResourceType = resourceType;
            MarkSessionType = markSessionType;
            State = StateMarking;
            LatestUpdateTimestampInTicks = DateTime.Now.Ticks;
            DependantResources = new List<DependantResourceModel>();
        }

        public override string ToString()
        {
            return
                $"mark session with id: {Id}, type: {MarkSessionType}, state: {State} for resourceType: {ResourceType}, resourceId: {ResourceId}, projectId: {ProjectId}";
        }
    }
}