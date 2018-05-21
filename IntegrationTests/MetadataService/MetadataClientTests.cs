using System;
using System.Net.Http;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.Metadata;
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
            var metadataId = "5d3d9168-5589-4d8c-9bc9-9f2f5ad75373";
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
            var projectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03";
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
            var metadataId = "b63ef2dd-05ba-4b7a-8e9a-e41047b540aa";
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
        public async void MarkMetadata_MarkedMetadataModel_NoExceptionThrown()
        {
            // Arrange
            var dependantResourceModel = new DependantResourceModel(
                ResourceTypeEnum.Metadata,
                "14574c26-7ebd-4468-8daf-89dc5058c352"
            );
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