using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.MarkedResource.Interfaces;
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
        private readonly ILoggerService _loggerService;
        private readonly IScenarioServiceClient _scenarioServiceClient;
        private readonly ISimPlanServiceClient _simPlanServiceClient;
        private readonly ISimRunServiceClient _simRunServiceClient;
        private readonly IResultDataServiceClient _resultDataServiceClient;
        private readonly IMarkedResourceHandler _markedResourceHandler;

        public ScenarioResourceHandler(
            IScenarioServiceClient scenarioServiceClient,
            ISimPlanServiceClient simPlanServiceClient,
            ISimRunServiceClient simRunServiceClient,
            IResultDataServiceClient resultDataServiceClient,
            IMarkedResourceHandler markedResourceHandler,
            ILoggerService loggerService
        )
        {
            _scenarioServiceClient = scenarioServiceClient;
            _simPlanServiceClient = simPlanServiceClient;
            _simRunServiceClient = simRunServiceClient;
            _resultDataServiceClient = resultDataServiceClient;
            _markedResourceHandler = markedResourceHandler;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkScenarioDependantResources(string scenarioId, string projectId)
        {
            var markedResources = new List<MarkedResourceModel>();

            try
            {
                var sourceScenario = await _scenarioServiceClient.MarkScenario(scenarioId);
                markedResources.Add(sourceScenario);

                var simPlansForScenario = await _simPlanServiceClient.GetSimPlansForScenario(scenarioId, projectId);
                foreach (var simPlanModel in simPlansForScenario)
                {
                    markedResources.Add(
                        await _simPlanServiceClient.MarkSimPlan(simPlanModel, projectId)
                    );
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
                    markedResources.Add(
                        await _simRunServiceClient.MarkSimRun(simRunModel.Id, projectId)
                    );
                }

                foreach (var simRunModel in simRunsForSimPlans)
                {
                    markedResources.Add(
                        await _resultDataServiceClient.MarkResultData(simRunModel)
                    );
                }

                return new OkObjectResult(markedResources);
            }
            catch (FailedToGetResourceException e)
            {
                _loggerService.LogExceptionMessage(e);
                var unused = _markedResourceHandler.UnmarkMarkedResources(markedResources, projectId);

                return new StatusCodeResult(503);
            }
            catch (FailedToUpdateResourceException e)
            {
                _loggerService.LogExceptionMessage(e);
                var unused = _markedResourceHandler.UnmarkMarkedResources(markedResources, projectId);

                return new StatusCodeResult(503);
            }
            catch (ResourceAlreadyMarkedException e)
            {
                _loggerService.LogExceptionMessage(e);
                var unused = _markedResourceHandler.UnmarkMarkedResources(markedResources, projectId);

                return new StatusCodeResult(503);
            }
            catch (CannotMarkResourceException e)
            {
                _loggerService.LogExceptionMessage(e);
                var unused = _markedResourceHandler.UnmarkMarkedResources(markedResources, projectId);

                return new StatusCodeResult(409);
            }
            catch (Exception e)
            {
                _loggerService.LogExceptionMessageWithStackTrace(e);
                var unused = _markedResourceHandler.UnmarkMarkedResources(markedResources, projectId);

                return new StatusCodeResult(503);
            }
        }
    }
}