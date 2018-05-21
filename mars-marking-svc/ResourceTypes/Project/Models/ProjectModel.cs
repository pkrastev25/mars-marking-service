namespace mars_marking_svc.ResourceTypes.Project.Models
{
    public class ProjectModel
    {
        public string Id { get; set; }

        public bool ToBeDeleted { get; set; }

        public ProjectModel(
            string id,
            bool toBeDeleted
        )
        {
            Id = id;
            ToBeDeleted = toBeDeleted;
        }
    }
}