using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;

namespace mars_marking_svc.ResourceTypes.ResultConfig.Interfaces
{
    public interface IResultConfigClient
    {
        Task<ResultConfigModel> GetResultConfig(string resultConfigId);

        Task<List<ResultConfigModel>> GetResultConfigsForMetadata(string metadataId);

        Task<DependantResourceModel> CreateDependantResultConfigResource(string resultConfigId);

        Task<DependantResourceModel> CreateDependantResultConfigResource(ResultConfigModel resultConfigModel);
    }
}