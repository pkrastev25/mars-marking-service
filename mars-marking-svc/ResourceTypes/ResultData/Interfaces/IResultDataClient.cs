using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.SimRun.Models;

namespace mars_marking_svc.ResourceTypes.ResultData.Interfaces
{
    public interface IResultDataClient
    {
        Task<DependantResourceModel> CreateMarkedResultData(string resultDataId);

        Task<DependantResourceModel> CreateMarkedResultData(SimRunModel simRunModel);
    }
}