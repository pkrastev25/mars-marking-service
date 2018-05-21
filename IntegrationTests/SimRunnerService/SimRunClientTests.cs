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
            var simRunId = "5afd54ba95c5e500013bc74e";
            var projectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03";
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
            var projectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03";
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
            var simRunId = "5b02975b95c5e500013bc751";
            var projectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03";
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
                "5b02976395c5e500013bc752"
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