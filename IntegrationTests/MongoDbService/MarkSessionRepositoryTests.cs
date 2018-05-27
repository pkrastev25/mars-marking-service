using System;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.MarkedResource.Enums;
using mars_marking_svc.Services;
using MongoDB.Driver;
using Xunit;

namespace IntegrationTests.MongoDbService
{
    public class MarkSessionRepositoryTests
    {
        [Fact]
        public async void Create_MarkSessionModelAlreadyExists_ThrowsException()
        {
            // Arrange
            var existingMarkSessionModel = new MarkSessionModel(
                "be69cb8c-45e4-4d80-8d55-419984aa2151",
                "be69cb8c-45e4-4d80-8d55-419984aa2151",
                ResourceTypeEnum.Project,
                MarkSessionTypeEnum.ToBeDeleted
            );
            var dbMongoService = new DbMongoService();
            var markSessionRepository = new MarkSessionRepository(dbMongoService);
            Exception exception = null;

            try
            {
                // Act
                await markSessionRepository.Create(existingMarkSessionModel);
            }
            catch (MarkSessionAlreadyExistsException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void GetForFilter_FilterIsExistingMarkSessionId_ReturnsMarkSessionModel()
        {
            // Arrange
            var filterDefinition = Builders<MarkSessionModel>.Filter.Eq(
                MarkSessionModel.BsomElementDefinitionId,
                "5b07decf7aa54a0007b3db51"
            );
            var dbMongoService = new DbMongoService();
            var markSessionRepository = new MarkSessionRepository(dbMongoService);

            // Act
            var result = await markSessionRepository.GetForFilter(filterDefinition);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetAllForFilter_FilterIsMarkSessionType_ReturnsMarkSessionModelList()
        {
            // Arrange
            var filterDefinition = Builders<MarkSessionModel>.Filter.Where(entry =>
                entry.MarkSessionType == MarkSessionTypeEnum.ToBeArchived
            );
            var dbMongoService = new DbMongoService();
            var markSessionRepository = new MarkSessionRepository(dbMongoService);

            // Act
            var result = await markSessionRepository.GetAllForFilter(filterDefinition);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void GetAll_MarkSessionModelsExist_ReturnsMarkSessionModelList()
        {
            // Arrange
            var dbMongoService = new DbMongoService();
            var markSessionRepository = new MarkSessionRepository(dbMongoService);

            // Act
            var result = await markSessionRepository.GetAll();

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void Update_MarkSessionModelsExist_NoExceptionThrown()
        {
            // Arrange
            var markSessionModel = new MarkSessionModel(
                "2085eb4c-7a94-4cc8-9c46-58f5166d3c82",
                "2085eb4c-7a94-4cc8-9c46-58f5166d3c82",
                ResourceTypeEnum.Project,
                MarkSessionTypeEnum.ToBeArchived
            );
            var dbMongoService = new DbMongoService();
            var markSessionRepository = new MarkSessionRepository(dbMongoService);
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
        public async void Delete_MissingProjectId_ExceptionThrown()
        {
            // Arrange
            var markSessionModel = new MarkSessionModel(
                "f05725ff-7da3-4dbe-83ce-184a585f47df",
                "f05725ff-7da3-4dbe-83ce-184a585f47df",
                ResourceTypeEnum.Project,
                MarkSessionTypeEnum.ToBeDeleted
            );
            var dbMongoService = new DbMongoService();
            var markSessionRepository = new MarkSessionRepository(dbMongoService);
            Exception exception = null;

            try
            {
                // Act
                await markSessionRepository.Delete(markSessionModel);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }
    }
}