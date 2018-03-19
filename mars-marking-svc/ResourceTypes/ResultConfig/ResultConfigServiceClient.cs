﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;
using mars_marking_svc.Services.Models;
using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.ResultConfig
{
    public class ResultConfigServiceClient : IResultConfigServiceClient
    {
        private readonly IHttpService _httpService;
        private readonly IMetadataServiceClient _metadataServiceClient;
        private readonly ILoggerService _loggerService;

        public ResultConfigServiceClient(
            IHttpService httpService,
            IMetadataServiceClient metadataServiceClient,
            ILoggerService loggerService
        )
        {
            _httpService = httpService;
            _metadataServiceClient = metadataServiceClient;
            _loggerService = loggerService;
        }

        public async Task<ResultConfigModel> GetResultConfig(string resultConfigId)
        {
            var response = await _httpService.GetAsync($"http://resultcfg-svc/api/ResultConfigs/{resultConfigId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get resultConfig with id: {resultConfigId} from resultcfg-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ResultConfigResponseModel>(jsonResponse).ResultConfigModel;
        }

        public async Task<List<ResultConfigModel>> GetResultConfigsForMetadata(string metadataId)
        {
            var response =
                await _httpService.GetAsync($"http://resultcfg-svc/api/ResultConfigs?modelDataId={metadataId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get resultConfigs for metadataId: {metadataId} from resultcfg-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var resultConfigResponseModel =
                JsonConvert.DeserializeObject<List<ResultConfigResponseModel>>(jsonResponse);

            return resultConfigResponseModel
                .Select(responseModel => responseModel.ResultConfigModel)
                .ToList();
        }

        public async Task<MarkedResourceModel> MarkResultConfig(string resultConfigId)
        {
            var resultConfig = await GetResultConfig(resultConfigId);

            return await MarkResultConfig(resultConfig);
        }

        public async Task<MarkedResourceModel> MarkResultConfig(ResultConfigModel resultConfigModel)
        {
            var metadataForResultConfig = await _metadataServiceClient.GetMetadata(resultConfigModel.ModelId);
            // TODO: Do not forget to remove the marks later!
            await _metadataServiceClient.MarkMetadata(metadataForResultConfig);
            var markedResource = new MarkedResourceModel
            {
                ResourceType = "resultConfig",
                ResourceId = resultConfigModel.ConfigId
            };
            _loggerService.LogMarkedResource(markedResource);

            return markedResource;
        }

        public async Task UnmarkResultConfig(string resultConfigId)
        {
            var resultConfigModel = await GetResultConfig(resultConfigId);
            var metadataForResultConfig = await _metadataServiceClient.GetMetadata(resultConfigModel.ModelId);
            await _metadataServiceClient.UnmarkMetadata(metadataForResultConfig.DataId);
            _loggerService.LogUnmarkResource("resultConfig", resultConfigId);
        }
    }
}