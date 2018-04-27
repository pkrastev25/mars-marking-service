using System.Collections.Generic;
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

        public async Task<SimRunModel> GetSimRun(
            string simRunId,
            string projectId
        )
        {
            var response = await _httpService.GetAsync(
                $"http://sim-runner-svc/simrun?simRunId={simRunId}&projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get simRun with id: {simRunId}, projectId: {projectId} from sim-runner-svc! The response status code is {response.StatusCode}"
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
                $"http://sim-runner-svc/simrun?simPlanId={simPlanId}&projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get simRuns for simPlanId: {simPlanId}, projectId: {projectId} from sim-runner-svc! The response status code is {response.StatusCode}"
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
                $"http://sim-runner-svc/simrun?projectid={projectId}"
            );

            response.ThrowExceptionIfNotSuccessfulResponse(
                new FailedToGetResourceException(
                    $"Failed to get simRuns for projectId: {projectId} from sim-runner-svc! The response status code is {response.StatusCode}"
                )
            );

            if (response.IsEmptyResponse())
            {
                return new List<SimRunModel>();
            }

            return await response.Deserialize<List<SimRunModel>>();
        }

        public async Task<DependantResourceModel> CreateDependantSimRunResource(
            string simRunId,
            string projectId
        )
        {
            var simRun = await GetSimRun(simRunId, projectId);
            simRun.Id = simRunId;

            return await CreateDependantSimRunResource(simRun, projectId);
        }

        public async Task<DependantResourceModel> CreateDependantSimRunResource(
            SimRunModel simRunModel,
            string projectId
        )
        {
            return await Task.Run(() =>
            {
                if (simRunModel.Status == "Creating")
                {
                    // TODO: Consider stopping the sim run
                }

                if (simRunModel.Status != "Aborted" && simRunModel.Status != "Finished")
                {
                    throw new CannotMarkResourceException(
                        $"simRun with id: {simRunModel.Id} and projectId: {projectId} cannot be used, because it is state: {simRunModel.Status}! It must be Aborted or Finished beforehand!"
                    );
                }

                var markedResource = new DependantResourceModel("simRun", simRunModel.Id);
                _loggerService.LogSkipEvent(markedResource.ToString());

                return markedResource;
            });
        }
    }
}