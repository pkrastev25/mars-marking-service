using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Models;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.ResourceTypes.Metadata
{
    public class MetadataResourceHandler : IMetadataResourceHandler
    {
        private readonly IMetadataClient _metadataClient;
        private readonly IScenarioClient _scenarioClient;
        private readonly IResultConfigClient _resultConfigClient;
        private readonly ISimPlanClient _simPlanClient;
        private readonly ISimRunClient _simRunClient;
        private readonly IResultDataClient _resultDataClient;
        private readonly IMarkSessionRepository _markSessionRepository;

        public MetadataResourceHandler(
            IMetadataClient metadataClient,
            IScenarioClient scenarioClient,
            IResultConfigClient resultConfigClient,
            ISimPlanClient simPlanClient,
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            IMarkSessionRepository markSessionRepository
        )
        {
            _metadataClient = metadataClient;
            _scenarioClient = scenarioClient;
            _resultConfigClient = resultConfigClient;
            _simPlanClient = simPlanClient;
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _markSessionRepository = markSessionRepository;
        }

        public async Task<MarkSessionModel> GatherResourcesForMarkSession(MarkSessionModel markSessionModel)
        {
            var metadataId = markSessionModel.ResourceId;
            var projectId = markSessionModel.ProjectId;

            var markedSourceMetadata = await _metadataClient.MarkMetadata(metadataId);
            markSessionModel.DependantResources.Add(markedSourceMetadata);
            await _markSessionRepository.Update(markSessionModel);

            var scenariosForMetadata = await _scenarioClient.GetScenariosForMetadata(metadataId);
            foreach (var scenarioModel in scenariosForMetadata)
            {
                var markedScenario = await _scenarioClient.MarkScenario(scenarioModel);
                markSessionModel.DependantResources.Add(markedScenario);
                await _markSessionRepository.Update(markSessionModel);
            }

            var resultConfigsForMetadata = await _resultConfigClient.GetResultConfigsForMetadata(metadataId);
            foreach (var resultConfigModel in resultConfigsForMetadata)
            {
                // ResultConfigs obey the metadata mark!
                var markedResultConfig =
                    await _resultConfigClient.CreateMarkedResultConfig(resultConfigModel);
                markSessionModel.DependantResources.Add(markedResultConfig);
                await _markSessionRepository.Update(markSessionModel);
            }

            var simPlansForScenarios = new List<SimPlanModel>();
            foreach (var scenarioModel in scenariosForMetadata)
            {
                simPlansForScenarios.AddRange(
                    await _simPlanClient.GetSimPlansForScenario(scenarioModel.ScenarioId, projectId)
                );
            }
            foreach (var simPlanModel in simPlansForScenarios)
            {
                var markedSimPlan = await _simPlanClient.MarkSimPlan(simPlanModel, projectId);
                markSessionModel.DependantResources.Add(markedSimPlan);
                await _markSessionRepository.Update(markSessionModel);
            }

            var simRunsForSimPlans = new List<SimRunModel>();
            foreach (var simPlanModel in simPlansForScenarios)
            {
                simRunsForSimPlans.AddRange(
                    await _simRunClient.GetSimRunsForSimPlan(simPlanModel.Id, projectId)
                );
            }
            foreach (var simRunModel in simRunsForSimPlans)
            {
                var stoppedSimRun = await _simRunClient.StopSimRun(simRunModel, projectId);
                markSessionModel.DependantResources.Add(stoppedSimRun);
                await _markSessionRepository.Update(markSessionModel);
            }

            foreach (var simRunModel in simRunsForSimPlans)
            {
                var resultData = await _resultDataClient.CreateMarkedResultData(simRunModel);
                markSessionModel.DependantResources.Add(resultData);
                await _markSessionRepository.Update(markSessionModel);
            }

            markSessionModel.State = MarkSessionModel.DoneState;
            await _markSessionRepository.Update(markSessionModel);

            return markSessionModel;
        }
    }
}