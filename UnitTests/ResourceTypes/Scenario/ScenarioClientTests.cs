using System;
using System.Net;
using System.Net.Http;
using mars_marking_svc.Exceptions;
using mars_marking_svc.ResourceTypes.Scenario;
using mars_marking_svc.ResourceTypes.Scenario.Models;
using mars_marking_svc.Services.Models;
using Moq;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.ResourceTypes.Scenario
{
    public class ScenarioClientTests
    {
        [Fact]
        public async void GetScenario_OkStatusCode_ReturnsScenarioModel()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(ScenarioModelDataMocks.MockMarkedScenarioModelJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var scenarioClient = new ScenarioClient(httpService.Object);

            // Act
            var result = await scenarioClient.GetScenario(It.IsAny<string>());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetScenario_InternalServerErrorStatusCode_ThrowsException()
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
            var scenarioClient = new ScenarioClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await scenarioClient.GetScenario(It.IsAny<string>());
            }
            catch (FailedToGetResourceException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void GetScenariosForMetadata_NotFoundStatusCode_ReturnsEmptyList()
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
            var scenarioClient = new ScenarioClient(httpService.Object);

            // Act
            var result = await scenarioClient.GetScenariosForMetadata(It.IsAny<string>());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void GetScenariosForMetadata_OkStatusCode_ReturnsScenarioModelList()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(ScenarioModelDataMocks.MockScenarioModelListJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var scenarioClient = new ScenarioClient(httpService.Object);

            // Act
            var result = await scenarioClient.GetScenariosForMetadata(It.IsAny<string>());

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void GetScenariosForProject_NoContentStatusCode_ReturnsEmptyList()
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
            var scenarioClient = new ScenarioClient(httpService.Object);

            // Act
            var result = await scenarioClient.GetScenariosForProject(It.IsAny<string>());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async void GetScenariosForProject_OkStatusCode_ReturnsScenarioModelList()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(ScenarioModelDataMocks.MockScenarioModelListJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var scenarioClient = new ScenarioClient(httpService.Object);

            // Act
            var result = await scenarioClient.GetScenariosForProject(It.IsAny<string>());

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void MarkScenario_MarkedScenarioModel_ThrowsException()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(ScenarioModelDataMocks.MockMarkedScenarioModelJson)
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var scenarioClient = new ScenarioClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await scenarioClient.MarkScenario(It.IsAny<string>());
            }
            catch (ResourceAlreadyMarkedException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void MarkScenario_ScenarioModel_ReturnsDependantResourceModel()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PatchAsync(It.IsAny<string>(), It.IsAny<ScenarioModel>()))
                .ReturnsAsync(httpResponseMessage);
            var scenarioClient = new ScenarioClient(httpService.Object);

            // Act
            var result = await scenarioClient.MarkScenario(ScenarioModelDataMocks.MockScenarioModel());

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void UnmarkScenario_NotFoundStatusCode_NoExceptionThrown()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Some error!")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PatchAsync(It.IsAny<string>(), It.IsAny<ScenarioModel>()))
                .ReturnsAsync(httpResponseMessage);
            var scenarioClient = new ScenarioClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await scenarioClient.UnmarkScenario(DependantResourceDataMocks.MockDependantResourceModel());
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void UnmarkScenario_InternalServerErrorStatusCode_ThrowsException()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Some error!")
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.PatchAsync(It.IsAny<string>(), It.IsAny<ScenarioModel>()))
                .ReturnsAsync(httpResponseMessage);
            var scenarioClient = new ScenarioClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await scenarioClient.UnmarkScenario(DependantResourceDataMocks.MockDependantResourceModel());
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