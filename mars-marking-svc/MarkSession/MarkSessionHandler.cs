using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.MarkedResource
{
    public class MarkSessionHandler : IMarkSessionHandler
    {
        private readonly IMetadataClient _metadataClient;
        private readonly IScenarioClient _scenarioClient;
        private readonly ISimPlanClient _simPlanClient;
        private readonly IMarkSessionRepository _markSessionRepository;
        private readonly ILoggerService _loggerService;

        public MarkSessionHandler(
            IMetadataClient metadataClient,
            IScenarioClient scenarioClient,
            ISimPlanClient simPlanClient,
            IMarkSessionRepository markSessionRepository,
            ILoggerService loggerService
        )
        {
            _metadataClient = metadataClient;
            _scenarioClient = scenarioClient;
            _simPlanClient = simPlanClient;
            _markSessionRepository = markSessionRepository;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> UnmarkResourcesForMarkSession(string resourceId)
        {
            try
            {
                var markSession = await _markSessionRepository.Get(resourceId);
                await UnmarkResourcesForMarkSession(markSession);

                return new OkResult();
            }
            catch (Exception e)
            {
                _loggerService.LogErrorEvent(e);

                return new StatusCodeResult(500);
            }
        }

        public async Task UnmarkResourcesForMarkSession(MarkSessionModel markSessionModel)
        {
            try
            {
                markSessionModel.State = MarkSessionModel.AbortingState;
                await _markSessionRepository.Update(markSessionModel);
                _loggerService.LogUpdateEvent(markSessionModel.ToString());

                if (markSessionModel.SourceDependency != null)
                {
                    await UnmarkMarkedResource(markSessionModel.SourceDependency, markSessionModel.ProjectId);
                    markSessionModel.SourceDependency = null;
                    await _markSessionRepository.Update(markSessionModel);
                }

                var markedDependantResources = new List<DependantResourceModel>(markSessionModel.DependantResources);

                foreach (var markedResourceModel in markedDependantResources)
                {
                    var unmarkedResourceModel =
                        await UnmarkMarkedResource(markedResourceModel, markSessionModel.ProjectId);
                    var index = markSessionModel.DependantResources.FindIndex(m =>
                        m.ResourceId == unmarkedResourceModel.ResourceId);
                    markSessionModel.DependantResources.RemoveAt(index);
                    await _markSessionRepository.Update(markSessionModel);
                }

                await _markSessionRepository.Delete(markSessionModel);
            }
            catch (Exception e)
            {
                _loggerService.LogErrorEvent(e);
            }
        }

        private async Task<DependantResourceModel> UnmarkMarkedResource(
            DependantResourceModel dependantResourceModel,
            string projectId
        )
        {
            switch (dependantResourceModel.ResourceType)
            {
                case "metadata":
                {
                    await _metadataClient.UnmarkMetadata(dependantResourceModel);
                    return dependantResourceModel;
                }
                case "scenario":
                {
                    await _scenarioClient.UnmarkScenario(dependantResourceModel);
                    return dependantResourceModel;
                }
                case "resultConfig":
                {
                    _loggerService.LogSkipEvent(dependantResourceModel.ToString());
                    return dependantResourceModel;
                }
                case "simPlan":
                {
                    await _simPlanClient.UnmarkSimPlan(dependantResourceModel, projectId);
                    return dependantResourceModel;
                }
                case "simRun":
                {
                    _loggerService.LogSkipEvent(dependantResourceModel.ToString());
                    return dependantResourceModel;
                }
                case "resultData":
                {
                    _loggerService.LogSkipEvent(dependantResourceModel.ToString());
                    return dependantResourceModel;
                }
                default:
                {
                    _loggerService.LogWarningEvent(
                        $"Unknown {dependantResourceModel} is encountered while unmarking! This might lead to an error in the system!"
                    );
                    return dependantResourceModel;
                }
            }
        }
    }
}