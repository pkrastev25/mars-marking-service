using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.DependantResource.Interfaces;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Models;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.DependantResource
{
    public class DependantResourceHandler : IDependantResourceHandler
    {
        private readonly IMarkSessionRepository _markSessionRepository;
        private readonly IMetadataClient _metadataClient;
        private readonly IScenarioClient _scenarioClient;
        private readonly IResultConfigClient _resultConfigClient;
        private readonly ISimPlanClient _simPlanClient;
        private readonly ISimRunClient _simRunClient;
        private readonly IResultDataClient _resultDataClient;
        private readonly ILoggerService _loggerService;

        public DependantResourceHandler(
            IMarkSessionRepository markSessionRepository,
            IMetadataClient metadataClient,
            IScenarioClient scenarioClient,
            IResultConfigClient resultConfigClient,
            ISimPlanClient simPlanClient,
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            ILoggerService loggerService
        )
        {
            _markSessionRepository = markSessionRepository;
            _metadataClient = metadataClient;
            _scenarioClient = scenarioClient;
            _resultConfigClient = resultConfigClient;
            _simPlanClient = simPlanClient;
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _loggerService = loggerService;
        }

        public async Task GatherResourcesForMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            switch (markSessionModel.ResourceType)
            {
                case ResourceTypeEnum.Project:
                {
                    await GatherResourcesForProjectMarkSession(markSessionModel);
                    break;
                }
                case ResourceTypeEnum.Metadata:
                {
                    await GatherResourcesForMetadataMarkSession(markSessionModel);
                    break;
                }
                case ResourceTypeEnum.Scenario:
                {
                    await GatherResourcesForScenarioMarkSession(markSessionModel);
                    break;
                }
                case ResourceTypeEnum.ResultConfig:
                {
                    await GatherResourcesForResultConfigMarkSession(markSessionModel);
                    break;
                }
                case ResourceTypeEnum.SimPlan:
                {
                    await GatherResourcesForSimPlanMarkSession(markSessionModel);
                    break;
                }
                case ResourceTypeEnum.SimRun:
                {
                    await GatherResourcesForSimRunMarkSession(markSessionModel);
                    break;
                }
                case ResourceTypeEnum.ResultData:
                {
                    await GatherResourcesForResultDataMarkSession(markSessionModel);
                    break;
                }
                default:
                {
                    throw new UnknownResourceTypeException(
                        $"{markSessionModel.ResourceType} is unknown!"
                    );
                }
            }
        }

        public async Task FreeResourcesForMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            if (markSessionModel.SourceDependency != null)
            {
                await UnmarkResource(markSessionModel.SourceDependency, markSessionModel.ProjectId);
                markSessionModel.SourceDependency = null;
                await _markSessionRepository.Update(markSessionModel);
            }

            var dependantResourceModels = new List<DependantResourceModel>(markSessionModel.DependantResources);
            foreach (var markedResourceModel in dependantResourceModels)
            {
                var unmarkedResourceModel = await UnmarkResource(markedResourceModel, markSessionModel.ProjectId);
                var index = markSessionModel.DependantResources.FindIndex(m =>
                    m.ResourceId == unmarkedResourceModel.ResourceId
                );
                markSessionModel.DependantResources.RemoveAt(index);
                await _markSessionRepository.Update(markSessionModel);
            }
        }

        private async Task GatherResourcesForProjectMarkSession(
            MarkSessionModel markSessionModel
        )
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
                    await _resultConfigClient.CreateDependantResultConfigResource(resultConfigModel);
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
                var markedSimRun = await _simRunClient.MarkSimRun(simRunModel, projectId);
                markSessionModel.DependantResources.Add(markedSimRun);
                await _markSessionRepository.Update(markSessionModel);
            }

            foreach (var simRunModel in simRunsForProject)
            {
                var markedResultData = await _resultDataClient.MarkResultData(simRunModel);
                markSessionModel.DependantResources.Add(markedResultData);
                await _markSessionRepository.Update(markSessionModel);
            }
        }

        private async Task GatherResourcesForMetadataMarkSession(
            MarkSessionModel markSessionModel
        )
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
                    await _resultConfigClient.CreateDependantResultConfigResource(resultConfigModel);
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
                var markedSimRun = await _simRunClient.MarkSimRun(simRunModel, projectId);
                markSessionModel.DependantResources.Add(markedSimRun);
                await _markSessionRepository.Update(markSessionModel);
            }

            foreach (var simRunModel in simRunsForSimPlans)
            {
                var markedResultData = await _resultDataClient.MarkResultData(simRunModel);
                markSessionModel.DependantResources.Add(markedResultData);
                await _markSessionRepository.Update(markSessionModel);
            }
        }

        private async Task GatherResourcesForScenarioMarkSession(
            MarkSessionModel markSessionModel
        )
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
                var markedSimRun = await _simRunClient.MarkSimRun(simRunModel.Id, projectId);
                markSessionModel.DependantResources.Add(markedSimRun);
                await _markSessionRepository.Update(markSessionModel);
            }

            foreach (var simRunModel in simRunsForSimPlans)
            {
                var markedResultData = await _resultDataClient.MarkResultData(simRunModel);
                markSessionModel.DependantResources.Add(markedResultData);
                await _markSessionRepository.Update(markSessionModel);
            }
        }

        private async Task GatherResourcesForResultConfigMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            var resultConfigId = markSessionModel.ResourceId;
            var projectId = markSessionModel.ProjectId;

            var sourceResultConfig = await _resultConfigClient.GetResultConfig(resultConfigId);
            var sourceDependantResource =
                new DependantResourceModel(ResourceTypeEnum.Metadata, sourceResultConfig.ModelId);
            markSessionModel.SourceDependency = sourceDependantResource;
            await _markSessionRepository.Update(markSessionModel);

            var markedSourceResultConfig =
                await _resultConfigClient.CreateDependantResultConfigResource(resultConfigId);
            markSessionModel.DependantResources.Add(markedSourceResultConfig);
            await _markSessionRepository.Update(markSessionModel);

            var simPlansForResultConfig =
                await _simPlanClient.GetSimPlansForResultConfig(resultConfigId, projectId);
            foreach (var simPlanModel in simPlansForResultConfig)
            {
                var markedSimPlan = await _simPlanClient.MarkSimPlan(simPlanModel, projectId);
                markSessionModel.DependantResources.Add(markedSimPlan);
                await _markSessionRepository.Update(markSessionModel);
            }

            var simRunsForSimPlans = new List<SimRunModel>();
            foreach (var simPlanModel in simPlansForResultConfig)
            {
                simRunsForSimPlans.AddRange(
                    await _simRunClient.GetSimRunsForSimPlan(simPlanModel.Id, projectId)
                );
            }
            foreach (var simRunModel in simRunsForSimPlans)
            {
                var markedSimSun = await _simRunClient.MarkSimRun(simRunModel, projectId);
                markSessionModel.DependantResources.Add(markedSimSun);
                await _markSessionRepository.Update(markSessionModel);
            }

            foreach (var simRunModel in simRunsForSimPlans)
            {
                var markedResultData = await _resultDataClient.MarkResultData(simRunModel);
                markSessionModel.DependantResources.Add(markedResultData);
                await _markSessionRepository.Update(markSessionModel);
            }
        }

        private async Task GatherResourcesForSimPlanMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            var simPlanId = markSessionModel.ResourceId;
            var projectId = markSessionModel.ProjectId;

            var markedSourceSimPlan = await _simPlanClient.MarkSimPlan(simPlanId, projectId);
            markSessionModel.DependantResources.Add(markedSourceSimPlan);
            await _markSessionRepository.Update(markSessionModel);

            var simRunsForSimPlan = await _simRunClient.GetSimRunsForSimPlan(simPlanId, projectId);
            foreach (var simRunModel in simRunsForSimPlan)
            {
                var markedSimRun = await _simRunClient.MarkSimRun(simRunModel, projectId);
                markSessionModel.DependantResources.Add(markedSimRun);
                await _markSessionRepository.Update(markSessionModel);
            }

            foreach (var simRunModel in simRunsForSimPlan)
            {
                var markedResultData = await _resultDataClient.MarkResultData(simRunModel);
                markSessionModel.DependantResources.Add(markedResultData);
                await _markSessionRepository.Update(markSessionModel);
            }
        }

        private async Task GatherResourcesForSimRunMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            var simRunId = markSessionModel.ResourceId;
            var projectId = markSessionModel.ProjectId;
            var sourceSimRun = await _simRunClient.GetSimRun(simRunId, projectId);

            var markedSourceSimRun = await _simRunClient.MarkSimRun(simRunId, projectId);
            markSessionModel.DependantResources.Add(markedSourceSimRun);
            await _markSessionRepository.Update(markSessionModel);

            var markedResultData = await _resultDataClient.MarkResultData(sourceSimRun);
            markSessionModel.DependantResources.Add(markedResultData);
            await _markSessionRepository.Update(markSessionModel);
        }

        private async Task GatherResourcesForResultDataMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            var markedResulrData = await _resultDataClient.MarkResultData(markSessionModel.ResourceId);
            markSessionModel.DependantResources.Add(markedResulrData);
            await _markSessionRepository.Update(markSessionModel);
        }

        private async Task<DependantResourceModel> UnmarkResource(
            DependantResourceModel dependantResourceModel,
            string projectId
        )
        {
            switch (dependantResourceModel.ResourceType)
            {
                case ResourceTypeEnum.Metadata:
                {
                    await _metadataClient.UnmarkMetadata(dependantResourceModel);
                    return dependantResourceModel;
                }
                case ResourceTypeEnum.Scenario:
                {
                    await _scenarioClient.UnmarkScenario(dependantResourceModel);
                    return dependantResourceModel;
                }
                case ResourceTypeEnum.ResultConfig:
                {
                    _loggerService.LogSkipEvent(dependantResourceModel.ToString());
                    return dependantResourceModel;
                }
                case ResourceTypeEnum.SimPlan:
                {
                    await _simPlanClient.UnmarkSimPlan(dependantResourceModel, projectId);
                    return dependantResourceModel;
                }
                case ResourceTypeEnum.SimRun:
                {
                    await _simRunClient.UnmarkSimRun(dependantResourceModel);
                    return dependantResourceModel;
                }
                case ResourceTypeEnum.ResultData:
                {
                    await _resultDataClient.UnmarkResultData(dependantResourceModel);
                    return dependantResourceModel;
                }
                default:
                {
                    throw new UnknownResourceTypeException(
                        $"{dependantResourceModel} is unknown!"
                    );
                }
            }
        }
    }
}