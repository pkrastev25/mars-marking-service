using System.Net.Http;
using mars_marking_svc.ResourceTypes.ResultConfig;
using mars_marking_svc.Services;
using Xunit;

namespace IntegrationTests.ResultConfigClientTests
{
    public class ResultConfigClientTests
    {
        [Fact]
        public async void GetResultConfig_ValidResultConfigId_ReturnsReultConfigModel()
        {
            // Arrange
            var resultConfigId = "b94284d47-7725-4554-9dac-b37a422b6aa1";
            var httpService = new HttpService(new HttpClient());
            var resultConfigClient = new ResultConfigClient(
                httpService
            );

            // Act
            var result = await resultConfigClient.GetResultConfig(resultConfigId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetResultConfigsForMetadata_ValidMetadataId_ReturnsReultConfigModelList()
        {
            // Arrange
            var metadataId = "5d3d9168-5589-4d8c-9bc9-9f2f5ad75373";
            var httpService = new HttpService(new HttpClient());
            var resultConfigClient = new ResultConfigClient(
                httpService
            );

            // Act
            var result = await resultConfigClient.GetResultConfigsForMetadata(metadataId);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async void CreateDependantResultConfigResource_ValidResultConfigId_ReturnsDependantResourceModel()
        {
            // Arrange
            var resultConfigId = "b4dcbee9-5e19-4117-9cb8-5d3a41016b1e";
            var httpService = new HttpService(new HttpClient());
            var resultConfigClient = new ResultConfigClient(
                httpService
            );

            // Act
            var result = await resultConfigClient.CreateDependantResultConfigResource(resultConfigId);

            // Assert
            Assert.NotNull(result);
        }
    }
}