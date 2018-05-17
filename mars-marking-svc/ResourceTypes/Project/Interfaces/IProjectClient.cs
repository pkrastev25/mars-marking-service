using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Project.Models;

namespace mars_marking_svc.ResourceTypes.Project.Interfaces
{
    public interface IProjectClient
    {
        Task<ProjectModel> GetProject(
            string projectId
        );

        Task<DependantResourceModel> MarkProject(
            string projectId
        );

        Task<DependantResourceModel> MarkProject(
            ProjectModel projectModel
        );

        Task UnmarkProject(
            DependantResourceModel dependantResourceModel
        );
    }
}