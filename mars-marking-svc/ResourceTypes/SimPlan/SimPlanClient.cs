using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Models;
using mars_marking_svc.Services.Models;
using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.SimPlan
{
    public class SimPlanClient : ISimPlanClient
    {
        private readonly IHttpService _httpService;
        private readonly ILoggerService _loggerService;

        public SimPlanClient(
            IHttpService httpService,
            ILoggerService loggerService
        )
        {
            _httpService = httpService;
            _loggerService = loggerService;
        }

        public async Task<SimPlanModel> GetSimPlan(string simPlanId, string projectId)
        {
            var response =
                await _httpService.GetAsync($"http://sim-runner-svc/simplan?id={simPlanId}&projectid={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get simPlan with id: {simPlanId}, projectId: {projectId} from sim-runner-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            // TODO: Potentially fix this in the sim-runner-svc!
            return JsonConvert.DeserializeObject<List<SimPlanModel>>(jsonResponse)[0];
        }

        public async Task<List<SimPlanModel>> GetSimPlansForScenario(string scenarioId, string projectId)
        {
            var response =
                await _httpService.GetAsync(
                    $"http://sim-runner-svc/simplan?scenarioid={scenarioId}&projectid={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get simPlans for scenarioId: {scenarioId}, projectId: {projectId} from sim-runner-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<SimPlanModel>>(jsonResponse);
        }

        public async Task<List<SimPlanModel>> GetSimPlansForResultConfig(string resultConfigId, string projectId)
        {
            var response =
                await _httpService.GetAsync(
                    $"http://sim-runner-svc/simplan?resultconfigid={resultConfigId}&projectid={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get simPlans for resultConfigId: {resultConfigId}, projectId: {projectId} from sim-runner-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<SimPlanModel>>(jsonResponse);
        }

        public async Task<List<SimPlanModel>> GetSimPlansForProject(string projectId)
        {
            var response = await _httpService.GetAsync($"http://sim-runner-svc/simplan?projectid={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get simPlans for projectId: {projectId} from sim-runner-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<SimPlanModel>>(jsonResponse);
        }

        public async Task<MarkedResourceModel> MarkSimPlan(string simPlanId, string projectId)
        {
            var simPlan = await GetSimPlan(simPlanId, projectId);
            simPlan.Id = simPlanId;

            return await MarkSimPlan(simPlan, projectId);
        }

        public async Task<MarkedResourceModel> MarkSimPlan(SimPlanModel simPlanModel, string projectId)
        {
            if (simPlanModel.ToBeDeleted)
            {
                throw new ResourceAlreadyMarkedException(
                    $"Cannot mark simPlan with id: {simPlanModel.Id}, projectId: {projectId}, it is already marked!"
                );
            }

            simPlanModel.ToBeDeleted = true;

            var response = await _httpService.PutAsync(
                $"http://sim-runner-svc/simplan?id={simPlanModel.Id}&projectid={projectId}",
                simPlanModel
            );

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToUpdateResourceException(
                    $"Failed to update simPlan with id: {simPlanModel.Id}, projectId: {projectId} from sim-runner-svc!"
                );
            }

            var markedResource = new MarkedResourceModel("simPlan", simPlanModel.Id);
            _loggerService.LogMarkEvent(markedResource.ToString());

            return markedResource;
        }

        public async Task UnmarkSimPlan(MarkedResourceModel markedResourceModel, string projectId)
        {
            if (!await DoesSimPlanExist(markedResourceModel.ResourceId, projectId))
            {
                _loggerService.LogSkipEvent(markedResourceModel.ToString());
                return;
            }

            var simPlanModel = await GetSimPlan(markedResourceModel.ResourceId, projectId);
            simPlanModel.ToBeDeleted = false;

            var response = await _httpService.PutAsync(
                $"http://sim-runner-svc/simplan?id={simPlanModel.Id}&projectid={projectId}",
                simPlanModel
            );

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToUpdateResourceException(
                    $"Failed to update simPlan with id: {simPlanModel.Id}, projectId: {projectId} from sim-runner-svc!"
                );
            }

            _loggerService.LogUnmarkEvent(markedResourceModel.ToString());
        }

        private async Task<bool> DoesSimPlanExist(string simPlanId, string projectId)
        {
            var response =
                await _httpService.GetAsync($"http://sim-runner-svc/simplan?id={simPlanId}&projectid={projectId}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            throw new FailedToGetResourceException(
                $"Failed to get simPlan with id: {simPlanId}, projectId: {projectId} from sim-runner-svc!"
            );
        }
    }
}