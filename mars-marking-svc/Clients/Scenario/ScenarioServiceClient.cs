using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Models;
using mars_marking_svc.Services;
using Newtonsoft.Json;

namespace mars_marking_svc.Clients.Scenario
{
    public class ScenarioServiceClient : IScenarioServiceClient
    {
        private readonly IHttpService _httpService;

        public ScenarioServiceClient(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<ScenarioModel> GetScenario(string scenarioId)
        {
            var response = await _httpService.GetAsync($"http://localhost:8080/scenarios/{scenarioId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // TODO: Revert the process
                throw new Exception();
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
                // TODO: revert the process
                throw new Exception();
            }

            scenarioModel.ToBeDeleted = true;

            var response = await _httpService.PatchAsync(
                $"http://localhost:8080/scenarios/{scenarioModel.ScenarioId}/metadata",
                scenarioModel
            );

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new MarkedResourceModel
                {
                    resourceType = "scenario",
                    resourceId = scenarioModel.ScenarioId
                };
            }

            // TODO: Revert the process
            throw new Exception();
        }

        public async Task<List<ScenarioModel>> GetScenariosForMetadata(string metadataId)
        {
            var response = await _httpService.GetAsync($"http://localhost:8080/scenarios?DataId={metadataId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // TODO: Revert the process
                throw new Exception();
            }
            
            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<ScenarioModel>>(jsonResponse);
        }

        public async Task<List<ScenarioModel>> GetScenariosForProject(string projectId)
        {
            var response = await _httpService.GetAsync($"http://localhost:8080/scenarios?Project={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // TODO: Revert the process
                throw new Exception();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<ScenarioModel>>(jsonResponse);
        }
    }
}