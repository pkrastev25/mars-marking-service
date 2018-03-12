using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Models;
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
            var response = await _httpService.GetAsync($"http://localhost:8080/scenarios/{scenarioId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get scenario resource with id: {scenarioId} from scenario-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ScenarioModel>(jsonResponse);
        }

        public async Task<MarkedResourceModel> MarkScenario(string scenarioId)
        {
            var scenario = GetScenario(scenarioId).Result;
            scenario.ScenarioId = scenarioId;

            return await MarkScenario(scenario);
        }

        public async Task<MarkedResourceModel> MarkScenario(ScenarioModel scenarioModel)
        {
            if (scenarioModel.ToBeDeleted)
            {
                throw new ResourceAlreadyMarkedException(
                    $"Cannot mark scenario resource with id: {scenarioModel.ScenarioId}, it is already marked!"
                );
            }

            scenarioModel.ToBeDeleted = true;

            var response = await _httpService.PatchAsync(
                $"http://localhost:8080/scenarios/{scenarioModel.ScenarioId}/metadata",
                scenarioModel
            );

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                throw new FailedToUpdateResourceException(
                    $"Failed to update scenario resource with id: {scenarioModel.ScenarioId} from scenario-svc!"
                );
            }

            var markedResource = new MarkedResourceModel
            {
                resourceType = "scenario",
                resourceId = scenarioModel.ScenarioId
            };
            _loggerService.LogMarkedResource(markedResource);

            return markedResource;
        }

        public async Task<List<ScenarioModel>> GetScenariosForMetadata(string metadataId)
        {
            var response = await _httpService.GetAsync($"http://localhost:8080/scenarios?DataId={metadataId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get scenario resources for metadataId: {metadataId} from scenario-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<ScenarioModel>>(jsonResponse);
        }

        public async Task<List<ScenarioModel>> GetScenariosForProject(string projectId)
        {
            var response = await _httpService.GetAsync($"http://localhost:8080/scenarios?Project={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get scenario resources for projectId: {projectId} from scenario-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<ScenarioModel>>(jsonResponse);
        }
    }
}