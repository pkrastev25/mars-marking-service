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
            var resultConfigId = "2462a6b6-5e07-48b4-b3e3-b3b207d55f4e";
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
            var metadataId = "4439722e-a6d0-4f7a-9d33-0cc5a2a66da0";
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
            var resultConfigId = "2462a6b6-5e07-48b4-b3e3-b3b207d55f4e";
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