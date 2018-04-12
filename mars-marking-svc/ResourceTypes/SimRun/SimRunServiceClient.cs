using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;
using Newtonsoft.Json;

namespace mars_marking_svc.ResourceTypes.SimRun
{
    public class SimRunClient : ISimRunClient
    {
        private readonly IHttpService _httpService;
        private readonly ILoggerService _loggerService;

        public SimRunClient(
            IHttpService httpService,
            ILoggerService loggerService
        )
        {
            _httpService = httpService;
            _loggerService = loggerService;
        }

        public async Task<SimRunModel> GetSimRun(string simRunId, string projectId)
        {
            var response =
                await _httpService.GetAsync($"http://sim-runner-svc/simrun?simRunId={simRunId}&projectid={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get simRun with id: {simRunId}, projectId: {projectId} from sim-runner-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<SimRunModel>>(jsonResponse)[0];
        }

        public async Task<List<SimRunModel>> GetSimRunsForSimPlan(string simPlanId, string projectId)
        {
            var response =
                await _httpService.GetAsync(
                    $"http://sim-runner-svc/simrun?simPlanId={simPlanId}&projectid={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get simRuns for simPlanId: {simPlanId}, projectId: {projectId} from sim-runner-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<SimRunModel>>(jsonResponse);
        }

        public async Task<List<SimRunModel>> GetSimRunsForProject(string projectId)
        {
            var response = await _httpService.GetAsync($"http://sim-runner-svc/simrun?projectid={projectId}");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToGetResourceException(
                    $"Failed to get simRuns for projectId: {projectId} from sim-runner-svc!"
                );
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<SimRunModel>>(jsonResponse);
        }

        public async Task<DependantResourceModel> StopSimRun(string simRunId, string projectId)
        {
            var simRun = await GetSimRun(simRunId, projectId);
            simRun.Id = simRunId;

            return await StopSimRun(simRun, projectId);
        }

        public async Task<DependantResourceModel> StopSimRun(SimRunModel simRunModel, string projectId)
        {
            var response = await _httpService.PutAsync(
                "http://sim-runner-svc/simrun",
                new SimRunUpdateModel
                {
                    SimRunId = simRunModel.Id,
                    Verb = SimRunUpdateModel.AbortVerb
                }
            );

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new FailedToUpdateResourceException(
                    $"Failed to update simRun with id: {simRunModel.Id}, projectId: {projectId} from sim-runner-svc!"
                );
            }

            var markedResource = new DependantResourceModel("simRun", simRunModel.Id);
            _loggerService.LogStopEvent(markedResource.ToString());

            return markedResource;
        }
    }
}