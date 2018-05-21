using System;
using mars_marking_svc.Exceptions;
using mars_marking_svc.ResourceTypes.Project;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.ResourceTypes.Project
{
    public class ProjectClientTests
    {
        [Fact]
        public async void GetProject_InvalidGrpcConntection_ThrowsException()
        {
            // Arrange
            var projectId = "0";
            Exception exception = null;
            using (var projectClient = new ProjectClient())
            {
                try
                {
                    // Act
                    await projectClient.GetProject(projectId);
                }
                catch (FailedToGetResourceException e)
                {
                    exception = e;
                }
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void MarkProject_InvalidGrpcConntection_ThrowsException()
        {
            // Arrange
            var projectId = "0";
            Exception exception = null;
            using (var projectClient = new ProjectClient())
            {
                try
                {
                    // Act
                    await projectClient.MarkProject(projectId);
                }
                catch (FailedToGetResourceException e)
                {
                    exception = e;
                }
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void MarkProject_MarkedProjectModel_ThrowsException()
        {
            // Arrange
            var projectModel = ProjectModelDataMocks.MockMarkedProjectModel();
            Exception exception = null;
            using (var projectClient = new ProjectClient())
            {
                try
                {
                    // Act
                    await projectClient.MarkProject(projectModel);
                }
                catch (ResourceAlreadyMarkedException e)
                {
                    exception = e;
                }
            }

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async void UnmarkProject_InvalidGrpcConntection_ThrowsException()
        {
            // Arrange
            var dependantResourceModel = DependantResourceDataMocks.MockDependantResourceModel();
            Exception exception = null;
            using (var projectClient = new ProjectClient())
            {
                try
                {
                    // Act
                    await projectClient.UnmarkProject(dependantResourceModel);
                }
                catch (FailedToUpdateResourceException e)
                {
                    exception = e;
                }
            }

            // Assert
            Assert.NotNull(exception);
        }
    }
}