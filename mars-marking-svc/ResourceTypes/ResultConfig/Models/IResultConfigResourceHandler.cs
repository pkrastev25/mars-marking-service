using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.ResultConfig.Models
{
    public interface IResultConfigResourceHandler
    {
        Task<IActionResult> MarkResultConfigDependantResources(string resultConfigId, string projectId);
    }
}