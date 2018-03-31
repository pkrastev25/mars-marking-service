using System;
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
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.Metadata
{
    public class MetadataResourceHandler : IMetadataResourceHandler
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

        public MetadataResourceHandler(
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

        public async Task<IActionResult> MarkMetadataDependantResources(string metadataId, string projectId)
        {
            var markSessionModel = new DbMarkSessionModel(metadataId, projectId, "metadata");

            try
            {
                await _dbService.InsertNewMarkSession(markSessionModel);

                var markedSourceMetadata = await _metadataServiceClient.MarkMetadata(metadataId);
                markSessionModel.DependantResources.Add(markedSourceMetadata);
                await _dbService.UpdateMarkSession(markSessionModel);

                var scenariosForMetadata = await _scenarioServiceClient.GetScenariosForMetadata(metadataId);
                foreach (var scenarioModel in scenariosForMetadata)
                {
                    var markedScenario = await _scenarioServiceClient.MarkScenario(scenarioModel);
                    markSessionModel.DependantResources.Add(markedScenario);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                var resultConfigsForMetadata = await _resultConfigServiceClient.GetResultConfigsForMetadata(metadataId);
                foreach (var resultConfigModel in resultConfigsForMetadata)
                {
                    // ResultConfigs obey the metadata mark!
                    var markedResultConfig =
                        await _resultConfigServiceClient.CreateMarkedResultConfig(resultConfigModel);
                    markSessionModel.DependantResources.Add(markedResultConfig);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                var simPlansForScenarios = new List<SimPlanModel>();
                foreach (var scenarioModel in scenariosForMetadata)
                {
                    simPlansForScenarios.AddRange(
                        await _simPlanServiceClient.GetSimPlansForScenario(scenarioModel.ScenarioId, projectId)
                    );
                }
                foreach (var simPlanModel in simPlansForScenarios)
                {
                    var markedSimPlan = await _simPlanServiceClient.MarkSimPlan(simPlanModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimPlan);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                var simRunsForSimPlans = new List<SimRunModel>();
                foreach (var simPlanModel in simPlansForScenarios)
                {
                    simRunsForSimPlans.AddRange(
                        await _simRunServiceClient.GetSimRunsForSimPlan(simPlanModel.Id, projectId)
                    );
                }
                foreach (var simRunModel in simRunsForSimPlans)
                {
                    var stoppedSimRun = await _simRunServiceClient.StopSimRun(simRunModel, projectId);
                    markSessionModel.DependantResources.Add(stoppedSimRun);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                foreach (var simRunModel in simRunsForSimPlans)
                {
                    var resultData = await _resultDataServiceClient.CreateMarkedResultData(simRunModel);
                    markSessionModel.DependantResources.Add(resultData);
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