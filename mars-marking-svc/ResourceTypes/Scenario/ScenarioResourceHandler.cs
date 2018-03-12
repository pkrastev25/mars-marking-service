using System;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Models;
using mars_marking_svc.ResourceTypes.Scenario.Models;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;

namespace mars_marking_svc.ResourceTypes.Scenario
{
    public class ScenarioResourceHandler : IScenarioResourceHandler
    {
        private readonly ILoggerService _loggerService;
        private readonly IScenarioServiceClient _scenarioServiceClient;

        public ScenarioResourceHandler(
            IScenarioServiceClient scenarioServiceClient,
            ILoggerService loggerService
        )
        {
            _scenarioServiceClient = scenarioServiceClient;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkScenarioDependantResources(string scenarioId)
        {
            var markedResources = new ArrayList<MarkedResourceModel>();

            try
            {
                var sourceScenario = await _scenarioServiceClient.MarkScenario(scenarioId);
                markedResources.Add(sourceScenario);

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