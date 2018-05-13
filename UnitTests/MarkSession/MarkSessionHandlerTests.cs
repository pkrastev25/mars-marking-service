using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using mars_marking_svc.BackgroundJobs.Interfaces;
using mars_marking_svc.DependantResource.Interfaces;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.MarkedResource;
using mars_marking_svc.ResourceTypes.MarkedResource.Enums;
using mars_marking_svc.Services.Models;
using MongoDB.Driver;
using Moq;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.MarkSession
{
    public class MarkSessionHandlerTests
    {
        [Fact]
        public async void CreateMarkSession_NewMarkSession_ReturnsMarkSessionModel()
        {
            // Arrange
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.Create(It.IsAny<MarkSessionModel>()))
                .Returns(Task.CompletedTask);
            markSessionRepository
                .Setup(m => m.Update(It.IsAny<MarkSessionModel>()))
                .Returns(Task.CompletedTask);
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            dependantResourceHandler
                .Setup(m => m.MarkResourcesForMarkSession(It.IsAny<MarkSessionModel>()))
                .Returns(Task.CompletedTask);
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );

            // Act
            var result = await markSessionHandler.CreateMarkSession(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            );

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void CreateMarkSession_MarkSessionAlreadyExists_ThrowsException()
        {
            // Arrange
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.Create(It.IsAny<MarkSessionModel>()))
                .Returns(Task.FromException(new MarkSessionAlreadyExistsException("")));
            markSessionRepository
                .Setup(m => m.Update(It.IsAny<MarkSessionModel>()))
                .Returns(Task.CompletedTask);
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            dependantResourceHandler
                .Setup(m => m.MarkResourcesForMarkSession(It.IsAny<MarkSessionModel>()))
                .Returns(Task.CompletedTask);
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );
            Exception exception = null;

            try
            {
                // Act
                await markSessionHandler.CreateMarkSession(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                );
            }
            catch (MarkSessionAlreadyExistsException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void GetMarkSessionById_InvalidMarkSessionId_ThrowsException()
        {
            // Arrange
            var markSessionId = "";
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );
            Exception exception = null;

            try
            {
                // Act
                await markSessionHandler.GetMarkSessionById(markSessionId);
            }
            catch (MarkSessionDoesNotExistException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void GetMarkSessionById_ValidMarkSessionId_ThrowsException()
        {
            // Arrange
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.GetForFilter(It.IsAny<FilterDefinition<MarkSessionModel>>()))
                .ReturnsAsync(MarkSessionModelDataMocks.MockMarkSessionModel);
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );

            // Act
            var result = await markSessionHandler.GetMarkSessionById(markSessionId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetMarkSessionsByMarkSessionType_ValidMarkSessionType_ReturnsMarkSessionModelList()
        {
            // Arrange
            var markSessionType = MarkSessionTypeEnum.ToBeDeleted;
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.GetAllForFilter(It.IsAny<FilterDefinition<MarkSessionModel>>()))
                .ReturnsAsync(MarkSessionModelDataMocks.MockMarkSessionModelList());
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );

            // Act
            var result = await markSessionHandler.GetMarkSessionsByMarkSessionType(markSessionType);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void GetMarkSessionsByMarkSessionType_InvalidMarkSessionType_ReturnsEmptyList()
        {
            // Arrange
            var markSessionType = "";
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.GetAllForFilter(It.IsAny<FilterDefinition<MarkSessionModel>>()))
                .ReturnsAsync(new List<MarkSessionModel>());
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );

            // Act
            var result = await markSessionHandler.GetMarkSessionsByMarkSessionType(markSessionType);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void UpdateMarkSessionType_ValidMarkSessionId_ReturnsUpdatedMarkSessionModel()
        {
            // Arrange
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionType = MarkSessionTypeEnum.ToBeArchived;
            var markSessionModel = MarkSessionModelDataMocks.MockMarkSessionModel();
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.GetForFilter(It.IsAny<FilterDefinition<MarkSessionModel>>()))
                .ReturnsAsync(markSessionModel);
            markSessionRepository
                .Setup(m => m.Update(It.IsAny<MarkSessionModel>()))
                .Returns(Task.CompletedTask);
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );

            // Act
            await markSessionHandler.UpdateMarkSessionType(markSessionId, markSessionType);

            // Assert
            Assert.Equal(markSessionType, markSessionModel.MarkSessionType);
        }

        [Fact]
        public async void UpdateMarkSessionType_InvalidMarkSessionId_ThrowsException()
        {
            // Arrange
            var markSessionId = "";
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );
            Exception exception = null;

            try
            {
                // Act
                await markSessionHandler.UpdateMarkSessionType(markSessionId, It.IsAny<string>());
            }
            catch (MarkSessionDoesNotExistException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void DeleteMarkSession_ValidMarkSessionId_ReturnsUpdatedMarkSessionModel()
        {
            // Arrange
            var backgroundJobId = "1234";
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.GetForFilter(It.IsAny<FilterDefinition<MarkSessionModel>>()))
                .ReturnsAsync(MarkSessionModelDataMocks.MockMarkSessionModel);
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            backgroundJobsHandler
                .Setup(m => m.CreateBackgroundJob(It.IsAny<Expression<Func<Task>>>()))
                .ReturnsAsync(backgroundJobId);
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );

            // Act
            var result = await markSessionHandler.DeleteMarkSession(markSessionId);

            // Assert
            Assert.Equal(result, backgroundJobId);
        }

        [Fact]
        public async void DeleteMarkSession_InvalidMarkSessionId_ThrowsException()
        {
            // Arrange
            var markSessionId = "";
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );
            Exception exception = null;

            try
            {
                // Act
                await markSessionHandler.DeleteMarkSession(markSessionId);
            }
            catch (MarkSessionDoesNotExistException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void DeleteEmptyMarkSession_ValidMarkSessionId_NoExceptionThrown()
        {
            // Arrange
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.GetForFilter(It.IsAny<FilterDefinition<MarkSessionModel>>()))
                .ReturnsAsync(MarkSessionModelDataMocks.MockMarkSessionModel);
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );
            Exception exception = null;

            try
            {
                // Act
                await markSessionHandler.DeleteMarkSession(markSessionId);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void DeleteEmptyMarkSession_InvalidMarkSessionId_ThrowsException()
        {
            // Arrange
            var markSessionId = "";
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );
            Exception exception = null;

            try
            {
                // Act
                await markSessionHandler.DeleteMarkSession(markSessionId);
            }
            catch (MarkSessionDoesNotExistException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void StartDeletionProcess_ValidMarkSessionId_NoExceptionThrown()
        {
            // Arrange
            var markSessionId = "5ae86f68b90b230007d7ea34";
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.GetForFilter(It.IsAny<FilterDefinition<MarkSessionModel>>()))
                .ReturnsAsync(MarkSessionModelDataMocks.MockMarkSessionModel);
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );
            Exception exception = null;

            try
            {
                // Act
                await markSessionHandler.StartDeletionProcess(markSessionId);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void StartDeletionProcess_InvalidMarkSessionId_NoExceptionThrown()
        {
            // Arrange
            var markSessionId = "";
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            var dependantResourceHandler = new Mock<IDependantResourceHandler>();
            var backgroundJobsHandler = new Mock<IBackgroundJobsHandler>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionHandler = new MarkSessionHandler(
                markSessionRepository.Object,
                dependantResourceHandler.Object,
                backgroundJobsHandler.Object,
                loggerService.Object
            );
            Exception exception = null;

            try
            {
                // Act
                await markSessionHandler.StartDeletionProcess(markSessionId);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }
    }
}