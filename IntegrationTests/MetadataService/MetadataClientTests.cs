using System;
using System.Net.Http;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.Metadata;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using mars_marking_svc.Services;
using Xunit;

namespace IntegrationTests.MetadataService
{
    public class MetadataClientTests
    {
        [Fact]
        public async void GetMetadata_ValidMetadataId_ReturnsMetadataModel()
        {
            // Arrange
            var metadataId = "4439722e-a6d0-4f7a-9d33-0cc5a2a66da0";
            var httpService = new HttpService(new HttpClient());
            var metadataClient = new MetadataClient(
                httpService
            );

            // Act
            var result = await metadataClient.GetMetadata(metadataId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetMetadataForProject_ValidProjectId_ReturnsMetadataModelList()
        {
            // Arrange
            var projectId = "623be379-ed40-49f3-bdd8-416f8cd0faa6";
            var httpService = new HttpService(new HttpClient());
            var metadataClient = new MetadataClient(
                httpService
            );

            // Act
            var result = await metadataClient.GetMetadataForProject(projectId);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void MarkMetadata_UnmarkedMetadataModel_ReturnsDependantResourceModel()
        {
            // Arrange
            var metadataId = "45db3205-83be-42a1-af14-6a03df9d9536";
            var httpService = new HttpService(new HttpClient());
            var metadataClient = new MetadataClient(
                httpService
            );

            // Act
            var result = await metadataClient.MarkMetadata(metadataId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void UnmarkMetadata_MarkedMetadataModel_NoExceptionThrown()
        {
            // Arrange
            var dependantResourceModel = new DependantResourceModel(
                ResourceTypeEnum.Metadata,
                "c9de8a5e-1ab1-431f-a759-f44d7eef4e19"
            )
            {
                PreviousState = MetadataModel.FinishedState
            };
            var httpService = new HttpService(new HttpClient());
            var metadataClient = new MetadataClient(
                httpService
            );
            Exception exception = null;

            try
            {
                // Act
                await metadataClient.UnmarkMetadata(dependantResourceModel);
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