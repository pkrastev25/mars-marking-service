using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.Scenario
{
    public class ScenarioResourceHandler : IScenarioResourceHandler
    {
        private readonly IDbService _dbService;
        private readonly ILoggerService _loggerService;
        private readonly IErrorHandlerService _errorHandlerService;
        private readonly IScenarioServiceClient _scenarioServiceClient;
        private readonly ISimPlanServiceClient _simPlanServiceClient;
        private readonly ISimRunServiceClient _simRunServiceClient;
        private readonly IResultDataServiceClient _resultDataServiceClient;

        public ScenarioResourceHandler(
            IScenarioServiceClient scenarioServiceClient,
            ISimPlanServiceClient simPlanServiceClient,
            ISimRunServiceClient simRunServiceClient,
            IResultDataServiceClient resultDataServiceClient,
            IDbService dbService,
            ILoggerService loggerService,
            IErrorHandlerService errorHandlerService
        )
        {
            _scenarioServiceClient = scenarioServiceClient;
            _simPlanServiceClient = simPlanServiceClient;
            _simRunServiceClient = simRunServiceClient;
            _resultDataServiceClient = resultDataServiceClient;
            _dbService = dbService;
            _loggerService = loggerService;
            _errorHandlerService = errorHandlerService;
        }

        public async Task<IActionResult> MarkScenarioDependantResources(string scenarioId, string projectId)
        {
            var markSessionModel = new DbMarkSessionModel(projectId, projectId, "scenario");

            try
            {
                await _dbService.InsertNewMarkSession(markSessionModel);

                var markedSourceScenario = await _scenarioServiceClient.MarkScenario(scenarioId);
                markSessionModel.DependantResources.Add(markedSourceScenario);
                await _dbService.UpdateMarkSession(markSessionModel);

                var simPlansForScenario = await _simPlanServiceClient.GetSimPlansForScenario(scenarioId, projectId);
                foreach (var simPlanModel in simPlansForScenario)
                {
                    var markedSimPlan = await _simPlanServiceClient.MarkSimPlan(simPlanModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimPlan);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                var simRunsForSimPlans = new List<SimRunModel>();
                foreach (var simPlanModel in simPlansForScenario)
                {
                    simRunsForSimPlans.AddRange(
                        await _simRunServiceClient.GetSimRunsForSimPlan(simPlanModel.Id, projectId)
                    );
                }
                foreach (var simRunModel in simRunsForSimPlans)
                {
                    var markedSimRun = await _simRunServiceClient.StopSimRun(simRunModel.Id, projectId);
                    markSessionModel.DependantResources.Add(markedSimRun);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                foreach (var simRunModel in simRunsForSimPlans)
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