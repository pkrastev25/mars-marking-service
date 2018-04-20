using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Models;
using mars_marking_svc.Services.Models;
using mars_marking_svc.Utils;

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
            var response = await _httpService.GetAsync(
                $"http://sim-runner-svc/simplan?id={simPlanId}&projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get simPlan with id: {simPlanId}, projectId: {projectId} from sim-runner-svc! The response status code is {response.StatusCode}"
                )
            );

            // TODO: Potentially fix this in the sim-runner-svc!
            var simPlanModels = await response.Deserialize<List<SimPlanModel>>();

            return simPlanModels[0];
        }

        public async Task<List<SimPlanModel>> GetSimPlansForScenario(string scenarioId, string projectId)
        {
            var response = await _httpService.GetAsync(
                $"http://sim-runner-svc/simplan?scenarioid={scenarioId}&projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get simPlans for scenarioId: {scenarioId}, projectId: {projectId} from sim-runner-svc! The response status code is {response.StatusCode}"
                )
            );

            return await response.Deserialize<List<SimPlanModel>>();
        }

        public async Task<List<SimPlanModel>> GetSimPlansForResultConfig(string resultConfigId, string projectId)
        {
            var response = await _httpService.GetAsync(
                $"http://sim-runner-svc/simplan?resultconfigid={resultConfigId}&projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get simPlans for resultConfigId: {resultConfigId}, projectId: {projectId} from sim-runner-svc! The response status code is {response.StatusCode}"
                )
            );

            return await response.Deserialize<List<SimPlanModel>>();
        }

        public async Task<List<SimPlanModel>> GetSimPlansForProject(string projectId)
        {
            var response = await _httpService.GetAsync(
                $"http://sim-runner-svc/simplan?projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get simPlans for projectId: {projectId} from sim-runner-svc! The response status code is {response.StatusCode}"
                )
            );

            return await response.Deserialize<List<SimPlanModel>>();
        }

        public async Task<DependantResourceModel> MarkSimPlan(string simPlanId, string projectId)
        {
            var simPlan = await GetSimPlan(simPlanId, projectId);
            simPlan.Id = simPlanId;

            return await MarkSimPlan(simPlan, projectId);
        }

        public async Task<DependantResourceModel> MarkSimPlan(SimPlanModel simPlanModel, string projectId)
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

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToUpdateResourceException(
                    $"Failed to update simPlan with id: {simPlanModel.Id}, projectId: {projectId} from sim-runner-svc! The response status code is {response.StatusCode}"
                )
            );

            var markedResource = new DependantResourceModel("simPlan", simPlanModel.Id);
            _loggerService.LogMarkEvent(markedResource.ToString());

            return markedResource;
        }

        public async Task UnmarkSimPlan(DependantResourceModel dependantResourceModel, string projectId)
        {
            if (!await DoesSimPlanExist(dependantResourceModel.ResourceId, projectId))
            {
                _loggerService.LogSkipEvent(dependantResourceModel.ToString());
                return;
            }

            var simPlanModel = await GetSimPlan(dependantResourceModel.ResourceId, projectId);
            simPlanModel.ToBeDeleted = false;

            var response = await _httpService.PutAsync(
                $"http://sim-runner-svc/simplan?id={simPlanModel.Id}&projectid={projectId}",
                simPlanModel
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToUpdateResourceException(
                    $"Failed to update simPlan with id: {simPlanModel.Id}, projectId: {projectId} from sim-runner-svc! The response status code is {response.StatusCode}"
                )
            );

            _loggerService.LogUnmarkEvent(dependantResourceModel.ToString());
        }

        private async Task<bool> DoesSimPlanExist(string simPlanId, string projectId)
        {
            var response = await _httpService.GetAsync(
                $"http://sim-runner-svc/simplan?id={simPlanId}&projectid={projectId}"
            );

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return true;
                case HttpStatusCode.NoContent:
                    return false;
                default:
                    throw new FailedToGetResourceException(
                        $"Failed to get simPlan with id: {simPlanId}, projectId: {projectId} from sim-runner-svc! The response status code is {response.StatusCode}"
                    );
            }
        }
    }
}