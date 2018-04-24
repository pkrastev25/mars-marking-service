using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Metadata.Models;

namespace mars_marking_svc.ResourceTypes.Metadata.Interfaces
{
    public interface IMetadataClient
    {
        Task<MetadataModel> GetMetadata(
            string metadataId
        );

        Task<List<MetadataModel>> GetMetadataForProject(
            string projectId
        );

        Task<DependantResourceModel> MarkMetadata(
            string metadataId
        );

        Task<DependantResourceModel> MarkMetadata(
            MetadataModel metadataModel
        );

        Task UnmarkMetadata(
            DependantResourceModel dependantResourceModel
        );
    }
}