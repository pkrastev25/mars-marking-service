using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.ResourceTypes.Scenario.Interfaces
{
    public interface IScenarioResourceHandler
    {
        Task<MarkSessionModel> GatherResourcesForMarkSession(MarkSessionModel markSessionModel);
    }
}