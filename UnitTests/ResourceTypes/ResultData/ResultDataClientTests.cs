using System;
using System.Net;
using System.Net.Http;
using mars_marking_svc.Exceptions;
using mars_marking_svc.ResourceTypes.ResultData;
using mars_marking_svc.Services.Models;
using Moq;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.ResourceTypes.ResultData
{
    public class ResultDataClientTests
    {
        [Fact]
        public async void MarkResultData_OkStatusCode_ReturnsDependantResourceModel()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var resultDataClient = new ResultDataClient(httpService.Object);

            // Act
            var result = await resultDataClient.MarkResultData(It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void MarkResultData_ConflictStatusCode_ThrowsException()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Conflict,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PostAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var resultDataClient = new ResultDataClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await resultDataClient.MarkResultData(It.IsAny<string>());
            }
            catch (ResourceAlreadyMarkedException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void UnmarkResultData_OkStatusCode_NoExceptionThrown()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.DeleteAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var resultDataClient = new ResultDataClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await resultDataClient.UnmarkResultData(DependantResourceDataMocks.MockDependantResourceModel());
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void UnmarkResultData_InternalServerErrorStatusCode_NoExceptionThrown()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.DeleteAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var resultDataClient = new ResultDataClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await resultDataClient.UnmarkResultData(DependantResourceDataMocks.MockDependantResourceModel());
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