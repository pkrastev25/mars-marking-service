using System;
using System.Net.Http;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.SimPlan;
using mars_marking_svc.Services;
using Xunit;

namespace IntegrationTests.SimRunnerService
{
    public class SimPlanClientTests
    {
        [Fact]
        public async void GetSimPlan_ValidSimPlanIdAndProjectId_ReturnsSimPlanModel()
        {
            // Arrange
            var simPlanId = "5b06abe652e35100015f04d9";
            var projectId = "623be379-ed40-49f3-bdd8-416f8cd0faa6";
            var httpService = new HttpService(new HttpClient());
            var simPlanClient = new SimPlanClient(
                httpService
            );

            // Act
            var result = await simPlanClient.GetSimPlan(simPlanId, projectId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetSimPlansForProject_ValidProjectId_ReturnsSimPlanModelList()
        {
            // Arrange
            var projectId = "623be379-ed40-49f3-bdd8-416f8cd0faa6";
            var httpService = new HttpService(new HttpClient());
            var simPlanClient = new SimPlanClient(
                httpService
            );

            // Act
            var result = await simPlanClient.GetSimPlansForProject(projectId);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void MarkSimPlan_UnmarkedSimPlanModel_ReturnsDependantResourceModel()
        {
            // Arrange
            var simPlanId = "5b06b85652e35100015f04db";
            var projectId = "73fcb3bf-bc8b-4c8b-801f-8a90d92bf9c2";
            var httpService = new HttpService(new HttpClient());
            var simPlanClient = new SimPlanClient(
                httpService
            );

            // Act
            var result = await simPlanClient.MarkSimPlan(simPlanId, projectId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void UnmarkSimPlan_MarkedSimPlanModel_NoExceptionThrown()
        {
            // Arrange
            var dependantResourceModel = new DependantResourceModel(
                ResourceTypeEnum.SimPlan,
                "5b07ddcf52e35100015f04e3"
            );
            var projectId = "be69cb8c-45e4-4d80-8d55-419984aa2151";
            var httpService = new HttpService(new HttpClient());
            var simPlanClient = new SimPlanClient(
                httpService
            );
            Exception exception = null;

            try
            {
                // Act
                await simPlanClient.UnmarkSimPlan(dependantResourceModel, projectId);
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