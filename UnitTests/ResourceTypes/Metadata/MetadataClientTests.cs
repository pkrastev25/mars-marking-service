using System;
using System.Net;
using System.Net.Http;
using mars_marking_svc.Exceptions;
using mars_marking_svc.ResourceTypes.Metadata;
using mars_marking_svc.Services.Models;
using Moq;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.ResourceTypes.Metadata
{
    public class MetadataClientTests
    {
        [Fact]
        public async void GetMetadata_OkStatusCode_ReturnsMetadataModel()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(MetadataModelDataMocks.MockToBeDeletedMetadataModelJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var metadataClient = new MetadataClient(httpService.Object, loggerService.Object);

            // Act
            var result = await metadataClient.GetMetadata(It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetMetadata_InternalServerErrorStatusCode_ThrowsException()
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
            var metadataClient = new MetadataClient(httpService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await metadataClient.GetMetadata(It.IsAny<string>());
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
            var metadataClient = new MetadataClient(httpService.Object, loggerService.Object);

            // Act
            var result = await metadataClient.GetMetadataForProject(It.IsAny<string>());

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
                Content = new StringContent(MarkSessionModelDataMocks.MockMarkSessionModelListJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var metadataClient = new MetadataClient(httpService.Object, loggerService.Object);

            // Act
            var result = await metadataClient.GetMetadataForProject(It.IsAny<string>());

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void MarkMetadata_ToBeDeletedMetadataModel_ThrowsException()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(MetadataModelDataMocks.MockToBeDeletedMetadataModelJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var metadataClient = new MetadataClient(httpService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await metadataClient.MarkMetadata(It.IsAny<string>());
            }
            catch (ResourceAlreadyMarkedException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void MarkMetadata_FinishedMetadataModel_ReturnsDependantResourceModel()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var metadataClient = new MetadataClient(httpService.Object, loggerService.Object);

            // Act
            var result = await metadataClient.MarkMetadata(MetadataModelDataMocks.MockFinishedMetadataModel());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void UnmarkMetadata_NotFoundStatusCode_NoExceptionThrown()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Some error!")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var metadataClient = new MetadataClient(httpService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await metadataClient.UnmarkMetadata(DependantResourceDataMocks.MockDependantResourceModel());
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void UnmarkMetadata_InternalServerErrorStatusCode_ThrowsException()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Some error!")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var metadataClient = new MetadataClient(httpService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await metadataClient.UnmarkMetadata(DependantResourceDataMocks.MockDependantResourceModel());
            }
            catch (FailedToUpdateResourceException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }
    }
}