using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Models;

namespace mars_marking_svc.Clients.Metadata
{
    public interface IMetadataServiceClient
    {
        Task<MetadataModel> GetMetadata(string metadataId);

        Task<MarkedResourceModel> MarkMetadata(string metadataId);

        Task<MarkedResourceModel> MarkMetadata(MetadataModel metadataModel);

        Task<List<MetadataModel>> GetMetadataForProject(string projectId);
    }
}