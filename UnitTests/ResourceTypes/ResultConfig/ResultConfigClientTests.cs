using System;
using System.Net;
using System.Net.Http;
using mars_marking_svc.Exceptions;
using mars_marking_svc.ResourceTypes.ResultConfig;
using mars_marking_svc.Services.Models;
using Moq;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.ResourceTypes.ResultConfig
{
    public class ResultConfigClientTests
    {
        [Fact]
        public async void GetResultConfig_OkStatusCode_ReturnsResultConfigModel()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(ResultConfigModelDataMocks.MockResultConfigResponseModelJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var resultConfigClient = new ResultConfigClient(httpService.Object, loggerService.Object);

            // Act
            var result = await resultConfigClient.GetResultConfig(It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetResultConfig_InternalServerErrorStatusCode_ThrowsException()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var resultConfigClient = new ResultConfigClient(httpService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await resultConfigClient.GetResultConfig(It.IsAny<string>());
            }
            catch (FailedToGetResourceException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void GetMetadataForProject_NotFoundStatusCode_ReturnsEmptyList()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var resultConfigClient = new ResultConfigClient(httpService.Object, loggerService.Object);

            // Act
            var result = await resultConfigClient.GetResultConfigsForMetadata(It.IsAny<string>());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void GetMetadataForProject_OkStatusCode_ReturnsMarkSessionModelList()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(ResultConfigModelDataMocks.MockResultConfigResponseModelListJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var resultConfigClient = new ResultConfigClient(httpService.Object, loggerService.Object);

            // Act
            var result = await resultConfigClient.GetResultConfigsForMetadata(It.IsAny<string>());

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void CreateDependantResultConfigResource_ValidResultConfiId_ReturnsDependantResourceModel()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(ResultConfigModelDataMocks.MockResultConfigResponseModelJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var resultConfigClient = new ResultConfigClient(httpService.Object, loggerService.Object);

            // Act
            var result = await resultConfigClient.CreateDependantResultConfigResource(It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void CreateDependantResultConfigResource_ValidResultConfigModel_ReturnsDependantResourceModel()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(MetadataModelDataMocks.MockFinishedMetadataModelJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            httpService
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var resultConfigClient = new ResultConfigClient(httpService.Object, loggerService.Object);

            // Act
            var result = await resultConfigClient.CreateDependantResultConfigResource(
                ResultConfigModelDataMocks.MockResultConfigModel()
            );

            // Assert
            Assert.NotNull(result);
        }
    }
}