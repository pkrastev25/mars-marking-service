using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.ResourceTypes.Metadata.Interfaces
{
    public interface IMetadataResourceHandler
    {
        Task<MarkSessionModel> GatherResourcesForMarkSession(MarkSessionModel markSessionModel);
    }
}