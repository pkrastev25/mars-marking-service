using System;
using System.Net.Http;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.Scenario;
using mars_marking_svc.Services;
using Xunit;

namespace IntegrationTests.ScenarioService
{
    public class ScenarioServiceClientTests
    {
        [Fact]
        public async void GetScenario_ValidScenarioId_ReturnsScenarioModel()
        {
            // Assert
            var scenarioId = "6c40eb45-1e21-435e-81c8-895e55c6c5d8";
            var httpService = new HttpService(new HttpClient());
            var scenarioClient = new ScenarioClient(
                httpService
            );

            // Act
            var result = await scenarioClient.GetScenario(scenarioId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetScenariosForProject_ValidProjectId_ReturnsScenarioModelList()
        {
            // Assert
            var projectId = "623be379-ed40-49f3-bdd8-416f8cd0faa6";
            var httpService = new HttpService(new HttpClient());
            var scenarioClient = new ScenarioClient(
                httpService
            );

            // Act
            var result = await scenarioClient.GetScenariosForProject(projectId);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void MarkScenario_UnmarkedScenarioModel_ReturnsDependantResourceModel()
        {
            // Assert
            var scenarioId = "ae7b7512-4233-476b-b3d7-6651aeae6518";
            var httpService = new HttpService(new HttpClient());
            var scenarioClient = new ScenarioClient(
                httpService
            );

            // Act
            var result = await scenarioClient.MarkScenario(scenarioId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void UnmarkScenario_MarkedScenarioModel_NoExceptionThrown()
        {
            // Assert
            var dependantResourceModel = new DependantResourceModel(
                ResourceTypeEnum.Scenario,
                "aad965db-fceb-4ee2-bd06-c89ab182ca4c"
            );
            var httpService = new HttpService(new HttpClient());
            var scenarioClient = new ScenarioClient(
                httpService
            );
            Exception exception = null;

            try
            {
                // Act
                await scenarioClient.UnmarkScenario(dependantResourceModel);
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