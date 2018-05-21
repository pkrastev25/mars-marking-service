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
                "e580ff4f-a3b3-4252-81c4-ad88a01cac03",
                "e580ff4f-a3b3-4252-81c4-ad88a01cac03",
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
                "5b02c41fd3b6f3000710485c"
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
                "e43ee0d6-ffdf-4e5d-933a-7bd238e4fc38",
                "e43ee0d6-ffdf-4e5d-933a-7bd238e4fc38",
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
        public async void Delete_MarkSessionModelsExist_NoExceptionThrown()
        {
            // Arrange
            var markSessionModel = new MarkSessionModel(
                "184d54e3-368a-48b8-9e45-dd04ede83913",
                "184d54e3-368a-48b8-9e45-dd04ede83913",
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
            Assert.Null(exception);
        }
    }
}