using System.Net.Http;
using mars_marking_svc.ResourceTypes.Metadata;
using mars_marking_svc.Services;
using Xunit;

namespace IntegrationTests
{
    public class MetadataClientTests
    {
        [Fact]
        public async void GetMetadata_ValidMetadataId_ReturnsMetadataModel()
        {
            // Arrange
            var httpService = new HttpService(new HttpClient());
            var loggerService = new LoggerService();
            var metadataClient = new MetadataClient(
                httpService,
                loggerService
            );

            // Act
            var result = await metadataClient.GetMetadata("5d3d9168-5589-4d8c-9bc9-9f2f5ad75373");

            // Assert
            Assert.NotNull(result);
        }
    }
}