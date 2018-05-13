using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.Services.Models;
using MongoDB.Driver;
using Moq;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.MarkSession
{
    public class MarkSessionRepositoryTests
    {
        [Fact]
        public async void Create_NewMarkSessionModel_NoExceptionThrown()
        {
            // Arrange
            var markSessionModel = MarkSessionModelDataMocks.MockMarkSessionModel();
            var markSessionModelIndexManager = new Mock<IMongoIndexManager<MarkSessionModel>>();
            markSessionModelIndexManager
                .Setup(m => m.CreateOneAsync(
                    It.IsAny<IndexKeysDefinition<MarkSessionModel>>(),
                    It.IsAny<CreateIndexOptions>(),
                    It.IsAny<CancellationToken>()
                )).ReturnsAsync("");
            var markSessionCollection = new Mock<IMongoCollection<MarkSessionModel>>();
            markSessionCollection
                .Setup(m => m.Indexes)
                .Returns(markSessionModelIndexManager.Object);
            var dbMongoService = new Mock<IDbMongoService>();
            dbMongoService
                .Setup(m => m.GetMarkSessionCollection())
                .Returns(markSessionCollection.Object);
            var loggerService = new Mock<ILoggerService>();
            var markSessionRepository = new MarkSessionRepository(dbMongoService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await markSessionRepository.Create(markSessionModel);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void Create_ExistingMarkSessionModel_ThrowsException()
        {
            // Arrange
            var markSessionModel = MarkSessionModelDataMocks.MockMarkSessionModel();
            var markSessionModelIndexManager = new Mock<IMongoIndexManager<MarkSessionModel>>();
            markSessionModelIndexManager
                .Setup(m => m.CreateOneAsync(
                    It.IsAny<IndexKeysDefinition<MarkSessionModel>>(),
                    It.IsAny<CreateIndexOptions>(),
                    It.IsAny<CancellationToken>()
                )).ReturnsAsync("");
            var markSessionCollection = new Mock<IMongoCollection<MarkSessionModel>>();
            markSessionCollection
                .Setup(m => m.Indexes)
                .Returns(markSessionModelIndexManager.Object);
            markSessionCollection
                .Setup(m => m.InsertOneAsync(
                    It.IsAny<MarkSessionModel>(),
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()
                )).Returns(Task.FromException(new Exception()));
            var dbMongoService = new Mock<IDbMongoService>();
            dbMongoService
                .Setup(m => m.GetMarkSessionCollection())
                .Returns(markSessionCollection.Object);
            var loggerService = new Mock<ILoggerService>();
            var markSessionRepository = new MarkSessionRepository(dbMongoService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await markSessionRepository.Create(markSessionModel);
            }
            catch (FailedToCreateMarkSessionException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void GetForFilter_NonExistingMarkSessionModel_ReturnsNull()
        {
            // Arrange
            var markSessionFilterDefinition = new Mock<FilterDefinition<MarkSessionModel>>();
            var markSessionModelAsyncCursor = new Mock<IAsyncCursor<MarkSessionModel>>();
            var markSessionCollection = new Mock<IMongoCollection<MarkSessionModel>>();
            markSessionCollection
                .Setup(m => m.FindAsync(
                    It.IsAny<FilterDefinition<MarkSessionModel>>(),
                    It.IsAny<FindOptions<MarkSessionModel, MarkSessionModel>>(),
                    It.IsAny<CancellationToken>()
                )).ReturnsAsync(markSessionModelAsyncCursor.Object);
            var dbMongoService = new Mock<IDbMongoService>();
            dbMongoService
                .Setup(m => m.GetMarkSessionCollection())
                .Returns(markSessionCollection.Object);
            var loggerService = new Mock<ILoggerService>();
            var markSessionRepository = new MarkSessionRepository(dbMongoService.Object, loggerService.Object);

            // Act
            var result = await markSessionRepository.GetForFilter(markSessionFilterDefinition.Object);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async void GetAll_NonExistingMarkSessionModels_ReturnsEmptyList()
        {
            // Arrange
            var markSessionModelAsyncCursor = new Mock<IAsyncCursor<MarkSessionModel>>();
            var markSessionCollection = new Mock<IMongoCollection<MarkSessionModel>>();
            markSessionCollection
                .Setup(m => m.FindAsync(
                    It.IsAny<FilterDefinition<MarkSessionModel>>(),
                    It.IsAny<FindOptions<MarkSessionModel, MarkSessionModel>>(),
                    It.IsAny<CancellationToken>()
                )).ReturnsAsync(markSessionModelAsyncCursor.Object);
            var dbMongoService = new Mock<IDbMongoService>();
            dbMongoService
                .Setup(m => m.GetMarkSessionCollection())
                .Returns(markSessionCollection.Object);
            var loggerService = new Mock<ILoggerService>();
            var markSessionRepository = new MarkSessionRepository(dbMongoService.Object, loggerService.Object);

            // Act
            var result = await markSessionRepository.GetAll();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void Update_ValidMarkSessionModel_NoExceptionThrown()
        {
            // Arrange
            var markSessionModel = MarkSessionModelDataMocks.MockMarkSessionModel();
            var markSessionCollection = new Mock<IMongoCollection<MarkSessionModel>>();
            markSessionCollection
                .Setup(m => m.ReplaceOneAsync(
                    It.IsAny<FilterDefinition<MarkSessionModel>>(),
                    It.IsAny<MarkSessionModel>(),
                    It.IsAny<UpdateOptions>(),
                    It.IsAny<CancellationToken>()
                )).ReturnsAsync(It.IsAny<ReplaceOneResult>());
            var dbMongoService = new Mock<IDbMongoService>();
            dbMongoService
                .Setup(m => m.GetMarkSessionCollection())
                .Returns(markSessionCollection.Object);
            var loggerService = new Mock<ILoggerService>();
            var markSessionRepository = new MarkSessionRepository(dbMongoService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await markSessionRepository.Update(markSessionModel);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void Delete_MarkSessionModelWithDependantResources_ThrowsException()
        {
            // Arrange
            var markSessionModel = MarkSessionModelDataMocks.MockMarkSessionModel();
            var dbMongoService = new Mock<IDbMongoService>();
            var loggerService = new Mock<ILoggerService>();
            var markSessionRepository = new MarkSessionRepository(dbMongoService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await markSessionRepository.Delete(markSessionModel);
            }
            catch (CannotDeleteMarkSessionException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void Delete_MarkSessionModelWithoutDependantResources_NoExceptionThrown()
        {
            // Arrange
            var markSessionModel = MarkSessionModelDataMocks.MockMarkSessionModel();
            markSessionModel.SourceDependency = null;
            markSessionModel.DependantResources = new List<DependantResourceModel>();
            var markSessionCollection = new Mock<IMongoCollection<MarkSessionModel>>();
            markSessionCollection
                .Setup(m => m.DeleteOneAsync(
                    It.IsAny<FilterDefinition<MarkSessionModel>>(),
                    It.IsAny<CancellationToken>()
                )).ReturnsAsync(It.IsAny<DeleteResult>());
            var dbMongoService = new Mock<IDbMongoService>();
            dbMongoService
                .Setup(m => m.GetMarkSessionCollection())
                .Returns(markSessionCollection.Object);
            var loggerService = new Mock<ILoggerService>();
            var markSessionRepository = new MarkSessionRepository(dbMongoService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await markSessionRepository.Delete(markSessionModel);
            }
            catch (CannotDeleteMarkSessionException e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }
    }
}