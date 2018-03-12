using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Models;

namespace mars_marking_svc.ResourceTypes.ResultConfig.Models
{
    public interface IResultConfigServiceClient
    {
        Task<ResultConfigModel> GetResultConfig(string resultConfigId);

        Task<MarkedResourceModel> MarkResultConfig(string resultConfigId);

        Task<MarkedResourceModel> MarkResultConfig(ResultConfigModel resultConfigModel);

        Task<List<ResultConfigModel>> GetResultConfigsForMetadata(string metadataId);
    }
}