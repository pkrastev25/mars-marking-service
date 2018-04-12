using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.MarkSession.Interfaces
{
    public interface IMarkSessionHandler
    {
        Task<IActionResult> UnmarkResourcesForMarkSession(string resourceId);

        Task UnmarkResourcesForMarkSession(MarkSessionModel markSessionModel);
    }
}