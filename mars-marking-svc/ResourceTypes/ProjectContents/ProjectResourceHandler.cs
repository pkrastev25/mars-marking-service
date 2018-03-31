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
        private readonly IMetadataServiceClient _metadataServiceClient;
        private readonly IScenarioServiceClient _scenarioServiceClient;
        private readonly IResultConfigServiceClient _resultConfigServiceClient;
        private readonly ISimPlanServiceClient _simPlanServiceClient;
        private readonly ISimRunServiceClient _simRunServiceClient;
        private readonly IResultDataServiceClient _resultDataServiceClient;
        private readonly IDbService _dbService;
        private readonly ILoggerService _loggerService;
        private readonly IErrorHandlerService _errorHandlerService;

        public ProjectResourceHandler(
            IMetadataServiceClient metadataServiceClient,
            IScenarioServiceClient scenarioServiceClient,
            IResultConfigServiceClient resultConfigServiceClient,
            ISimPlanServiceClient simPlanServiceClient,
            ISimRunServiceClient simRunServiceClient,
            IResultDataServiceClient resultDataServiceClient,
            IDbService dbService,
            ILoggerService loggerService,
            IErrorHandlerService errorHandlerService
        )
        {
            _metadataServiceClient = metadataServiceClient;
            _scenarioServiceClient = scenarioServiceClient;
            _resultConfigServiceClient = resultConfigServiceClient;
            _simPlanServiceClient = simPlanServiceClient;
            _simRunServiceClient = simRunServiceClient;
            _resultDataServiceClient = resultDataServiceClient;
            _dbService = dbService;
            _loggerService = loggerService;
            _errorHandlerService = errorHandlerService;
        }

        public async Task<IActionResult> MarkProjectDependantResources(string projectId)
        {
            var markSessionModel = new DbMarkSessionModel(projectId, projectId, "project");

            try
            {
                await _dbService.InsertNewMarkSession(markSessionModel);

                var metadataForProject = await _metadataServiceClient.GetMetadataForProject(projectId);
                foreach (var metadataModel in metadataForProject)
                {
                    var markedMetadata = await _metadataServiceClient.MarkMetadata(metadataModel);
                    markSessionModel.DependantResources.Add(markedMetadata);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                var scenariosForProject = await _scenarioServiceClient.GetScenariosForProject(projectId);
                foreach (var scenarioModel in scenariosForProject)
                {
                    var markedScenario = await _scenarioServiceClient.MarkScenario(scenarioModel);
                    markSessionModel.DependantResources.Add(markedScenario);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                var resultConfigsForMetadata = new List<ResultConfigModel>();
                foreach (var metadataModel in metadataForProject)
                {
                    resultConfigsForMetadata.AddRange(
                        await _resultConfigServiceClient.GetResultConfigsForMetadata(metadataModel.DataId)
                    );
                }
                // ResultConfigs obey the mark of the metadata!
                foreach (var resultConfigModel in resultConfigsForMetadata)
                {
                    var markedResultConfig =
                        await _resultConfigServiceClient.CreateMarkedResultConfig(resultConfigModel);
                    markSessionModel.DependantResources.Add(markedResultConfig);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                var simPlansForProject = await _simPlanServiceClient.GetSimPlansForProject(projectId);
                foreach (var simPlanModel in simPlansForProject)
                {
                    var markedSimPlanModel = await _simPlanServiceClient.MarkSimPlan(simPlanModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimPlanModel);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                var simRunsForProject = await _simRunServiceClient.GetSimRunsForProject(projectId);
                foreach (var simRunModel in simRunsForProject)
                {
                    var markedSimRun = await _simRunServiceClient.StopSimRun(simRunModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimRun);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                foreach (var simRunModel in simRunsForProject)
                {
                    var markedResultData = await _resultDataServiceClient.CreateMarkedResultData(simRunModel);
                    markSessionModel.DependantResources.Add(markedResultData);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                markSessionModel.State = DbMarkSessionModel.DoneState;
                await _dbService.UpdateMarkSession(markSessionModel);
                _loggerService.LogUpdateEvent(markSessionModel.ToString());

                return new OkObjectResult(markSessionModel.DependantResources);
            }
            catch (Exception e)
            {
                var unused = _errorHandlerService.HandleError(e, markSessionModel);

                return _errorHandlerService.GetStatusCodeForError(e);
            }
        }
    }
}