using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
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
        private readonly ILoggerService _loggerService;
        private readonly IErrorHandlerService _errorHandlerService;

        public SimRunResourceHandler(
            ISimRunServiceClient simRunServiceClient,
            IResultDataServiceClient resultDataServiceClient,
            ILoggerService loggerService,
            IErrorHandlerService errorHandlerService
        )
        {
            _simRunServiceClient = simRunServiceClient;
            _resultDataServiceClient = resultDataServiceClient;
            _loggerService = loggerService;
            _errorHandlerService = errorHandlerService;
        }

        public async Task<IActionResult> MarkSimRunDependantResources(string simRunId, string projectId)
        {
            var dependantResources = new List<MarkedResourceModel>();
            _loggerService.LogWarningEvent(
                $"Mark session for simPlan, id: {simRunId}, projectId: {projectId} will not be created, because simRuns do not have a mark! However, the simRun will be stopped!"
            );

            try
            {
                var sourceSimRun = await _simRunServiceClient.GetSimRun(simRunId, projectId);
                var markedSourceSimRun = await _simRunServiceClient.StopSimRun(simRunId, projectId);
                dependantResources.Add(markedSourceSimRun);

                var markedResultData = await _resultDataServiceClient.CreateMarkedResultData(sourceSimRun);
                dependantResources.Add(markedResultData);

                return new OkObjectResult(dependantResources);
            }
            catch (Exception e)
            {
                _loggerService.LogErrorEvent(e);

                return _errorHandlerService.GetStatusCodeForError(e);
            }
        }
    }
}