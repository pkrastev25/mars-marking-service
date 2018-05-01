using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;
using mars_marking_svc.Utils;

namespace mars_marking_svc.ResourceTypes.ResultData
{
    public class ResultDataClient : IResultDataClient
    {
        private readonly IHttpService _httpService;
        private readonly ILoggerService _loggerService;

        public ResultDataClient(
            IHttpService httpService,
            ILoggerService loggerService
        )
        {
            _httpService = httpService;
            _loggerService = loggerService;
        }

        public async Task<DependantResourceModel> MarkResultData(
            string resultDataId
        )
        {
            var response = await _httpService.PostAsync(
                $"http://database-utility-svc:8090/resultData/mongodb-result/{resultDataId}/marks",
                ""
            );

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var markedResources = new DependantResourceModel(ResourceTypeEnum.ResultData, resultDataId);
                    _loggerService.LogMarkEvent(markedResources.ToString());

                    return markedResources;
                case HttpStatusCode.Conflict:
                    throw new ResourceAlreadyMarkedException(
                        $"Cannot mark resultData with id: {resultDataId}, it is already marked!"
                    );
                default:
                    throw new FailedToUpdateResourceException(
                        $"Failed to update resultData with id: {resultDataId} from database-utility-svc!" +
                        await response.IncludeStatusCodeAndMessageFromResponse()
                    );
            }
        }

        public async Task<DependantResourceModel> MarkResultData(
            SimRunModel simRunModel
        )
        {
            return await MarkResultData(simRunModel.SimulationId);
        }

        public async Task UnmarkResultData(DependantResourceModel dependantResourceModel)
        {
            var response = await _httpService.DeleteAsync(
                $"http://database-utility-svc:8090/resultData/mongodb-result/{dependantResourceModel.ResourceId}/marks"
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToUpdateResourceException(
                    $"Failed to update resultData with id: {dependantResourceModel.ResourceId} from database-utility-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            _loggerService.LogUnmarkEvent(dependantResourceModel.ToString());
        }
    }
}