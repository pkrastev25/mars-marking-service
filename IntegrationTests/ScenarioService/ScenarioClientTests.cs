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
            var scenarioId = "30ce974b-b743-41bb-81a3-a89cad1cf37a";
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
            var projectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03";
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
            var scenarioId = "7e41561a-21fd-4522-a645-4fd5d434650c";
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
                "f5120e41-fb2e-491a-a9b3-bf8eb3c89aff"
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