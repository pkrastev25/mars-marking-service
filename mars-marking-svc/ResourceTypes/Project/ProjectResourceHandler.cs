using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.ProjectContents.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.ResourceTypes.ProjectContents
{
    public class ProjectResourceHandler : IProjectResourceHandler
    {
        private readonly IMetadataClient _metadataClient;
        private readonly IScenarioClient _scenarioClient;
        private readonly IResultConfigClient _resultConfigClient;
        private readonly ISimPlanClient _simPlanClient;
        private readonly ISimRunClient _simRunClient;
        private readonly IResultDataClient _resultDataClient;
        private readonly IMarkSessionRepository _markSessionRepository;

        public ProjectResourceHandler(
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
            var projectId = markSessionModel.ProjectId;

            var metadataForProject = await _metadataClient.GetMetadataForProject(projectId);
            foreach (var metadataModel in metadataForProject)
            {
                var markedMetadata = await _metadataClient.MarkMetadata(metadataModel);
                markSessionModel.DependantResources.Add(markedMetadata);
                await _markSessionRepository.Update(markSessionModel);
            }

            var scenariosForProject = await _scenarioClient.GetScenariosForProject(projectId);
            foreach (var scenarioModel in scenariosForProject)
            {
                var markedScenario = await _scenarioClient.MarkScenario(scenarioModel);
                markSessionModel.DependantResources.Add(markedScenario);
                await _markSessionRepository.Update(markSessionModel);
            }

            var resultConfigsForMetadata = new List<ResultConfigModel>();
            foreach (var metadataModel in metadataForProject)
            {
                resultConfigsForMetadata.AddRange(
                    await _resultConfigClient.GetResultConfigsForMetadata(metadataModel.DataId)
                );
            }
            // ResultConfigs obey the mark of the metadata!
            foreach (var resultConfigModel in resultConfigsForMetadata)
            {
                var markedResultConfig =
                    await _resultConfigClient.CreateMarkedResultConfig(resultConfigModel);
                markSessionModel.DependantResources.Add(markedResultConfig);
                await _markSessionRepository.Update(markSessionModel);
            }

            var simPlansForProject = await _simPlanClient.GetSimPlansForProject(projectId);
            foreach (var simPlanModel in simPlansForProject)
            {
                var markedSimPlanModel = await _simPlanClient.MarkSimPlan(simPlanModel, projectId);
                markSessionModel.DependantResources.Add(markedSimPlanModel);
                await _markSessionRepository.Update(markSessionModel);
            }

            var simRunsForProject = await _simRunClient.GetSimRunsForProject(projectId);
            foreach (var simRunModel in simRunsForProject)
            {
                var markedSimRun = await _simRunClient.StopSimRun(simRunModel, projectId);
                markSessionModel.DependantResources.Add(markedSimRun);
                await _markSessionRepository.Update(markSessionModel);
            }

            foreach (var simRunModel in simRunsForProject)
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