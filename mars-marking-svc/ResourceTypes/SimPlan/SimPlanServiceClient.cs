using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Models;
using mars_marking_svc.ResourceTypes.SimPlan.Models;
using mars_marking_svc.Services.Models;
using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.SimPlan
{
    public class SimPlanServiceClient : ISimPlanServiceClient
    {
        private readonly IHttpService _httpService;
        private readonly ILoggerService _loggerService;

        public SimPlanServiceClient(
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
                    $"Failed to get simPlan resource with id: {simPlanId} and projectId: {projectId} from sim-runner-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SimPlanModel>(jsonResponse);
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
                    $"Cannot mark simPlan resource with id: {simPlanModel.Id} and projectId: {projectId}, it is already marked!"
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
                    $"Failed to update simPlan resource with id: {simPlanModel.Id} and projectId: {projectId} from sim-runner-svc!"
                );
            }

            var markedResource = new MarkedResourceModel
            {
                resourceType = "simPlan",
                resourceId = simPlanModel.Id
            };
            _loggerService.LogMarkedResource(markedResource);

            return markedResource;
        }

        public async Task<List<SimPlanModel>> GetSimPlansForProject(string projectId)
        {
            var response = await _httpService.GetAsync($"http://sim-runner-svc/simplan?projectid={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get simPlan resources for projectId: {projectId} from sim-runner-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<SimPlanModel>>(jsonResponse);
        }

        public async Task<List<SimPlanModel>> GetSimPlansForScenario(string scenarioId, string projectId)
        {
            var response =
                await _httpService.GetAsync(
                    $"http://sim-runner-svc/simplan?scenarioid={scenarioId}&projectid={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get simPlan resources for scenarioId: {scenarioId} and projectId: {projectId} from sim-runner-svc!"
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
                    $"Failed to get simPlan resources for resultConfigId: {resultConfigId} and projectId: {projectId} from sim-runner-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<SimPlanModel>>(jsonResponse);
        }
    }
}