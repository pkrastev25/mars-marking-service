using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace mars_marking_svc.MarkedResource.Models
{
    public class MarkSessionModel
    {
        public const string MarkingState = "Marking";

        public const string AbortingState = "Aborting";

        public const string DoneState = "Done";

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("resourceId")]
        public string ResourceId { get; set; }

        [BsonElement("projectId")]
        public string ProjectId { get; set; }

        [BsonElement("resourceType")]
        public string ResourceType { get; set; }

        [BsonElement("markSessionType")]
        public string MarkSessionType { get; set; }

        [BsonElement("state")]
        public string State { get; set; }

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
            State = MarkingState;
            DependantResources = new List<DependantResourceModel>();
        }

        public override string ToString()
        {
            return
                $"mark session with id: {Id}, type: {MarkSessionType}, state: {State} for resourceType: {ResourceType}, resourceId: {ResourceId}, projectId: {ProjectId}";
        }
    }
}