using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.SimRun.Models;

namespace mars_marking_svc.ResourceTypes.ResultData.Interfaces
{
    public interface IResultDataServiceClient
    {
        Task<MarkedResourceModel> MarkResultData(string resultDataId);

        Task<MarkedResourceModel> MarkResultData(SimRunModel simRunModel);

        Task UnmarkResultData(string resultDataId);
    }
}