using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using mars_marking_svc.BackgroundJobs.Interfaces;
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
        private const int MaxDelayForJobInSeconds = 60;

        private readonly IMarkSessionRepository _markSessionRepository;
        private readonly IDependantResourceHandler _dependantResourceHandler;
        private readonly IBackgroundJobsHandler _backgroundJobsHandler;
        private readonly ILoggerService _loggerService;

        public MarkSessionHandler(
            IMarkSessionRepository markSessionRepository,
            IDependantResourceHandler dependantResourceHandler,
            IBackgroundJobsHandler backgroundJobsHandler,
            ILoggerService loggerService
        )
        {
            _markSessionRepository = markSessionRepository;
            _dependantResourceHandler = dependantResourceHandler;
            _backgroundJobsHandler = backgroundJobsHandler;
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
                await _dependantResourceHandler.MarkResourcesForMarkSession(markSessionModel);

                markSessionModel.State = MarkSessionModel.StateComplete;
                await _markSessionRepository.Update(markSessionModel);
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

        public async Task<MarkSessionModel> GetMarkSessionById(
            string markSessionId
        )
        {
            return await FindMarkSessionById(markSessionId);
        }

        public async Task<IEnumerable<MarkSessionModel>> GetMarkSessionsByMarkSessionType(
            string markSessionType
        )
        {
            return await _markSessionRepository.GetAllForFilter(
                Builders<MarkSessionModel>.Filter.Where(entry =>
                    entry.MarkSessionType == markSessionType && entry.State == MarkSessionModel.StateComplete
                )
            );
        }

        public async Task UpdateMarkSessionType(
            string markSessionId,
            string markSessionType
        )
        {
            var markSessionModel = await FindMarkSessionById(markSessionId);

            markSessionModel.MarkSessionType = markSessionType;
            await _markSessionRepository.Update(markSessionModel);
        }

        public async Task<string> DeleteMarkSession(
            string markSessionId
        )
        {
            await FindMarkSessionById(markSessionId);

            return await _backgroundJobsHandler.CreateBackgroundJob(
                () => StartDeletionProcess(markSessionId)
            );
        }

        public async Task DeleteEmptyMarkSession(
            string markSessionId
        )
        {
            var markSessionModel = await FindMarkSessionById(markSessionId);
            markSessionModel.DependantResources = new List<DependantResourceModel>();
            await _dependantResourceHandler.UnmarkResourcesForMarkSession(markSessionModel);

            await _markSessionRepository.Delete(markSessionModel);
        }

        public async Task StartDeletionProcess(
            string markSessionId
        )
        {
            var isMarkSessionDeleted = false;
            var taskExecutionDelayInSeconds = 1;
            var restartCount = 0;
            var stopwatch = new Stopwatch();

            while (!isMarkSessionDeleted)
            {
                try
                {
                    _loggerService.LogBackgroundJobInfoEvent(
                        $"Unmarking job for mark session with id: {markSessionId} will start in {taskExecutionDelayInSeconds} second/s, restart count: {restartCount}"
                    );
                    await Task.Delay(TimeSpan.FromSeconds(taskExecutionDelayInSeconds));
                    stopwatch.Start();

                    var markSessionModel = await FindMarkSessionById(markSessionId);

                    markSessionModel.State = MarkSessionModel.StateUnmarking;
                    await _markSessionRepository.Update(markSessionModel);
                    await _dependantResourceHandler.UnmarkResourcesForMarkSession(markSessionModel);
                    await _markSessionRepository.Delete(markSessionModel);

                    isMarkSessionDeleted = true;
                    stopwatch.Stop();
                }
                catch (MarkSessionDoesNotExistException)
                {
                    stopwatch.Stop();
                    isMarkSessionDeleted = true;
                }
                catch (Exception e)
                {
                    stopwatch.Stop();
                    _loggerService.LogBackgroundJobErrorEvent(stopwatch.Elapsed.TotalSeconds, e);
                    taskExecutionDelayInSeconds = taskExecutionDelayInSeconds * 2 % MaxDelayForJobInSeconds;
                    restartCount++;
                }
            }

            _loggerService.LogBackgroundJobInfoEvent(
                stopwatch.Elapsed.TotalSeconds,
                $"Unmarking job for mark session with id: {markSessionId} completed!"
            );
        }

        private async Task<MarkSessionModel> FindMarkSessionById(
            string markSessionId
        )
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
                Builders<MarkSessionModel>.Filter.Eq(MarkSessionModel.BsomElementDefinitionId, bsonObjectId)
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