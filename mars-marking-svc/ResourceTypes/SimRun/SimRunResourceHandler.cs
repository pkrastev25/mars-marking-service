using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Models;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.SimRun
{
    public class SimRunResourceHandler : ISimRunResourceHandler
    {
        private readonly ISimRunServiceClient _simRunServiceClient;
        private readonly ILoggerService _loggerService;

        public SimRunResourceHandler(
            ISimRunServiceClient simRunServiceClient,
            ILoggerService loggerService
        )
        {
            _simRunServiceClient = simRunServiceClient;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkSimRunDependantResources(string simRunId, string projectId)
        {
            var markedResources = new List<MarkedResourceModel>();

            try
            {
                var sourceSimRun = await _simRunServiceClient.MarkSimRun(simRunId, projectId);
                markedResources.Add(sourceSimRun);

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