using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.ResourceTypes.SimPlan
{
    public class SimPlanResourceHandler : ISimPlanResourceHandler
    {
        private readonly ISimPlanClient _simPlanClient;
        private readonly ISimRunClient _simRunClient;
        private readonly IResultDataClient _resultDataClient;
        private readonly IMarkSessionRepository _markSessionRepository;

        public SimPlanResourceHandler(
            ISimPlanClient simPlanClient,
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            IMarkSessionRepository markSessionRepository
        )
        {
            _simPlanClient = simPlanClient;
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _markSessionRepository = markSessionRepository;
        }

        public async Task<MarkSessionModel> GatherResourcesForMarkSession(MarkSessionModel markSessionModel)
        {
            var simPlanId = markSessionModel.ResourceId;
            var projectId = markSessionModel.ProjectId;

            var markedSourceSimPlan = await _simPlanClient.MarkSimPlan(simPlanId, projectId);
            markSessionModel.DependantResources.Add(markedSourceSimPlan);
            await _markSessionRepository.Update(markSessionModel);

            var simRunsForSimPlan = await _simRunClient.GetSimRunsForSimPlan(simPlanId, projectId);
            foreach (var simRunModel in simRunsForSimPlan)
            {
                var markedSimRun = await _simRunClient.StopSimRun(simRunModel, projectId);
                markSessionModel.DependantResources.Add(markedSimRun);
                await _markSessionRepository.Update(markSessionModel);
            }

            foreach (var simRunModel in simRunsForSimPlan)
            {
                var markedResultData = await _resultDataClient.CreateMarkedResultData(simRunModel);
                markSessionModel.DependantResources.Add(markedResultData);
                await _markSessionRepository.Update(markSessionModel);
            }

            markSessionModel.State = MarkSessionModel.DoneState;
            await _markSessionRepository.Update(markSessionModel);

            return markSessionModel;
        }
    }
}