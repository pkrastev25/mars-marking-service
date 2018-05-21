using System;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.Project;
using mars_marking_svc.ResourceTypes.Project.Models;
using Xunit;

namespace IntegrationTests.ProjectService
{
    public class ProjectClientTests
    {
        [Fact]
        public async void GetProject_InvalidProjectId_ThrowsException()
        {
            // Arrange
            var projectId = "0";
            var projectClient = new ProjectClient();
            Exception exception = null;

            try
            {
                // Act
                await projectClient.GetProject(projectId);
            }
            catch (FailedToGetResourceException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void MarkProject_InvalidProjectModel_ThrowsException()
        {
            // Arrange
            var projectModel = new ProjectModel(
                "0",
                false
            );
            var projectClient = new ProjectClient();
            Exception exception = null;

            try
            {
                // Act
                await projectClient.MarkProject(projectModel);
            }
            catch (FailedToUpdateResourceException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void UnmarkProject_InvalidDependantResourceModel_ThrowsException()
        {
            // Arrange
            var dependantResourceModel = new DependantResourceModel(
                ResourceTypeEnum.Project,
                "0"
            );
            var projectClient = new ProjectClient();
            Exception exception = null;

            try
            {
                // Act
                await projectClient.UnmarkProject(dependantResourceModel);
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