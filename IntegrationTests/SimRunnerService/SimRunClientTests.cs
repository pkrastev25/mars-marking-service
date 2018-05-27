using System;
using System.Net.Http;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.SimRun;
using mars_marking_svc.Services;
using Xunit;

namespace IntegrationTests.SimRunnerService
{
    public class SimRunClientTests
    {
        [Fact]
        public async void GetSimRun_ValidSimRunIdAndProjectId_ReturnsSimRunModel()
        {
            // Arrange
            var simRunId = "5b06acef52e35100015f04da";
            var projectId = "623be379-ed40-49f3-bdd8-416f8cd0faa6";
            var httpService = new HttpService(new HttpClient());
            var simRunClient = new SimRunClient(
                httpService
            );

            // Act
            var result = await simRunClient.GetSimRun(simRunId, projectId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetSimRunsForProject_ValidProjectId_ReturnsSimRunModelList()
        {
            // Arrange
            var projectId = "623be379-ed40-49f3-bdd8-416f8cd0faa6";
            var httpService = new HttpService(new HttpClient());
            var simRunClient = new SimRunClient(
                httpService
            );

            // Act
            var result = await simRunClient.GetSimRunsForProject(projectId);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void MarkSimRun_UnmarkedSimRunModel_ReturnsDependantResourceModel()
        {
            // Arrange
            var simRunId = "5b06b85752e35100015f04dc";
            var projectId = "73fcb3bf-bc8b-4c8b-801f-8a90d92bf9c2";
            var httpService = new HttpService(new HttpClient());
            var simRunClient = new SimRunClient(
                httpService
            );

            // Act
            var result = await simRunClient.MarkSimRun(simRunId, projectId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void UnmarkSimRun_MarkedSimRunModel_NoExceptionThrown()
        {
            // Arrange
            var dependantResourceModel = new DependantResourceModel(
                ResourceTypeEnum.SimRun,
                "5b07ddd052e35100015f04e4"
            );
            var httpService = new HttpService(new HttpClient());
            var simRunClient = new SimRunClient(
                httpService
            );
            Exception exception = null;

            try
            {
                // Act
                await simRunClient.UnmarkSimRun(dependantResourceModel);
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