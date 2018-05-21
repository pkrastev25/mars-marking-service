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
            var simPlanId = "5afd54b895c5e500013bc74d";
            var projectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03";
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
            var projectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03";
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
            var simPlanId = "5b028f6095c5e500013bc74f";
            var projectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03";
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
                "5b028f6795c5e500013bc750"
            );
            var projectId = "580ff4f-a3b3-4252-81c4-ad88a01cac03";
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