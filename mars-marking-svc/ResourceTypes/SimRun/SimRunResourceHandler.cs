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
        private readonly ISimRunClient _simRunClient;
        private readonly IResultDataClient _resultDataClient;
        private readonly ILoggerService _loggerService;
        private readonly IErrorService _errorService;

        public SimRunResourceHandler(
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            ILoggerService loggerService,
            IErrorService errorService
        )
        {
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _loggerService = loggerService;
            _errorService = errorService;
        }

        public async Task<IActionResult> MarkSimRunDependantResources(string simRunId, string projectId)
        {
            var dependantResources = new List<DependantResourceModel>();
            _loggerService.LogWarningEvent(
                $"Mark session for simPlan, id: {simRunId}, projectId: {projectId} will not be created, because simRuns do not have a mark! However, the simRun will be stopped!"
            );

            try
            {
                var sourceSimRun = await _simRunClient.GetSimRun(simRunId, projectId);
                var markedSourceSimRun = await _simRunClient.StopSimRun(simRunId, projectId);
                dependantResources.Add(markedSourceSimRun);

                var markedResultData = await _resultDataClient.CreateMarkedResultData(sourceSimRun);
                dependantResources.Add(markedResultData);

                return new OkObjectResult(dependantResources);
            }
            catch (Exception e)
            {
                _loggerService.LogErrorEvent(e);

                return _errorService.GetStatusCodeForError(e);
            }
        }
    }
}