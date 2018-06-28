using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Models;
using mars_marking_svc.Services.Models;
using mars_marking_svc.Utils;

namespace mars_marking_svc.ResourceTypes.Scenario
{
    public class ScenarioClient : IScenarioClient
    {
        private readonly string _baseUrl;
        private readonly IHttpService _httpService;

        public ScenarioClient(
            IHttpService httpService
        )
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.Constants.ScenarioSvcUrlKey);
            _baseUrl = string.IsNullOrEmpty(baseUrl) ? "scenario-svc" : baseUrl;
            _httpService = httpService;
        }

        public async Task<ScenarioModel> GetScenario(
            string scenarioId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/scenarios/{scenarioId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    await response.FormatRequestAndResponse(
                        $"Failed to get scenario with id: {scenarioId} from scenario-svc!"
                    )
                )
            );

            return await response.Deserialize<ScenarioModel>();
        }

        public async Task<List<ScenarioModel>> GetScenariosForMetadata(
            string metadataId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/scenarios?DataId={metadataId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToGetResourceException(
                    await response.FormatRequestAndResponse(
                        $"Failed to get scenarios for metadataId: {metadataId} from scenario-svc!"
                    )
                )
            );

            if (response.IsEmptyResponse())
            {
                return new List<ScenarioModel>();
            }

            return await response.Deserialize<List<ScenarioModel>>();
        }

        public async Task<List<ScenarioModel>> GetScenariosForProject(
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/scenarios?Project={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToGetResourceException(
                    await response.FormatRequestAndResponse(
                        $"Failed to get scenarios for projectId: {projectId} from scenario-svc!"
                    )
                )
            );

            if (response.IsEmptyResponse())
            {
                return new List<ScenarioModel>();
            }

            return await response.Deserialize<List<ScenarioModel>>();
        }

        public async Task<DependantResourceModel> MarkScenario(
            string scenarioId
        )
        {
            var scenario = await GetScenario(scenarioId);
            scenario.ScenarioId = scenarioId;

            return await MarkScenario(scenario);
        }

        public async Task<DependantResourceModel> MarkScenario(
            ScenarioModel scenarioModel
        )
        {
            if (scenarioModel.ToBeDeleted)
            {
                throw new ResourceAlreadyMarkedException(
                    $"Cannot mark {scenarioModel}, it is already marked!"
                );
            }

            scenarioModel.ToBeDeleted = true;
            scenarioModel.ReadOnly = true;

            var response = await _httpService.PutAsync(
                $"http://{_baseUrl}/scenarios/{scenarioModel.ScenarioId}/marks",
                scenarioModel
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToUpdateResourceException(
                    await response.FormatRequestAndResponse(
                        $"Failed to update scenario {scenarioModel} from scenario-svc!"
                    )
                )
            );

            return new DependantResourceModel(
                ResourceTypeEnum.Scenario,
                scenarioModel.ScenarioId
            );
        }

        public async Task UnmarkScenario(
            DependantResourceModel dependantResourceModel
        )
        {
            var scenarioModel = new ScenarioModel
            {
                ScenarioId = dependantResourceModel.ResourceId,
                ToBeDeleted = false,
                ReadOnly = false
            };

            var response = await _httpService.PutAsync(
                $"http://{_baseUrl}/scenarios/{scenarioModel.ScenarioId}/marks",
                scenarioModel
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToUpdateResourceException(
                    await response.FormatRequestAndResponse(
                        $"Failed to update {dependantResourceModel} from scenario-svc!"
                    )
                )
            );
        }
    }
}