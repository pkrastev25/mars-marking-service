using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;
using mars_marking_svc.Utils;

namespace mars_marking_svc.ResourceTypes.SimRun
{
    public class SimRunClient : ISimRunClient
    {
        private readonly string _baseUrl;
        private readonly IHttpService _httpService;

        public SimRunClient(
            IHttpService httpService
        )
        {
            var baseUrl = Environment.GetEnvironmentVariable(Constants.Constants.SimRunnerSvcUrlKey);
            _baseUrl = string.IsNullOrEmpty(baseUrl) ? "sim-runner-svc" : baseUrl;
            _httpService = httpService;
        }

        public async Task<SimRunModel> GetSimRun(
            string simRunId,
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/simrun?simRunId={simRunId}&projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get simRun with id: {simRunId}, projectId: {projectId} from sim-runner-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            var simPlanModels = await response.Deserialize<List<SimRunModel>>();

            return simPlanModels[0];
        }

        public async Task<List<SimRunModel>> GetSimRunsForSimPlan(
            string simPlanId,
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/simrun?simPlanId={simPlanId}&projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToGetResourceException(
                    $"Failed to get simRuns for simPlanId: {simPlanId}, projectId: {projectId} from sim-runner-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            if (response.IsEmptyResponse())
            {
                return new List<SimRunModel>();
            }

            return await response.Deserialize<List<SimRunModel>>();
        }

        public async Task<List<SimRunModel>> GetSimRunsForProject(
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://{_baseUrl}/simrun?projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToGetResourceException(
                    $"Failed to get simRuns for projectId: {projectId} from sim-runner-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );

            if (response.IsEmptyResponse())
            {
                return new List<SimRunModel>();
            }

            return await response.Deserialize<List<SimRunModel>>();
        }

        public async Task<DependantResourceModel> MarkSimRun(
            string simRunId,
            string projectId
        )
        {
            var simRun = await GetSimRun(simRunId, projectId);
            simRun.Id = simRunId;

            return await MarkSimRun(simRun, projectId);
        }

        public async Task<DependantResourceModel> MarkSimRun(
            SimRunModel simRunModel,
            string projectId
        )
        {
            if (simRunModel.ToBeDeleted)
            {
                throw new ResourceAlreadyMarkedException(
                    $"Cannot mark simRun with id: {simRunModel.Id}, projectId: {projectId}, it is already marked!"
                );
            }

            var simRunMarkUpdateModel = new SimRunMarkUpdateModel(simRunModel.Id, true);

            var response = await _httpService.PutAsync(
                $"http://{_baseUrl}/simrun/marks",
                simRunMarkUpdateModel
            );

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return new DependantResourceModel(ResourceTypeEnum.SimRun, simRunModel.Id);
                case HttpStatusCode.Conflict:
                    throw new CannotMarkResourceException(
                        $"Cannot mark simRun with id: {simRunModel.Id}, projectId: {projectId} it must be in state: {SimRunModel.StatusFinished}, {SimRunModel.StatusAborted} or {SimRunModel.StatusFailed} beforehand!" +
                        await response.IncludeStatusCodeAndMessageFromResponse()
                    );
                default:
                    throw new FailedToUpdateResourceException(
                        $"Failed to update simRun with id: {simRunModel.Id}, projectId: {projectId} from sim-runner-svc!" +
                        await response.IncludeStatusCodeAndMessageFromResponse()
                    );
            }
        }

        public async Task UnmarkSimRun(DependantResourceModel dependantResourceModel)
        {
            var simRunMarkUpdateModel = new SimRunMarkUpdateModel(dependantResourceModel.ResourceId, false);

            var response = await _httpService.PutAsync(
                $"http://{_baseUrl}/simrun/marks",
                simRunMarkUpdateModel
            );

            response.ThrowExceptionIfNotSuccessfulResponseOrNot404Response(
                new FailedToUpdateResourceException(
                    $"Failed to update simRun with id: {dependantResourceModel.ResourceId} from sim-runner-svc!" +
                    await response.IncludeStatusCodeAndMessageFromResponse()
                )
            );
        }
    }
}