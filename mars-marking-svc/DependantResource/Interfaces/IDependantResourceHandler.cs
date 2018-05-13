using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.DependantResource.Interfaces
{
    public interface IDependantResourceHandler
    {
        Task MarkResourcesForMarkSession(
            MarkSessionModel markSessionModel
        );

        Task UnmarkResourcesForMarkSession(
            MarkSessionModel markSessionModel
        );
    }
}