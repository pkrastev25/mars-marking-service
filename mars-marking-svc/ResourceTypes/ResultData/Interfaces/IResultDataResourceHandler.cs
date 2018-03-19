using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.ResultData.Interfaces
{
    public interface IResultDataResourceHandler
    {
        Task<IActionResult> MarkResultDataDependantResources(string resultDataId, string projectId);
    }
}