using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.MarkedResource.Interfaces;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.ResultData
{
    public class ResultDataResourceHandler : IResultDataResourceHandler
    {
        private readonly IResultDataServiceClient _resultDataServiceClient;
        private readonly IMarkedResourceHandler _markedResourceHandler;
        private readonly ILoggerService _loggerService;

        public ResultDataResourceHandler(
            IResultDataServiceClient resultDataServiceClient,
            IMarkedResourceHandler markedResourceHandler,
            ILoggerService loggerService
        )
        {
            _resultDataServiceClient = resultDataServiceClient;
            _markedResourceHandler = markedResourceHandler;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkResultDataDependantResources(string resultDataId, string projectId)
        {
            var markedResources = new List<MarkedResourceModel>();

            try
            {
                markedResources.Add(
                    await _resultDataServiceClient.MarkResultData(resultDataId)
                );

                return new OkObjectResult(markedResources);
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