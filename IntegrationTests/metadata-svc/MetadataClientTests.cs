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
            var result = await metadataClient.GetMetadata("5afd545391024a0001fe8db2");

            // Assert
            Assert.NotNull(result);
        }
    }
}