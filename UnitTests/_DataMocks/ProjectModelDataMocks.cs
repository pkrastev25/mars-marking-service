using mars_marking_svc.ResourceTypes.Project.Models;

namespace UnitTests._DataMocks
{
    public static class ProjectModelDataMocks
    {
        public static ProjectModel MockMarkedProjectModel()
        {
            return new ProjectModel
            {
                Id = "be1cabd5-c121-49a0-9860-824419efb39a",
                ToBeDeleted = true
            };
        }
    }
}