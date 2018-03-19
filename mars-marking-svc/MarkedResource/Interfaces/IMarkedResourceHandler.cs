using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.ResourceTypes.MarkedResource.Interfaces
{
    public interface IMarkedResourceHandler
    {
        Task UnmarkMarkedResources(IEnumerable<MarkedResourceModel> markedResources, string projetcId);
    }
}