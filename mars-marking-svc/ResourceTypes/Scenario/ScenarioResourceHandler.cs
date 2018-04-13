using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.ResourceTypes.Scenario
{
    public class ScenarioResourceHandler : IScenarioResourceHandler
    {
        private readonly IMarkSessionRepository _markSessionRepository;
        private readonly IScenarioClient _scenarioClient;
        private readonly ISimPlanClient _simPlanClient;
        private readonly ISimRunClient _simRunClient;
        private readonly IResultDataClient _resultDataClient;

        public ScenarioResourceHandler(
            IScenarioClient scenarioClient,
            ISimPlanClient simPlanClient,
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            IMarkSessionRepository markSessionRepository
        )
        {
            _scenarioClient = scenarioClient;
            _simPlanClient = simPlanClient;
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _markSessionRepository = markSessionRepository;
        }

        public async Task<MarkSessionModel> GatherResourcesForMarkSession(MarkSessionModel markSessionModel)
        {
            var scenarioId = markSessionModel.ResourceId;
            var projectId = markSessionModel.ProjectId;

            var markedSourceScenario = await _scenarioClient.MarkScenario(scenarioId);
            markSessionModel.DependantResources.Add(markedSourceScenario);
            await _markSessionRepository.Update(markSessionModel);

            var simPlansForScenario = await _simPlanClient.GetSimPlansForScenario(scenarioId, projectId);
            foreach (var simPlanModel in simPlansForScenario)
            {
                var markedSimPlan = await _simPlanClient.MarkSimPlan(simPlanModel, projectId);
                markSessionModel.DependantResources.Add(markedSimPlan);
                await _markSessionRepository.Update(markSessionModel);
            }

            var simRunsForSimPlans = new List<SimRunModel>();
            foreach (var simPlanModel in simPlansForScenario)
            {
                simRunsForSimPlans.AddRange(
                    await _simRunClient.GetSimRunsForSimPlan(simPlanModel.Id, projectId)
                );
            }
            foreach (var simRunModel in simRunsForSimPlans)
            {
                var markedSimRun = await _simRunClient.StopSimRun(simRunModel.Id, projectId);
                markSessionModel.DependantResources.Add(markedSimRun);
                await _markSessionRepository.Update(markSessionModel);
            }

            foreach (var simRunModel in simRunsForSimPlans)
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