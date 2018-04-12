using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.ResourceTypes.ResultData
{
    public class ResultDataClient : IResultDataClient
    {
        private readonly ILoggerService _loggerService;

        public ResultDataClient(
            ILoggerService loggerService
        )
        {
            _loggerService = loggerService;
        }

        public async Task<DependantResourceModel> CreateMarkedResultData(string resultDataId)
        {
            return await Task.Run(() =>
            {
                var markedResources = new DependantResourceModel("resultData", resultDataId);
                _loggerService.LogSkipEvent(markedResources.ToString());

                return markedResources;
            });
        }

        public async Task<DependantResourceModel> CreateMarkedResultData(SimRunModel simRunModel)
        {
            return await CreateMarkedResultData(simRunModel.SimulationId);
        }
    }
}