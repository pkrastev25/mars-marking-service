using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.DependantResource.Interfaces
{
    public interface IDependantResourceHandler
    {
        Task GatherResourcesForMarkSession(MarkSessionModel markSessionModel);

        Task FreeResourcesForMarkSession(MarkSessionModel markSessionModel);
    }
}