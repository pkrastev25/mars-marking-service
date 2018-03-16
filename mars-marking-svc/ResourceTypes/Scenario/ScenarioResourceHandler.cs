﻿using System;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Models;
using mars_marking_svc.ResourceTypes.Scenario.Models;
using mars_marking_svc.ResourceTypes.SimPlan.Models;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;

namespace mars_marking_svc.ResourceTypes.Scenario
{
    public class ScenarioResourceHandler : IScenarioResourceHandler
    {
        private readonly ILoggerService _loggerService;
        private readonly IScenarioServiceClient _scenarioServiceClient;
        private readonly ISimPlanServiceClient _simPlanServiceClient;

        public ScenarioResourceHandler(
            IScenarioServiceClient scenarioServiceClient,
            ISimPlanServiceClient simPlanServiceClient,
            ILoggerService loggerService
        )
        {
            _scenarioServiceClient = scenarioServiceClient;
            _simPlanServiceClient = simPlanServiceClient;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkScenarioDependantResources(string scenarioId, string projectId)
        {
            var markedResources = new ArrayList<MarkedResourceModel>();

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

                return new OkObjectResult(markedResources);
            }
            catch (FailedToGetResourceException e)
            {
                // TODO: Remove the marks
                _loggerService.LogExceptionMessage(e);
                return new StatusCodeResult(503);
            }
            catch (FailedToUpdateResourceException e)
            {
                // TODO: Remove the marks
                _loggerService.LogExceptionMessage(e);
                return new StatusCodeResult(503);
            }
            catch (ResourceAlreadyMarkedException e)
            {
                // TODO: Remove the marks
                _loggerService.LogExceptionMessage(e);
                return new StatusCodeResult(503);
            }
            catch (CannotMarkResourceException e)
            {
                // TODO: Remove the marks
                _loggerService.LogExceptionMessage(e);
                return new StatusCodeResult(409);
            }
            catch (Exception e)
            {
                // TODO: Remove the marks
                _loggerService.LogExceptionMessageWithStackTrace(e);
                return new StatusCodeResult(503);
            }
        }
    }
}