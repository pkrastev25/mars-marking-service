using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.ProjectContents.Interfaces
{
    public interface IProjectResourceHandler
    {
        Task<IActionResult> MarkProjectDependantResources(string projectId);
    }
}