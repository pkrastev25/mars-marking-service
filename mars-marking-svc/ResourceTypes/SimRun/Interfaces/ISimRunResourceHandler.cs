using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.ResourceTypes.SimRun.Interfaces
{
    public interface ISimRunResourceHandler
    {
        Task<MarkSessionModel> GatherResourcesForMarkSession(MarkSessionModel markSessionHandler);
    }
}