using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.Metadata.Interfaces
{
    public interface IMetadataResourceHandler
    {
        Task<IActionResult> MarkMetadataDependantResources(string metadataId, string projectId);
    }
}