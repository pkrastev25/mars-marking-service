using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.ResourceTypes.ResultData
{
    public class ResultDataServiceClient : IResultDataServiceClient
    {
        private readonly ILoggerService _loggerService;

        public ResultDataServiceClient(
            ILoggerService loggerService
        )
        {
            _loggerService = loggerService;
        }

        public async Task<MarkedResourceModel> MarkResultData(string resultDataId)
        {
            return await Task.Run(() =>
            {
                var markedResources = new MarkedResourceModel
                {
                    ResourceType = "resultData",
                    ResourceId = resultDataId
                };
                _loggerService.LogMarkedResource(markedResources);

                return markedResources;
            });
        }

        public async Task<MarkedResourceModel> MarkResultData(SimRunModel simRunModel)
        {
            return await MarkResultData(simRunModel.SimulationId);
        }

        public async Task UnmarkResultData(string resultDataId)
        {
            await Task.Run(() => { _loggerService.LogUnmarkResource("resultData", resultDataId); });
        }
    }
}