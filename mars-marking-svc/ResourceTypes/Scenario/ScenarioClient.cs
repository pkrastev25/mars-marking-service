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
        private readonly IHttpService _httpService;
        private readonly ILoggerService _loggerService;

        public ScenarioClient(
            IHttpService httpService,
            ILoggerService loggerService
        )
        {
            _httpService = httpService;
            _loggerService = loggerService;
        }

        public async Task<ScenarioModel> GetScenario(
            string scenarioId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://scenario-svc/scenarios/{scenarioId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get scenario with id: {scenarioId} from scenario-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            return await response.Deserialize<ScenarioModel>();
        }

        public async Task<List<ScenarioModel>> GetScenariosForMetadata(
            string metadataId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://scenario-svc/scenarios?DataId={metadataId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get scenarios for metadataId: {metadataId} from scenario-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
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
                $"http://scenario-svc/scenarios?Project={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get scenarios for projectId: {projectId} from scenario-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
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

            var response = await _httpService.PatchAsync(
                $"http://scenario-svc/scenarios/{scenarioModel.ScenarioId}/metadata",
                scenarioModel
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToUpdateResourceException(
                    $"Failed to update scenario {scenarioModel} from scenario-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            var markedResource = new DependantResourceModel(ResourceTypeEnum.Scenario, scenarioModel.ScenarioId);
            _loggerService.LogMarkEvent(markedResource.ToString());

            return markedResource;
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

            var response = await _httpService.PatchAsync(
                $"http://scenario-svc/scenarios/{scenarioModel.ScenarioId}/metadata",
                scenarioModel
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToUpdateResourceException(
                    $"Failed to update {dependantResourceModel} from scenario-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            _loggerService.LogUnmarkEvent(dependantResourceModel.ToString());
        }
    }
}