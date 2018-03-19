using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.MarkedResource.Interfaces;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.ResourceTypes.MarkedResource
{
    public class MarkedResourceHandler : IMarkedResourceHandler
    {
        private readonly IMetadataServiceClient _metadataServiceClient;
        private readonly IScenarioServiceClient _scenarioServiceClient;
        private readonly IResultConfigServiceClient _resultConfigServiceClient;
        private readonly ISimPlanServiceClient _simPlanServiceClient;
        private readonly ISimRunServiceClient _simRunServiceClient;
        private readonly IResultDataServiceClient _resultDataServiceClient;
        private readonly ILoggerService _loggerService;

        public MarkedResourceHandler(
            IMetadataServiceClient metadataServiceClient,
            IScenarioServiceClient scenarioServiceClient,
            IResultConfigServiceClient resultConfigServiceClient,
            ISimPlanServiceClient simPlanServiceClient,
            ISimRunServiceClient simRunServiceClient,
            IResultDataServiceClient resultDataServiceClient,
            ILoggerService loggerService
        )
        {
            _metadataServiceClient = metadataServiceClient;
            _scenarioServiceClient = scenarioServiceClient;
            _resultConfigServiceClient = resultConfigServiceClient;
            _simPlanServiceClient = simPlanServiceClient;
            _simRunServiceClient = simRunServiceClient;
            _resultDataServiceClient = resultDataServiceClient;
            _loggerService = loggerService;
        }

        public async Task UnmarkMarkedResources(IEnumerable<MarkedResourceModel> markedResources, string projectId)
        {
            var failedToUnmarkResources = new List<MarkedResourceModel>();

            foreach (var markedResourceModel in markedResources)
            {
                switch (markedResourceModel.ResourceType)
                {
                    case "metadata":
                    {
                        try
                        {
                            await _metadataServiceClient.UnmarkMetadata(markedResourceModel.ResourceId);
                        }
                        catch (Exception e)
                        {
                            failedToUnmarkResources.Add(markedResourceModel);
                            _loggerService.LogExceptionMessageWithStackTrace(e);
                        }

                        break;
                    }
                    case "scenario":
                    {
                        try
                        {
                            await _scenarioServiceClient.UnmarkScenario(markedResourceModel.ResourceId);
                        }
                        catch (Exception e)
                        {
                            failedToUnmarkResources.Add(markedResourceModel);
                            _loggerService.LogExceptionMessageWithStackTrace(e);
                        }

                        break;
                    }
                    case "resultConfig":
                    {
                        try
                        {
                            await _resultConfigServiceClient.UnmarkResultConfig(markedResourceModel.ResourceId);
                        }
                        catch (Exception e)
                        {
                            failedToUnmarkResources.Add(markedResourceModel);
                            _loggerService.LogExceptionMessageWithStackTrace(e);
                        }

                        break;
                    }
                    case "simPlan":
                    {
                        try
                        {
                            await _simPlanServiceClient.UnmarkSimPlan(markedResourceModel.ResourceId, projectId);
                        }
                        catch (Exception e)
                        {
                            failedToUnmarkResources.Add(markedResourceModel);
                            _loggerService.LogExceptionMessageWithStackTrace(e);
                        }

                        break;
                    }
                    case "simRun":
                    {
                        try
                        {
                            await _simRunServiceClient.UnmarkSimRun(markedResourceModel.ResourceId, projectId);
                        }
                        catch (Exception e)
                        {
                            failedToUnmarkResources.Add(markedResourceModel);
                            _loggerService.LogExceptionMessageWithStackTrace(e);
                        }

                        break;
                    }
                    case "resultData":
                    {
                        try
                        {
                            await _resultDataServiceClient.UnmarkResultData(markedResourceModel.ResourceId);
                        }
                        catch (Exception e)
                        {
                            failedToUnmarkResources.Add(markedResourceModel);
                            _loggerService.LogExceptionMessageWithStackTrace(e);
                        }

                        break;
                    }
                    default:
                    {
                        _loggerService.LogExceptionMessage(
                            $"[ERROR] Unknown resource type: {markedResourceModel.ResourceType} with id: {markedResourceModel.ResourceId} is encountered while unmarking!"
                        );
                        break;
                    }
                }
            }
        }
    }
}