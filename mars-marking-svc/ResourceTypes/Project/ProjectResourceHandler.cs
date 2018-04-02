using System;
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
using Microsoft.AspNetCore.Mvc;

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
        private readonly IDbMarkSessionClient _dbMarkSessionClient;
        private readonly ILoggerService _loggerService;
        private readonly IErrorService _errorService;

        public ProjectResourceHandler(
            IMetadataClient metadataClient,
            IScenarioClient scenarioClient,
            IResultConfigClient resultConfigClient,
            ISimPlanClient simPlanClient,
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            IDbMarkSessionClient dbMarkSessionClient,
            ILoggerService loggerService,
            IErrorService errorService
        )
        {
            _metadataClient = metadataClient;
            _scenarioClient = scenarioClient;
            _resultConfigClient = resultConfigClient;
            _simPlanClient = simPlanClient;
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _dbMarkSessionClient = dbMarkSessionClient;
            _loggerService = loggerService;
            _errorService = errorService;
        }

        public async Task<IActionResult> MarkProjectDependantResources(string projectId)
        {
            var markSessionModel = new DbMarkSessionModel(projectId, projectId, "project");

            try
            {
                await _dbMarkSessionClient.Create(markSessionModel);

                var metadataForProject = await _metadataClient.GetMetadataForProject(projectId);
                foreach (var metadataModel in metadataForProject)
                {
                    var markedMetadata = await _metadataClient.MarkMetadata(metadataModel);
                    markSessionModel.DependantResources.Add(markedMetadata);
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                var scenariosForProject = await _scenarioClient.GetScenariosForProject(projectId);
                foreach (var scenarioModel in scenariosForProject)
                {
                    var markedScenario = await _scenarioClient.MarkScenario(scenarioModel);
                    markSessionModel.DependantResources.Add(markedScenario);
                    await _dbMarkSessionClient.Update(markSessionModel);
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
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                var simPlansForProject = await _simPlanClient.GetSimPlansForProject(projectId);
                foreach (var simPlanModel in simPlansForProject)
                {
                    var markedSimPlanModel = await _simPlanClient.MarkSimPlan(simPlanModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimPlanModel);
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                var simRunsForProject = await _simRunClient.GetSimRunsForProject(projectId);
                foreach (var simRunModel in simRunsForProject)
                {
                    var markedSimRun = await _simRunClient.StopSimRun(simRunModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimRun);
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                foreach (var simRunModel in simRunsForProject)
                {
                    var markedResultData = await _resultDataClient.CreateMarkedResultData(simRunModel);
                    markSessionModel.DependantResources.Add(markedResultData);
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                markSessionModel.State = DbMarkSessionModel.DoneState;
                await _dbMarkSessionClient.Update(markSessionModel);
                _loggerService.LogUpdateEvent(markSessionModel.ToString());

                return new OkObjectResult(markSessionModel.DependantResources);
            }
            catch (Exception e)
            {
                _errorService.HandleError(e, markSessionModel);

                return _errorService.GetStatusCodeForError(e);
            }
        }
    }
}