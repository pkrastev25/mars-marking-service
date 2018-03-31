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
        private readonly IMetadataClient _metadataClient;
        private readonly IScenarioClient _scenarioClient;
        private readonly ISimPlanClient _simPlanClient;
        private readonly IDbMarkSessionClient _dbMarkSessionClient;
        private readonly ILoggerService _loggerService;

        public DbMarkSessionHandler(
            IMetadataClient metadataClient,
            IScenarioClient scenarioClient,
            ISimPlanClient simPlanClient,
            IDbMarkSessionClient dbMarkSessionClient,
            ILoggerService loggerService
        )
        {
            _metadataClient = metadataClient;
            _scenarioClient = scenarioClient;
            _simPlanClient = simPlanClient;
            _dbMarkSessionClient = dbMarkSessionClient;
            _loggerService = loggerService;
        }

        public async Task UnmarkResourcesForMarkSession(DbMarkSessionModel markSessionModel)
        {
            try
            {
                markSessionModel.State = DbMarkSessionModel.AbortingState;
                await _dbMarkSessionClient.Update(markSessionModel);
                _loggerService.LogUpdateEvent(markSessionModel.ToString());

                if (markSessionModel.SourceDependency != null)
                {
                    await UnmarkMarkedResource(markSessionModel.SourceDependency, markSessionModel.ProjectId);
                    markSessionModel.SourceDependency = null;
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                var markedDependantResources = new List<MarkedResourceModel>(markSessionModel.DependantResources);

                foreach (var markedResourceModel in markedDependantResources)
                {
                    var unmarkedResourceModel =
                        await UnmarkMarkedResource(markedResourceModel, markSessionModel.ProjectId);
                    var index = markSessionModel.DependantResources.FindIndex(m =>
                        m.ResourceId == unmarkedResourceModel.ResourceId);
                    markSessionModel.DependantResources.RemoveAt(index);
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                await _dbMarkSessionClient.Delete(markSessionModel);
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
                    await _metadataClient.UnmarkMetadata(markedResourceModel);
                    return markedResourceModel;
                }
                case "scenario":
                {
                    await _scenarioClient.UnmarkScenario(markedResourceModel);
                    return markedResourceModel;
                }
                case "resultConfig":
                {
                    _loggerService.LogSkipEvent(markedResourceModel.ToString());
                    return markedResourceModel;
                }
                case "simPlan":
                {
                    await _simPlanClient.UnmarkSimPlan(markedResourceModel, projectId);
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