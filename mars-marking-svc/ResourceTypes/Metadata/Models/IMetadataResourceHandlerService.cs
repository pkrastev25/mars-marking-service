using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.Metadata.Models
{
    public interface IMetadataResourceHandlerService
    {
        Task<IActionResult> MarkMetadataDependantResources(string metadataId);
    }
}