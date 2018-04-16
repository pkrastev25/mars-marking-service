using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire;
using mars_marking_svc.DependantResource.Interfaces;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.Services.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace mars_marking_svc.ResourceTypes.MarkedResource
{
    public class MarkSessionHandler : IMarkSessionHandler
    {
        private readonly IMarkSessionRepository _markSessionRepository;
        private readonly IDependantResourceHandler _dependantResourceHandler;
        private readonly ILoggerService _loggerService;

        public MarkSessionHandler(
            IMarkSessionRepository markSessionRepository,
            IDependantResourceHandler dependantResourceHandler,
            ILoggerService loggerService
        )
        {
            _markSessionRepository = markSessionRepository;
            _dependantResourceHandler = dependantResourceHandler;
            _loggerService = loggerService;
        }

        public async Task<MarkSessionModel> CreateMarkSession(
            string resourceId,
            string projectId,
            string resourceType,
            string markSessionType
        )
        {
            var markSessionModel = new MarkSessionModel(resourceId, projectId, resourceType, markSessionType);

            try
            {
                await _markSessionRepository.Create(markSessionModel);
                await _dependantResourceHandler.GatherResourcesForMarkSession(markSessionModel);

                markSessionModel.State = MarkSessionModel.DoneState;
                await _markSessionRepository.Update(markSessionModel);
                _loggerService.LogUpdateEvent(markSessionModel.ToString());
            }
            catch (Exception e)
            {
                if (!(e is FailedToCreateMarkSessionException || e is MarkSessionAlreadyExistsException))
                {
                    await DeleteMarkSession(markSessionModel.Id.ToString());
                }

                throw;
            }

            return markSessionModel;
        }

        public async Task<MarkSessionModel> GetMarkSessionById(string markSessionId)
        {
            return await FindMarkSessionById(markSessionId);
        }

        public async Task<IEnumerable<MarkSessionModel>> GetMarkSessionsByMarkSessionType(string markSessionType)
        {
            return await _markSessionRepository.GetAllForFilter(
                Builders<MarkSessionModel>.Filter.Where(entry =>
                    entry.MarkSessionType == markSessionType && entry.State == MarkSessionModel.DoneState
                )
            );
        }

        public async Task UpdateMarkSession(string markSessionId, string markSessionType)
        {
            var markSessionModel = await FindMarkSessionById(markSessionId);

            markSessionModel.MarkSessionType = markSessionType;
            await _markSessionRepository.Update(markSessionModel);
        }

        public async Task DeleteMarkSession(string markSessionId)
        {
            await FindMarkSessionById(markSessionId);
            await Task.Run(() => { BackgroundJob.Enqueue(() => StartDeletionProcess(markSessionId)); });
        }

        public async Task StartDeletionProcess(string markSessionId)
        {
            var isMarkSessionDeleted = false;
            var taskExecutionDelayInSeconds = 1;

            while (!isMarkSessionDeleted)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(taskExecutionDelayInSeconds));

                    var markSessionModel = await FindMarkSessionById(markSessionId);

                    markSessionModel.State = MarkSessionModel.AbortingState;
                    await _markSessionRepository.Update(markSessionModel);
                    _loggerService.LogUpdateEvent(markSessionModel.ToString());

                    await _dependantResourceHandler.FreeResourcesForMarkSession(markSessionModel);

                    await _markSessionRepository.Delete(markSessionModel);
                    isMarkSessionDeleted = true;
                }
                catch (MarkSessionDoesNotExistException)
                {
                    isMarkSessionDeleted = true;
                }
                catch (Exception e)
                {
                    _loggerService.LogErrorEvent(e);
                    taskExecutionDelayInSeconds *= 2;
                }
            }
        }

        private async Task<MarkSessionModel> FindMarkSessionById(string markSessionId)
        {
            BsonObjectId bsonObjectId;

            try
            {
                bsonObjectId = new BsonObjectId(new ObjectId(markSessionId));
            }
            catch (Exception e)
            {
                throw new MarkSessionDoesNotExistException(
                    $"mark session with id: {markSessionId} does not exist!",
                    e
                );
            }

            var markSessionModel = await _markSessionRepository.GetForFilter(
                Builders<MarkSessionModel>.Filter.Eq("_id", bsonObjectId)
            );

            if (markSessionModel == null)
            {
                throw new MarkSessionDoesNotExistException(
                    $"mark session with id: {markSessionId} does not exist!"
                );
            }

            return markSessionModel;
        }
    }
}