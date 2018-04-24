using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.SimRun.Models;

namespace mars_marking_svc.ResourceTypes.ResultData.Interfaces
{
    public interface IResultDataClient
    {
        Task<DependantResourceModel> GatherResultDataDependantResource(
            string resultDataId
        );

        Task<DependantResourceModel> GatherResultDataDependantResource(
            SimRunModel simRunModel
        );
    }
}