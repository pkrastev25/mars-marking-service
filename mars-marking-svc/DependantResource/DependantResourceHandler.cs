using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mars_marking_svc.DependantResource.Interfaces;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Models;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Models;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;
using mars_marking_svc.Utils;

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

        public async Task MarkResourcesForMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            switch (markSessionModel.ResourceType)
            {
                case ResourceTypeEnum.Project:
                    await MarkResourcesForProjectMarkSession(markSessionModel);
                    break;
                case ResourceTypeEnum.Metadata:
                    await MarkResourcesForMetadataMarkSession(markSessionModel);
                    break;
                case ResourceTypeEnum.Scenario:
                    await MarkResourcesForScenarioMarkSession(markSessionModel);
                    break;
                case ResourceTypeEnum.ResultConfig:
                    await MarkResourcesForResultConfigMarkSession(markSessionModel);
                    break;
                case ResourceTypeEnum.SimPlan:
                    await MarkResourcesForSimPlanMarkSession(markSessionModel);
                    break;
                case ResourceTypeEnum.SimRun:
                    await MarkResourcesForSimRunMarkSession(markSessionModel);
                    break;
                case ResourceTypeEnum.ResultData:
                    await MarkResourcesForResultDataMarkSession(markSessionModel);
                    break;
                default:
                    throw new UnknownResourceTypeException(
                        $"{markSessionModel.ResourceType} is unknown!"
                    );
            }
        }

        public async Task UnmarkResourcesForMarkSession(
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
            var taskList = dependantResourceModels.Select(markedResourceModel => Task.Run(
                async () =>
                {
                    await UnmarkResource(markedResourceModel, markSessionModel.ProjectId);

                    lock (markSessionModel.DependantResources)
                    {
                        markSessionModel.DependantResources.Remove(markedResourceModel);
                    }
                }
            )).ToList();

            await ExecuteTasksThenUpdateMarkSession(taskList, markSessionModel);
        }

        private async Task MarkResourcesForProjectMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            var projectId = markSessionModel.ProjectId;

            var metadataForProject = await _metadataClient.GetMetadataForProject(projectId);
            await MarkResourcesThenUpdateMarkSession(metadataForProject, projectId, markSessionModel);

            var scenariosForProject = await _scenarioClient.GetScenariosForProject(projectId);
            await MarkResourcesThenUpdateMarkSession(scenariosForProject, projectId, markSessionModel);

            var resultConfigsForMetadata = new List<ResultConfigModel>();
            foreach (var metadataModel in metadataForProject)
            {
                resultConfigsForMetadata.AddRange(
                    await _resultConfigClient.GetResultConfigsForMetadata(metadataModel.DataId)
                );
            }
            await MarkResourcesThenUpdateMarkSession(resultConfigsForMetadata, projectId, markSessionModel);

            var simPlansForProject = await _simPlanClient.GetSimPlansForProject(projectId);
            await MarkResourcesThenUpdateMarkSession(simPlansForProject, projectId, markSessionModel);

            var simRunsForProject = await _simRunClient.GetSimRunsForProject(projectId);
            await MarkResourcesThenUpdateMarkSession(simRunsForProject, projectId, markSessionModel);

            await MarkResultDataThenUpdateMarkSession(simRunsForProject, markSessionModel);
        }

        private async Task MarkResourcesForMetadataMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            var metadataId = markSessionModel.ResourceId;
            var projectId = markSessionModel.ProjectId;

            var markedSourceMetadata = await _metadataClient.MarkMetadata(metadataId);
            markSessionModel.DependantResources.Add(markedSourceMetadata);
            await _markSessionRepository.Update(markSessionModel);

            var scenariosForMetadata = await _scenarioClient.GetScenariosForMetadata(metadataId);
            await MarkResourcesThenUpdateMarkSession(scenariosForMetadata, projectId, markSessionModel);

            var resultConfigsForMetadata = await _resultConfigClient.GetResultConfigsForMetadata(metadataId);
            await MarkResourcesThenUpdateMarkSession(resultConfigsForMetadata, projectId, markSessionModel);

            var simPlansForScenarios = new List<SimPlanModel>();
            foreach (var scenarioModel in scenariosForMetadata)
            {
                simPlansForScenarios.AddRange(
                    await _simPlanClient.GetSimPlansForScenario(scenarioModel.ScenarioId, projectId)
                );
            }
            await MarkResourcesThenUpdateMarkSession(simPlansForScenarios, projectId, markSessionModel);

            var simRunsForSimPlans = new List<SimRunModel>();
            foreach (var simPlanModel in simPlansForScenarios)
            {
                simRunsForSimPlans.AddRange(
                    await _simRunClient.GetSimRunsForSimPlan(simPlanModel.Id, projectId)
                );
            }
            await MarkResourcesThenUpdateMarkSession(simRunsForSimPlans, projectId, markSessionModel);

            await MarkResultDataThenUpdateMarkSession(simRunsForSimPlans, markSessionModel);
        }

        private async Task MarkResourcesForScenarioMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            var scenarioId = markSessionModel.ResourceId;
            var projectId = markSessionModel.ProjectId;

            var markedSourceScenario = await _scenarioClient.MarkScenario(scenarioId);
            markSessionModel.DependantResources.Add(markedSourceScenario);
            await _markSessionRepository.Update(markSessionModel);

            var simPlansForScenario = await _simPlanClient.GetSimPlansForScenario(scenarioId, projectId);
            await MarkResourcesThenUpdateMarkSession(simPlansForScenario, projectId, markSessionModel);

            var simRunsForSimPlans = new List<SimRunModel>();
            foreach (var simPlanModel in simPlansForScenario)
            {
                simRunsForSimPlans.AddRange(
                    await _simRunClient.GetSimRunsForSimPlan(simPlanModel.Id, projectId)
                );
            }
            await MarkResourcesThenUpdateMarkSession(simRunsForSimPlans, projectId, markSessionModel);

            await MarkResultDataThenUpdateMarkSession(simRunsForSimPlans, markSessionModel);
        }

        private async Task MarkResourcesForResultConfigMarkSession(
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
            await MarkResourcesThenUpdateMarkSession(simPlansForResultConfig, projectId, markSessionModel);

            var simRunsForSimPlans = new List<SimRunModel>();
            foreach (var simPlanModel in simPlansForResultConfig)
            {
                simRunsForSimPlans.AddRange(
                    await _simRunClient.GetSimRunsForSimPlan(simPlanModel.Id, projectId)
                );
            }
            await MarkResourcesThenUpdateMarkSession(simRunsForSimPlans, projectId, markSessionModel);

            await MarkResultDataThenUpdateMarkSession(simRunsForSimPlans, markSessionModel);
        }

        private async Task MarkResourcesForSimPlanMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            var simPlanId = markSessionModel.ResourceId;
            var projectId = markSessionModel.ProjectId;

            var markedSourceSimPlan = await _simPlanClient.MarkSimPlan(simPlanId, projectId);
            markSessionModel.DependantResources.Add(markedSourceSimPlan);
            await _markSessionRepository.Update(markSessionModel);

            var simRunsForSimPlan = await _simRunClient.GetSimRunsForSimPlan(simPlanId, projectId);
            await MarkResourcesThenUpdateMarkSession(simRunsForSimPlan, projectId, markSessionModel);

            await MarkResultDataThenUpdateMarkSession(simRunsForSimPlan, markSessionModel);
        }

        private async Task MarkResourcesForSimRunMarkSession(
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

        private async Task MarkResourcesForResultDataMarkSession(
            MarkSessionModel markSessionModel
        )
        {
            var markedResulrData = await _resultDataClient.MarkResultData(markSessionModel.ResourceId);
            markSessionModel.DependantResources.Add(markedResulrData);
            await _markSessionRepository.Update(markSessionModel);
        }

        private async Task MarkResourceThenAddToMarkSession<TResourceModel>(
            TResourceModel resourceModel,
            string projectId,
            MarkSessionModel markSessionModel
        )
        {
            DependantResourceModel markedResource;

            if (resourceModel is MetadataModel)
            {
                markedResource = await _metadataClient.MarkMetadata(resourceModel as MetadataModel);
            }
            else if (resourceModel is ScenarioModel)
            {
                markedResource = await _scenarioClient.MarkScenario(resourceModel as ScenarioModel);
            }
            else if (resourceModel is ResultConfigModel)
            {
                // ResultConfigs obey the mark of the metadata!  
                markedResource =
                    await _resultConfigClient.CreateDependantResultConfigResource(resourceModel as ResultConfigModel);
            }
            else if (resourceModel is SimPlanModel)
            {
                markedResource = await _simPlanClient.MarkSimPlan(resourceModel as SimPlanModel, projectId);
            }
            else if (resourceModel is SimRunModel)
            {
                markedResource = await _simRunClient.MarkSimRun(resourceModel as SimRunModel, projectId);
            }
            else
            {
                throw new UnknownResourceModelException(
                    $"{resourceModel.GetType()} is unknown!"
                );
            }

            lock (markSessionModel.DependantResources)
            {
                markSessionModel.DependantResources.Add(markedResource);
            }
        }

        private async Task MarkResultDataThenUpdateMarkSession(
            IEnumerable<SimRunModel> simRunModels,
            MarkSessionModel markSessionModel
        )
        {
            if (simRunModels == null)
            {
                return;
            }

            var taskList = simRunModels.Select(simRunModel => Task.Run(
                async () =>
                {
                    var markedResultData = await _resultDataClient.MarkResultData(simRunModel);

                    lock (markSessionModel.DependantResources)
                    {
                        markSessionModel.DependantResources.Add(markedResultData);
                    }
                }
            )).ToList();

            await ExecuteTasksThenUpdateMarkSession(taskList, markSessionModel);
        }

        private async Task MarkResourcesThenUpdateMarkSession<TResourceModel>(
            IEnumerable<TResourceModel> resourceModels,
            string projectId,
            MarkSessionModel markSessionModel
        )
        {
            if (resourceModels == null)
            {
                return;
            }

            var taskList = resourceModels.Select(resourceModel => MarkResourceThenAddToMarkSession(
                resourceModel,
                projectId,
                markSessionModel
            )).ToList();

            await ExecuteTasksThenUpdateMarkSession(taskList, markSessionModel);
        }

        private async Task ExecuteTasksThenUpdateMarkSession(
            IEnumerable<Task> tasks,
            MarkSessionModel markSessionModel
        )
        {
            Exception exception = null;

            try
            {
                await TaskUtil.ExecuteTasksInParallel(tasks);
            }
            catch (Exception e)
            {
                exception = e;
            }
            finally
            {
                await _markSessionRepository.Update(markSessionModel);

                if (exception != null)
                {
                    throw exception;
                }
            }
        }

        private async Task UnmarkResource(
            DependantResourceModel dependantResourceModel,
            string projectId
        )
        {
            switch (dependantResourceModel.ResourceType)
            {
                case ResourceTypeEnum.Metadata:
                    await _metadataClient.UnmarkMetadata(dependantResourceModel);
                    break;
                case ResourceTypeEnum.Scenario:
                    await _scenarioClient.UnmarkScenario(dependantResourceModel);
                    break;
                case ResourceTypeEnum.ResultConfig:
                    _loggerService.LogSkipEvent(dependantResourceModel.ToString());
                    break;
                case ResourceTypeEnum.SimPlan:
                    await _simPlanClient.UnmarkSimPlan(dependantResourceModel, projectId);
                    break;
                case ResourceTypeEnum.SimRun:
                    await _simRunClient.UnmarkSimRun(dependantResourceModel);
                    break;
                case ResourceTypeEnum.ResultData:
                    await _resultDataClient.UnmarkResultData(dependantResourceModel);
                    break;
                default:
                    throw new UnknownResourceTypeException(
                        $"{dependantResourceModel} is unknown!"
                    );
            }
        }
    }
}