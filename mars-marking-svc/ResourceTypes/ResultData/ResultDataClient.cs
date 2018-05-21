using System;
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
        private readonly string _baseUrl;
        private readonly IHttpService _httpService;

        public ResultDataClient(
            IHttpService httpService
        )
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.Constants.DatabaseUtilitySvcUrlKey);
            _baseUrl = string.IsNullOrEmpty(baseUrl) ? "database-utility-svc:8090" : baseUrl;
            _httpService = httpService;
        }

        public async Task<DependantResourceModel> MarkResultData(
            string resultDataId
        )
        {
            var response = await _httpService.PostAsync(
                $"http://{_baseUrl}/resultData/mongodb-result/{resultDataId}/marks",
                ""
            );

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return new DependantResourceModel(ResourceTypeEnum.ResultData, resultDataId);
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
                $"http://{_baseUrl}/resultData/mongodb-result/{dependantResourceModel.ResourceId}/marks"
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToUpdateResourceException(
                    $"Failed to update resultData with id: {dependantResourceModel.ResourceId} from database-utility-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );
        }
    }
}