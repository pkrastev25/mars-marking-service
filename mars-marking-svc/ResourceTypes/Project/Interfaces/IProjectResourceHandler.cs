using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.ResourceTypes.ProjectContents.Interfaces
{
    public interface IProjectResourceHandler
    {
        Task<MarkSessionModel> GatherResourcesForMarkSession(MarkSessionModel markSessionModel);
    }
}