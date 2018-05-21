using System;
using System.Net;
using System.Net.Http;
using mars_marking_svc.Exceptions;
using mars_marking_svc.ResourceTypes.SimPlan;
using mars_marking_svc.ResourceTypes.SimPlan.Models;
using mars_marking_svc.Services.Models;
using Moq;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.ResourceTypes.SimPlan
{
    public class SimPlanClientTests
    {
        [Fact]
        public async void GetSimPlan_OkStatusCode_ReturnsSimPlanModel()
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
            var simPlanClient = new SimPlanClient(httpService.Object);

            // Act
            var result = await simPlanClient.GetSimPlan(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetSimPlan_InternalServerErrorStatusCode_ThrowsException()
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
            var simPlanClient = new SimPlanClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await simPlanClient.GetSimPlan(It.IsAny<string>(), It.IsAny<string>());
            }
            catch (FailedToGetResourceException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void GetSimPlansForScenario_NotFoundStatusCode_ReturnsEmptyList()
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
            var simPlanClient = new SimPlanClient(httpService.Object);

            // Act
            var result = await simPlanClient.GetSimPlansForScenario(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void GetSimPlansForScenario_OkStatusCode_ReturnsSimPlanModelList()
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
            var simPlanClient = new SimPlanClient(httpService.Object);

            // Act
            var result = await simPlanClient.GetSimPlansForScenario(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void GetSimPlansForResultConfig_NoContentStatusCode_ReturnsEmptyList()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NoContent,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var simPlanClient = new SimPlanClient(httpService.Object);

            // Act
            var result = await simPlanClient.GetSimPlansForResultConfig(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void GetSimPlansForResultConfig_OkStatusCode_ReturnsSimPlanModelList()
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
            var simPlanClient = new SimPlanClient(httpService.Object);

            // Act
            var result = await simPlanClient.GetSimPlansForResultConfig(It.IsAny<string>(), It.IsAny<string>());

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void GetSimPlansForProject_NotFoundStatusCode_ReturnsEmptyList()
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
            var simPlanClient = new SimPlanClient(httpService.Object);

            // Act
            var result = await simPlanClient.GetSimPlansForProject(It.IsAny<string>());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void GetSimPlansForProject_OkStatusCode_ReturnsSimPlanModelList()
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
            var simPlanClient = new SimPlanClient(httpService.Object);

            // Act
            var result = await simPlanClient.GetSimPlansForProject(It.IsAny<string>());

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void MarkSimPlan_MarkedSimPlanModel_ThrowsException()
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
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<SimPlanModel>()))
                .ReturnsAsync(httpResponseMessage);
            var simPlanClient = new SimPlanClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await simPlanClient.MarkSimPlan(It.IsAny<string>(), It.IsAny<string>());
            }
            catch (ResourceAlreadyMarkedException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void MarkSimPlan_SimPlanModel_ReturnsDependantResourceModel()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<SimPlanModel>()))
                .ReturnsAsync(httpResponseMessage);
            var simPlanClient = new SimPlanClient(httpService.Object);

            // Act
            var result = await simPlanClient.MarkSimPlan(SimPlanModelDataMocks.MockSimPlanModel(), It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void UnmarkSimPlan_OkStatusCode_NoExceptionThrown()
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
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<SimPlanModel>()))
                .ReturnsAsync(httpResponseMessage);
            var simPlanClient = new SimPlanClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await simPlanClient.UnmarkSimPlan(
                    DependantResourceDataMocks.MockDependantResourceModel(),
                    It.IsAny<string>()
                );
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void UnmarkSimPlan_InternalServerErrorStatusCode_ExceptionThrown()
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
            httpService
                .Setup(m => m.PutAsync(It.IsAny<string>(), It.IsAny<SimPlanModel>()))
                .ReturnsAsync(httpResponseMessage);
            var simPlanClient = new SimPlanClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await simPlanClient.UnmarkSimPlan(
                    DependantResourceDataMocks.MockDependantResourceModel(),
                    It.IsAny<string>()
                );
            }
            catch (FailedToGetResourceException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }
    }
}