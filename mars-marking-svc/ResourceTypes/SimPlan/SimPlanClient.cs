using System;
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
        private readonly string _baseUrl;
        private readonly IHttpService _httpService;

        public SimPlanClient(
            IHttpService httpService
        )
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.Constants.SimRunnerSvcUrlKey);
            _baseUrl = string.IsNullOrEmpty(baseUrl) ? "sim-runner-svc" : baseUrl;
            _httpService = httpService;
        }

        public async Task<SimPlanModel> GetSimPlan(
            string simPlanId,
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/simplan?id={simPlanId}&projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get simPlan with id: {simPlanId}, projectId: {projectId} from sim-runner-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            var simPlanModels = await response.Deserialize<List<SimPlanModel>>();

            return simPlanModels[0];
        }

        public async Task<List<SimPlanModel>> GetSimPlansForScenario(
            string scenarioId,
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/simplan?scenarioid={scenarioId}&projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToGetResourceException(
                    $"Failed to get simPlans for scenarioId: {scenarioId}, projectId: {projectId} from sim-runner-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            if (response.IsEmptyResponse())
            {
                return new List<SimPlanModel>();
            }

            return await response.Deserialize<List<SimPlanModel>>();
        }

        public async Task<List<SimPlanModel>> GetSimPlansForResultConfig(
            string resultConfigId,
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/simplan?resultconfigid={resultConfigId}&projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToGetResourceException(
                    $"Failed to get simPlans for resultConfigId: {resultConfigId}, projectId: {projectId} from sim-runner-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            if (response.IsEmptyResponse())
            {
                return new List<SimPlanModel>();
            }

            return await response.Deserialize<List<SimPlanModel>>();
        }

        public async Task<List<SimPlanModel>> GetSimPlansForProject(
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/simplan?projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToGetResourceException(
                    $"Failed to get simPlans for projectId: {projectId} from sim-runner-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            if (response.IsEmptyResponse())
            {
                return new List<SimPlanModel>();
            }

            return await response.Deserialize<List<SimPlanModel>>();
        }

        public async Task<DependantResourceModel> MarkSimPlan(
            string simPlanId,
            string projectId
        )
        {
            var simPlan = await GetSimPlan(simPlanId, projectId);
            simPlan.Id = simPlanId;

            return await MarkSimPlan(simPlan, projectId);
        }

        public async Task<DependantResourceModel> MarkSimPlan(
            SimPlanModel simPlanModel,
            string projectId
        )
        {
            if (simPlanModel.ToBeDeleted)
            {
                throw new ResourceAlreadyMarkedException(
                    $"Cannot mark simPlan with id: {simPlanModel.Id}, projectId: {projectId}, it is already marked!"
                );
            }

            simPlanModel.ToBeDeleted = true;

            var response = await _httpService.PutAsync(
                $"http://{_baseUrl}/simplan?id={simPlanModel.Id}&projectid={projectId}",
                simPlanModel
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToUpdateResourceException(
                    $"Failed to update simPlan with id: {simPlanModel.Id}, projectId: {projectId} from sim-runner-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            return new DependantResourceModel(
                ResourceTypeEnum.SimPlan,
                simPlanModel.Id
            );
        }

        public async Task UnmarkSimPlan(
            DependantResourceModel dependantResourceModel,
            string projectId
        )
        {
            if (await DoesSimPlanExist(dependantResourceModel.ResourceId, projectId))
            {
                var simPlanModel = await GetSimPlan(dependantResourceModel.ResourceId, projectId);
                simPlanModel.ToBeDeleted = false;

                var response = await _httpService.PutAsync(
                    $"http://{_baseUrl}/simplan?id={simPlanModel.Id}&projectid={projectId}",
                    simPlanModel
                );

                response.ThrowExceptionIfNotSuccessfulResponse(
                    new FailedToUpdateResourceException(
                        $"Failed to update simPlan with id: {simPlanModel.Id}, projectId: {projectId} from sim-runner-svc!" +
                        await response.IncludeStatusCodeAndMessageFromResponse()
                    )
                );
            }
        }

        private async Task<bool> DoesSimPlanExist(
            string simPlanId,
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/simplan?id={simPlanId}&projectid={projectId}"
            );

            if (response.IsSuccessStatusCode)
            {
                return response.StatusCode != HttpStatusCode.NoContent;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }

            throw new FailedToGetResourceException(
                $"Failed to get simPlan with id: {simPlanId}, projectId: {projectId} from sim-runner-svc!" +
                await response.IncludeStatusCodeAndMessageFromResponse()
            );
        }
    }
}