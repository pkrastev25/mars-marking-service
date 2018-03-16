using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.ResultData
{
    public class ResultDataResourceHandler : IResultDataResourceHandler
    {
        private readonly IResultDataServiceClient _resultDataServiceClient;
        private readonly ILoggerService _loggerService;

        public ResultDataResourceHandler(
            IResultDataServiceClient resultDataServiceClient,
            ILoggerService loggerService
        )
        {
            _resultDataServiceClient = resultDataServiceClient;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkResultDataDependantResources(string resultDataId)
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
                // TODO: Remove the marks
                _loggerService.LogExceptionMessageWithStackTrace(e);
                return new StatusCodeResult(503);
            }
        }
    }
}