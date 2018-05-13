using System;
using System.Net;
using System.Net.Http;
using mars_marking_svc.Exceptions;
using mars_marking_svc.ResourceTypes.SimRun;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;
using Moq;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.ResourceTypes.SimRun
{
    public class SimRunClientTests
    {
        [Fact]
        public async void GetSimRun_OkStatusCode_ReturnsSimRunModel()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(SimRunModelDataMocks.MockSimRunModelListJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var simRunClient = new SimRunClient(httpService.Object, loggerService.Object);

            // Act
            var result = await simRunClient.GetSimRun(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetSimRun_InternalServerErrorStatusCode_ThrowsException()
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
            var simRunClient = new SimRunClient(httpService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await simRunClient.GetSimRun(It.IsAny<string>(), It.IsAny<string>());
            }
            catch (FailedToGetResourceException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void GetSimRunsForSimPlan_NotFoundStatusCode_ReturnsEmptyList()
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
            var simRunClient = new SimRunClient(httpService.Object, loggerService.Object);

            // Act
            var result = await simRunClient.GetSimRunsForSimPlan(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void GetSimRunsForSimPlan_OkStatusCode_ReturnsSimRunModelList()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(SimRunModelDataMocks.MockSimRunModelListJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var simRunClient = new SimRunClient(httpService.Object, loggerService.Object);

            // Act
            var result = await simRunClient.GetSimRunsForSimPlan(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void GetSimRunsForProject_NotFoundStatusCode_ReturnsEmptyList()
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
            var simRunClient = new SimRunClient(httpService.Object, loggerService.Object);

            // Act
            var result = await simRunClient.GetSimRunsForProject(It.IsAny<string>());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void GetSimRunsForProject_OkStatusCode_ReturnsSimRunModelList()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(SimPlanModelDataMocks.MockSimPlanModelListJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var simRunClient = new SimRunClient(httpService.Object, loggerService.Object);

            // Act
            var result = await simRunClient.GetSimRunsForProject(It.IsAny<string>());

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void MarkSimRun_MarkedSimRunModel_ThrowsException()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(SimPlanModelDataMocks.MockMarkedSimPlanModelJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            httpService
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<SimRunModel>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var simRunClient = new SimRunClient(httpService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await simRunClient.MarkSimRun(It.IsAny<string>(), It.IsAny<string>());
            }
            catch (ResourceAlreadyMarkedException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void MarkSimRun_SimRunModel_ReturnsDependantResourceModel()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<SimRunMarkUpdateModel>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var simRunClient = new SimRunClient(httpService.Object, loggerService.Object);

            // Act
            var result = await simRunClient.MarkSimRun(SimRunModelDataMocks.MockSimRunModel(), It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void UnmarkSimRun_OkStatusCode_NoExceptionThrown()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(SimPlanModelDataMocks.MockMarkedSimPlanModelJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<SimRunMarkUpdateModel>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var simRunClient = new SimRunClient(httpService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await simRunClient.UnmarkSimRun(DependantResourceDataMocks.MockDependantResourceModel());
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void UnmarkSimRun_InternalServerErrorStatusCode_ExceptionThrown()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<SimRunMarkUpdateModel>()))
                .ReturnsAsync(httpResponseMessage);
            var loggerService = new Mock<ILoggerService>();
            var simRunClient = new SimRunClient(httpService.Object, loggerService.Object);
            Exception exception = null;

            try
            {
                // Act
                await simRunClient.UnmarkSimRun(DependantResourceDataMocks.MockDependantResourceModel());
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