using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.ResourceTypes.MarkedResource.Dtos;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.ProjectContents.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mars_marking_svc.ResourceTypes.MarkedResource
{
    public class MarkSessionHandler : IMarkSessionHandler
    {
        private readonly IMetadataClient _metadataClient;
        private readonly IScenarioClient _scenarioClient;
        private readonly ISimPlanClient _simPlanClient;
        private readonly IMarkSessionRepository _markSessionRepository;
        private readonly ILoggerService _loggerService;
        private readonly IProjectResourceHandler _projectResourceHandler;
        private readonly IMetadataResourceHandler _metadataResourceHandler;
        private readonly IScenarioResourceHandler _scenarioResourceHandler;
        private readonly IResultConfigResourceHandler _resultConfigResourceHandler;
        private readonly ISimPlanResourceHandler _simPlanResourceHandler;
        private readonly ISimRunResourceHandler _simRunResourceHandler;
        private readonly IErrorService _errorService;
        private readonly IMapper _mapper;

        public MarkSessionHandler(
            IMetadataClient metadataClient,
            IScenarioClient scenarioClient,
            ISimPlanClient simPlanClient,
            IMarkSessionRepository markSessionRepository,
            ILoggerService loggerService,
            IProjectResourceHandler projectResourceHandler,
            IMetadataResourceHandler metadataResourceHandler,
            IScenarioResourceHandler scenarioResourceHandler,
            IResultConfigResourceHandler resultConfigResourceHandler,
            ISimPlanResourceHandler simPlanResourceHandler,
            ISimRunResourceHandler simRunResourceHandler,
            IErrorService errorService,
            IMapper mapper
        )
        {
            _metadataClient = metadataClient;
            _scenarioClient = scenarioClient;
            _simPlanClient = simPlanClient;
            _markSessionRepository = markSessionRepository;
            _loggerService = loggerService;
            _projectResourceHandler = projectResourceHandler;
            _metadataResourceHandler = metadataResourceHandler;
            _scenarioResourceHandler = scenarioResourceHandler;
            _resultConfigResourceHandler = resultConfigResourceHandler;
            _simPlanResourceHandler = simPlanResourceHandler;
            _simRunResourceHandler = simRunResourceHandler;
            _errorService = errorService;
            _mapper = mapper;
        }

        public async Task<IActionResult> CreateMarkSession(
            string resourceType,
            string resourceId,
            string markSessionType,
            string projectId
        )
        {
            var markSessionModel = new MarkSessionModel(resourceId, projectId, resourceType, markSessionType);

            try
            {
                await _markSessionRepository.Create(markSessionModel);

                switch (resourceType)
                {
                    case "project":
                        markSessionModel =
                            await _projectResourceHandler.GatherResourcesForMarkSession(markSessionModel);
                        break;
                    case "metadata":
                        markSessionModel =
                            await _metadataResourceHandler.GatherResourcesForMarkSession(markSessionModel);
                        break;
                    case "scenario":
                        markSessionModel =
                            await _scenarioResourceHandler.GatherResourcesForMarkSession(markSessionModel);
                        break;
                    case "resultConfig":
                        markSessionModel =
                            await _resultConfigResourceHandler.GatherResourcesForMarkSession(markSessionModel);
                        break;
                    case "simPlan":
                        markSessionModel =
                            await _simPlanResourceHandler.GatherResourcesForMarkSession(markSessionModel);
                        break;
                    case "simRun":
                        markSessionModel = await _simRunResourceHandler.GatherResourcesForMarkSession(markSessionModel);
                        break;
                }
            }
            catch (Exception e)
            {
                // TODO: Start the long running process of unmarking !
                FreeResourcesAndDeleteMarkSession(markSessionModel);

                _loggerService.LogErrorEvent(e);

                return _errorService.GetStatusCodeResultForError(e);
            }
            _loggerService.LogUpdateEvent(markSessionModel.ToString());
            var markSessionForReturnDto = _mapper.Map<MarkSessionForReturnDto>(markSessionModel);

            return new OkObjectResult(markSessionForReturnDto);
        }

        public async Task<IActionResult> GetMarkSessionById(string markSessionId)
        {
            try
            {
                var markSessionModel = await FindMarkSessionById(markSessionId);
                var markSessionForReturnDto = _mapper.Map<MarkSessionForReturnDto>(markSessionModel);

                return new OkObjectResult(markSessionForReturnDto);
            }
            catch (Exception e)
            {
                _loggerService.LogErrorEvent(e);

                return _errorService.GetStatusCodeResultForError(e);
            }
        }

        public async Task<IActionResult> GetMarkSessionsByMarkSessionType(string markSessionType)
        {
            try
            {
                var markSessionModels = await _markSessionRepository.GetAllForFilter(
                    Builders<MarkSessionModel>.Filter.Where(entry => entry.MarkSessionType == markSessionType)
                );
                var markSessionsForReturnDto = _mapper.Map<List<MarkSessionForReturnDto>>(markSessionModels);

                return new OkObjectResult(markSessionsForReturnDto);
            }
            catch (Exception e)
            {
                _loggerService.LogErrorEvent(e);

                return _errorService.GetStatusCodeResultForError(e);
            }
        }

        public async Task<IActionResult> UpdateMarkSession(string markSessionId, string markSessionType)
        {
            try
            {
                var markSessionModel = await FindMarkSessionById(markSessionId);

                markSessionModel.MarkSessionType = markSessionType;
                await _markSessionRepository.Update(markSessionModel);

                return new OkResult();
            }
            catch (Exception e)
            {
                _loggerService.LogErrorEvent(e);

                return _errorService.GetStatusCodeResultForError(e);
            }
        }

        public async Task<IActionResult> DeleteMarkSession(string markSessionId)
        {
            try
            {
                var markSessionModel = await FindMarkSessionById(markSessionId);
                // TODO: FIX
                FreeResourcesAndDeleteMarkSession(markSessionModel);

                return new OkResult();
            }
            catch (Exception e)
            {
                _loggerService.LogErrorEvent(e);

                return _errorService.GetStatusCodeResultForError(e);
            }
        }

        private async Task<MarkSessionModel> FindMarkSessionById(string markSessionId)
        {
            var markSessionModel = await _markSessionRepository.GetForFilter(
                Builders<MarkSessionModel>.Filter.Eq("_id", new BsonObjectId(new ObjectId(markSessionId)))
            );

            if (markSessionModel == null)
            {
                throw new MarkSessionDoesNotExistException(
                    $"mark session with id: {markSessionId} does not exist!"
                );
            }

            return markSessionModel;
        }

        // TODO: Use something more stable for running this process than the Tasks !!!
        public async Task FreeResourcesAndDeleteMarkSession(MarkSessionModel markSessionModel)
        {
            markSessionModel.State = MarkSessionModel.AbortingState;
            await _markSessionRepository.Update(markSessionModel);
            _loggerService.LogUpdateEvent(markSessionModel.ToString());

            if (markSessionModel.SourceDependency != null)
            {
                await FreeDependantResource(markSessionModel.SourceDependency, markSessionModel.ProjectId);
                markSessionModel.SourceDependency = null;
                await _markSessionRepository.Update(markSessionModel);
            }

            var markedDependantResources = new List<DependantResourceModel>(markSessionModel.DependantResources);

            foreach (var markedResourceModel in markedDependantResources)
            {
                var unmarkedResourceModel =
                    await FreeDependantResource(markedResourceModel, markSessionModel.ProjectId);
                var index = markSessionModel.DependantResources.FindIndex(m =>
                    m.ResourceId == unmarkedResourceModel.ResourceId
                );
                markSessionModel.DependantResources.RemoveAt(index);
                await _markSessionRepository.Update(markSessionModel);
            }

            await _markSessionRepository.Delete(markSessionModel);
        }

        private async Task<DependantResourceModel> FreeDependantResource(
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