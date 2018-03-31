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
        private readonly IDbMarkSessionClient _dbMarkSessionClient;
        private readonly ILoggerService _loggerService;
        private readonly IErrorHandlerService _errorHandlerService;
        private readonly IScenarioClient _scenarioClient;
        private readonly ISimPlanClient _simPlanClient;
        private readonly ISimRunClient _simRunClient;
        private readonly IResultDataClient _resultDataClient;

        public ScenarioResourceHandler(
            IScenarioClient scenarioClient,
            ISimPlanClient simPlanClient,
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            IDbMarkSessionClient dbMarkSessionClient,
            ILoggerService loggerService,
            IErrorHandlerService errorHandlerService
        )
        {
            _scenarioClient = scenarioClient;
            _simPlanClient = simPlanClient;
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _dbMarkSessionClient = dbMarkSessionClient;
            _loggerService = loggerService;
            _errorHandlerService = errorHandlerService;
        }

        public async Task<IActionResult> MarkScenarioDependantResources(string scenarioId, string projectId)
        {
            var markSessionModel = new DbMarkSessionModel(projectId, projectId, "scenario");

            try
            {
                await _dbMarkSessionClient.Create(markSessionModel);

                var markedSourceScenario = await _scenarioClient.MarkScenario(scenarioId);
                markSessionModel.DependantResources.Add(markedSourceScenario);
                await _dbMarkSessionClient.Update(markSessionModel);

                var simPlansForScenario = await _simPlanClient.GetSimPlansForScenario(scenarioId, projectId);
                foreach (var simPlanModel in simPlansForScenario)
                {
                    var markedSimPlan = await _simPlanClient.MarkSimPlan(simPlanModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimPlan);
                    await _dbMarkSessionClient.Update(markSessionModel);
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
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                foreach (var simRunModel in simRunsForSimPlans)
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
                var unused = _errorHandlerService.HandleError(e, markSessionModel);

                return _errorHandlerService.GetStatusCodeForError(e);
            }
        }
    }
}