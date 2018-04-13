using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.ResourceTypes.SimRun
{
    public class SimRunResourceHandler : ISimRunResourceHandler
    {
        private readonly ISimRunClient _simRunClient;
        private readonly IResultDataClient _resultDataClient;
        private readonly IMarkSessionRepository _markSessionRepository;

        public SimRunResourceHandler(
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            IMarkSessionRepository markSessionRepository
        )
        {
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _markSessionRepository = markSessionRepository;
        }

        public async Task<MarkSessionModel> GatherResourcesForMarkSession(MarkSessionModel markSessionModel)
        {
            var simRunId = markSessionModel.ResourceId;
            var projectId = markSessionModel.ProjectId;
            var sourceSimRun = await _simRunClient.GetSimRun(simRunId, projectId);

            if (sourceSimRun.Status == "Running")
            {
                throw new CannotMarkResourceException(
                    $"simRun with id: {simRunId} and projectId: {projectId} cannot be used, because it is still running!"
                );
            }

            var markedSourceSimRun = await _simRunClient.StopSimRun(simRunId, projectId);
            markSessionModel.DependantResources.Add(markedSourceSimRun);
            await _markSessionRepository.Update(markSessionModel);

            var markedResultData = await _resultDataClient.CreateMarkedResultData(sourceSimRun);
            markSessionModel.DependantResources.Add(markedResultData);
            await _markSessionRepository.Update(markSessionModel);

            return markSessionModel;
        }
    }
}