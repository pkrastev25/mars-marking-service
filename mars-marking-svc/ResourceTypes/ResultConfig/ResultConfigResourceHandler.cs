using System;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Models;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;

namespace mars_marking_svc.ResourceTypes.ResultConfig
{
    public class ResultConfigResourceHandler : IResultConfigResourceHandler
    {
        private readonly IResultConfigServiceClient _resultConfigServiceClient;
        private readonly ILoggerService _loggerService;

        public ResultConfigResourceHandler(
            IResultConfigServiceClient resultConfigServiceClient,
            ILoggerService loggerService
        )
        {
            _resultConfigServiceClient = resultConfigServiceClient;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkResultConfigDependantResources(string resultConfigId)
        {
            var markedResources = new ArrayList<MarkedResourceModel>();

            try
            {
                var sourceResultConfig = await _resultConfigServiceClient.MarkResultConfig(resultConfigId);
                markedResources.Add(sourceResultConfig);

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