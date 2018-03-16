using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Models;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.SimPlan
{
    public class SimPlanResourceHandler : ISimPlanResourceHandler
    {
        private readonly ISimPlanServiceClient _simPlanServiceClient;
        private readonly ISimRunServiceClient _simRunServiceClient;
        private readonly ILoggerService _loggerService;

        public SimPlanResourceHandler(
            ISimPlanServiceClient simPlanServiceClient,
            ISimRunServiceClient simRunServiceClient,
            ILoggerService loggerService
        )
        {
            _simPlanServiceClient = simPlanServiceClient;
            _simRunServiceClient = simRunServiceClient;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkSimPlanDependantResources(string simPlanId, string projectId)
        {
            var markedResources = new List<MarkedResourceModel>();

            try
            {
                var sourceSimPlan = await _simPlanServiceClient.MarkSimPlan(simPlanId, projectId);
                markedResources.Add(sourceSimPlan);

                var simRunsForSimPlan = await _simRunServiceClient.GetSimRunsForSimPlan(simPlanId, projectId);
                foreach (var simRunModel in simRunsForSimPlan)
                {
                    markedResources.Add(
                        await _simRunServiceClient.MarkSimRun(simRunModel, projectId)
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