using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Models;
using mars_marking_svc.ResourceTypes.Metadata.Models;

namespace mars_marking_svc.ResourceTypes.Metadata.Interfaces
{
    public interface IMetadataServiceClient
    {
        Task<MetadataModel> GetMetadata(string metadataId);

        Task<List<MetadataModel>> GetMetadataForProject(string projectId);
        
        Task<MarkedResourceModel> MarkMetadata(string metadataId);

        Task<MarkedResourceModel> MarkMetadata(MetadataModel metadataModel);
    }
}