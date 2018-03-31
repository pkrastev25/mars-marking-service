using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.ResourceTypes.MarkedResource.Interfaces
{
    public interface IDbMarkSessionHandler
    {
        Task UnmarkResourcesForMarkSession(DbMarkSessionModel markSessionModel);
    }
}