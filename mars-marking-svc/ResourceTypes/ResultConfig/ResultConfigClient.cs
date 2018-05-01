using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;
using mars_marking_svc.Services.Models;
using mars_marking_svc.Utils;

namespace mars_marking_svc.ResourceTypes.ResultConfig
{
    public class ResultConfigClient : IResultConfigClient
    {
        private readonly IHttpService _httpService;
        private readonly ILoggerService _loggerService;

        public ResultConfigClient(
            IHttpService httpService,
            ILoggerService loggerService
        )
        {
            _httpService = httpService;
            _loggerService = loggerService;
        }

        public async Task<ResultConfigModel> GetResultConfig(
            string resultConfigId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://resultcfg-svc/api/ResultConfigs/{resultConfigId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get resultConfig with id: {resultConfigId} from resultcfg-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            var resultConfigResponseModel = await response.Deserialize<ResultConfigResponseModel>();

            return resultConfigResponseModel.ResultConfigModel;
        }

        public async Task<List<ResultConfigModel>> GetResultConfigsForMetadata(
            string metadataId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://resultcfg-svc/api/ResultConfigs?modelDataId={metadataId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get resultConfigs for metadataId: {metadataId} from resultcfg-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            if (response.IsEmptyResponse())
            {
                return new List<ResultConfigModel>();
            }

            var resultConfigResponseModels = await response.Deserialize<List<ResultConfigResponseModel>>();

            return resultConfigResponseModels
                .Select(responseModel => responseModel.ResultConfigModel)
                .ToList();
        }

        public async Task<DependantResourceModel> CreateDependantResultConfigResource(
            string resultConfigId
        )
        {
            var resultConfig = await GetResultConfig(resultConfigId);

            return await CreateDependantResultConfigResource(resultConfig);
        }

        public async Task<DependantResourceModel> CreateDependantResultConfigResource(
            ResultConfigModel resultConfigModel
        )
        {
            return await Task.Run(() =>
            {
                var markedResource =
                    new DependantResourceModel(ResourceTypeEnum.ResultConfig, resultConfigModel.ConfigId);
                _loggerService.LogSkipEvent(markedResource.ToString());

                return markedResource;
            });
        }
    }
}