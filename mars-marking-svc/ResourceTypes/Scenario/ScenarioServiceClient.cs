using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Models;
using mars_marking_svc.Services.Models;
using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.Scenario
{
    public class ScenarioServiceClient : IScenarioServiceClient
    {
        private readonly IHttpService _httpService;
        private readonly ILoggerService _loggerService;

        public ScenarioServiceClient(
            IHttpService httpService,
            ILoggerService loggerService
        )
        {
            _httpService = httpService;
            _loggerService = loggerService;
        }

        public async Task<ScenarioModel> GetScenario(string scenarioId)
        {
            var response = await _httpService.GetAsync($"http://scenario-svc/scenarios/{scenarioId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get scenario with id: {scenarioId} from scenario-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ScenarioModel>(jsonResponse);
        }

        public async Task<List<ScenarioModel>> GetScenariosForMetadata(string metadataId)
        {
            var response = await _httpService.GetAsync($"http://scenario-svc/scenarios?DataId={metadataId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get scenarios for metadataId: {metadataId} from scenario-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<ScenarioModel>>(jsonResponse);
        }

        public async Task<List<ScenarioModel>> GetScenariosForProject(string projectId)
        {
            var response = await _httpService.GetAsync($"http://scenario-svc/scenarios?Project={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get scenarios for projectId: {projectId} from scenario-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<ScenarioModel>>(jsonResponse);
        }

        public async Task<MarkedResourceModel> MarkScenario(string scenarioId)
        {
            var scenario = await GetScenario(scenarioId);
            scenario.ScenarioId = scenarioId;

            return await MarkScenario(scenario);
        }

        public async Task<MarkedResourceModel> MarkScenario(ScenarioModel scenarioModel)
        {
            if (scenarioModel.ToBeDeleted)
            {
                throw new ResourceAlreadyMarkedException(
                    $"Cannot mark {scenarioModel}, it is already marked!"
                );
            }

            scenarioModel.ToBeDeleted = true;

            var response = await _httpService.PatchAsync(
                $"http://scenario-svc/scenarios/{scenarioModel.ScenarioId}/metadata",
                scenarioModel
            );

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new FailedToUpdateResourceException(
                    $"Failed to update scenario {scenarioModel} from scenario-svc!"
                );
            }

            var markedResource = new MarkedResourceModel("scenario", scenarioModel.ScenarioId);
            _loggerService.LogMarkEvent(markedResource.ToString());

            return markedResource;
        }

        public async Task UnmarkScenario(MarkedResourceModel markedResourceModel)
        {
            if (!await DoesScenarioExist(markedResourceModel.ResourceId))
            {
                _loggerService.LogSkipEvent(markedResourceModel.ToString());
                return;
            }

            var scenarioModel = new ScenarioModel
            {
                ScenarioId = markedResourceModel.ResourceId,
                ToBeDeleted = false
            };

            var response = await _httpService.PatchAsync(
                $"http://scenario-svc/scenarios/{scenarioModel.ScenarioId}/metadata",
                scenarioModel
            );

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new FailedToUpdateResourceException(
                    $"Failed to update {markedResourceModel} from scenario-svc!"
                );
            }

            _loggerService.LogUnmarkEvent(markedResourceModel.ToString());
        }

        private async Task<bool> DoesScenarioExist(string scenarioId)
        {
            var response = await _httpService.GetAsync($"http://scenario-svc/scenarios/{scenarioId}");

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return true;
                case HttpStatusCode.NotFound:
                    return false;
                default:
                    throw new FailedToGetResourceException(
                        $"Failed to get scenario with id: {scenarioId} from scenario-svc!"
                    );
            }
        }
    }
}