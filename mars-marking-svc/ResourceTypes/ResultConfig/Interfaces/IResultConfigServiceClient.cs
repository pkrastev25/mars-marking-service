using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Models;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;

namespace mars_marking_svc.ResourceTypes.ResultConfig.Interfaces
{
    public interface IResultConfigServiceClient
    {
        Task<ResultConfigModel> GetResultConfig(string resultConfigId);

        Task<List<ResultConfigModel>> GetResultConfigsForMetadata(string metadataId);

        Task<MarkedResourceModel> MarkResultConfig(string resultConfigId);

        Task<MarkedResourceModel> MarkResultConfig(ResultConfigModel resultConfigModel);
    }
}