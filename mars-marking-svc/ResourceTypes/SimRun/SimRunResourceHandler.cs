using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.MarkedResource.Interfaces;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.SimRun
{
    public class SimRunResourceHandler : ISimRunResourceHandler
    {
        private readonly ISimRunServiceClient _simRunServiceClient;
        private readonly IResultDataServiceClient _resultDataServiceClient;
        private readonly IMarkedResourceHandler _markedResourceHandler;
        private readonly ILoggerService _loggerService;

        public SimRunResourceHandler(
            ISimRunServiceClient simRunServiceClient,
            IResultDataServiceClient resultDataServiceClient,
            IMarkedResourceHandler markedResourceHandler,
            ILoggerService loggerService
        )
        {
            _simRunServiceClient = simRunServiceClient;
            _resultDataServiceClient = resultDataServiceClient;
            _markedResourceHandler = markedResourceHandler;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkSimRunDependantResources(string simRunId, string projectId)
        {
            var markedResources = new List<MarkedResourceModel>();

            try
            {
                var sourceSimRun = await _simRunServiceClient.GetSimRun(simRunId, projectId);
                var markedSourceSimRun = await _simRunServiceClient.MarkSimRun(sourceSimRun, projectId);
                markedResources.Add(markedSourceSimRun);

                markedResources.Add(
                    await _resultDataServiceClient.MarkResultData(sourceSimRun)
                );

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