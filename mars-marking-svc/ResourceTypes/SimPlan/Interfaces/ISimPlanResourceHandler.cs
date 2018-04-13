using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.ResourceTypes.SimPlan.Interfaces
{
    public interface ISimPlanResourceHandler
    {
        Task<MarkSessionModel> GatherResourcesForMarkSession(MarkSessionModel markSessionModel);
    }
}