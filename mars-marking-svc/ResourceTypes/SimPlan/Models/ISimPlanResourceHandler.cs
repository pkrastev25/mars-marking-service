using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.SimPlan.Models
{
    public interface ISimPlanResourceHandler
    {
        Task<IActionResult> MarkSimPlanDependantResources(string simPlanId, string projectId);
    }
}