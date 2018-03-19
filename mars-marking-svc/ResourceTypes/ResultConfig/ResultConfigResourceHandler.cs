using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.MarkedResource.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.ResultConfig
{
    public class ResultConfigResourceHandler : IResultConfigResourceHandler
    {
        private readonly IResultConfigServiceClient _resultConfigServiceClient;
        private readonly ISimPlanServiceClient _simPlanServiceClient;
        private readonly ISimRunServiceClient _simRunServiceClient;
        private readonly IResultDataServiceClient _resultDataServiceClient;
        private readonly IMarkedResourceHandler _markedResourceHandler;
        private readonly ILoggerService _loggerService;

        public ResultConfigResourceHandler(
            IResultConfigServiceClient resultConfigServiceClient,
            ISimPlanServiceClient simPlanServiceClient,
            ISimRunServiceClient simRunServiceClient,
            IResultDataServiceClient resultDataServiceClient,
            IMarkedResourceHandler markedResourceHandler,
            ILoggerService loggerService
        )
        {
            _resultConfigServiceClient = resultConfigServiceClient;
            _simPlanServiceClient = simPlanServiceClient;
            _simRunServiceClient = simRunServiceClient;
            _resultDataServiceClient = resultDataServiceClient;
            _markedResourceHandler = markedResourceHandler;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkResultConfigDependantResources(string resultConfigId, string projectId)
        {
            var markedResources = new List<MarkedResourceModel>();

            try
            {
                var sourceResultConfig = await _resultConfigServiceClient.MarkResultConfig(resultConfigId);
                markedResources.Add(sourceResultConfig);

                var simPlansForResultConfig =
                    await _simPlanServiceClient.GetSimPlansForResultConfig(resultConfigId, projectId);
                foreach (var simPlanModel in simPlansForResultConfig)
                {
                    markedResources.Add(
                        await _simPlanServiceClient.MarkSimPlan(simPlanModel, projectId)
                    );
                }

                var simRunsForSimPlans = new List<SimRunModel>();
                foreach (var simPlanModel in simPlansForResultConfig)
                {
                    simRunsForSimPlans.AddRange(
                        await _simRunServiceClient.GetSimRunsForSimPlan(simPlanModel.Id, projectId)
                    );
                }
                foreach (var simRunModel in simRunsForSimPlans)
                {
                    markedResources.Add(
                        await _simRunServiceClient.MarkSimRun(simRunModel, projectId)
                    );
                }

                foreach (var simRunModel in simRunsForSimPlans)
                {
                    markedResources.Add(
                        await _resultDataServiceClient.MarkResultData(simRunModel)
                    );
                }

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