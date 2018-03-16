using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.SimRun.Interfaces
{
    public interface ISimRunResourceHandler
    {
        Task<IActionResult> MarkSimRunDependantResources(string simRunId, string projectId);
    }
}