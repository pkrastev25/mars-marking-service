using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.MarkedResource.Interfaces;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.ResourceTypes.MarkedResource
{
    public class DbMarkSessionHandler : IDbMarkSessionHandler
    {
        private readonly IMetadataServiceClient _metadataServiceClient;
        private readonly IScenarioServiceClient _scenarioServiceClient;
        private readonly ISimPlanServiceClient _simPlanServiceClient;
        private readonly IDbService _dbService;
        private readonly ILoggerService _loggerService;

        public DbMarkSessionHandler(
            IMetadataServiceClient metadataServiceClient,
            IScenarioServiceClient scenarioServiceClient,
            ISimPlanServiceClient simPlanServiceClient,
            IDbService dbService,
            ILoggerService loggerService
        )
        {
            _metadataServiceClient = metadataServiceClient;
            _scenarioServiceClient = scenarioServiceClient;
            _simPlanServiceClient = simPlanServiceClient;
            _dbService = dbService;
            _loggerService = loggerService;
        }

        public async Task UnmarkResourcesForMarkSession(DbMarkSessionModel markSessionModel)
        {
            try
            {
                markSessionModel.State = DbMarkSessionModel.AbortingState;
                await _dbService.UpdateMarkSession(markSessionModel);
                _loggerService.LogUpdateEvent(markSessionModel.ToString());

                if (markSessionModel.SourceDependency != null)
                {
                    await UnmarkMarkedResource(markSessionModel.SourceDependency, markSessionModel.ProjectId);
                    markSessionModel.SourceDependency = null;
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                var markedDependantResources = new List<MarkedResourceModel>(markSessionModel.DependantResources);

                foreach (var markedResourceModel in markedDependantResources)
                {
                    var unmarkedResourceModel =
                        await UnmarkMarkedResource(markedResourceModel, markSessionModel.ProjectId);
                    var index = markSessionModel.DependantResources.FindIndex(m =>
                        m.ResourceId == unmarkedResourceModel.ResourceId);
                    markSessionModel.DependantResources.RemoveAt(index);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                await _dbService.DeleteMarkSession(markSessionModel);
            }
            catch (Exception e)
            {
                _loggerService.LogErrorEvent(e);
            }
        }

        private async Task<MarkedResourceModel> UnmarkMarkedResource(
            MarkedResourceModel markedResourceModel,
            string projectId
        )
        {
            switch (markedResourceModel.ResourceType)
            {
                case "metadata":
                {
                    await _metadataServiceClient.UnmarkMetadata(markedResourceModel);
                    return markedResourceModel;
                }
                case "scenario":
                {
                    await _scenarioServiceClient.UnmarkScenario(markedResourceModel);
                    return markedResourceModel;
                }
                case "resultConfig":
                {
                    _loggerService.LogSkipEvent(markedResourceModel.ToString());
                    return markedResourceModel;
                }
                case "simPlan":
                {
                    await _simPlanServiceClient.UnmarkSimPlan(markedResourceModel, projectId);
                    return markedResourceModel;
                }
                case "simRun":
                {
                    _loggerService.LogSkipEvent(markedResourceModel.ToString());
                    return markedResourceModel;
                }
                case "resultData":
                {
                    _loggerService.LogSkipEvent(markedResourceModel.ToString());
                    return markedResourceModel;
                }
                default:
                {
                    _loggerService.LogWarningEvent(
                        $"Unknown {markedResourceModel} is encountered while unmarking! This might lead to an error in the system!"
                    );
                    return markedResourceModel;
                }
            }
        }
    }
}