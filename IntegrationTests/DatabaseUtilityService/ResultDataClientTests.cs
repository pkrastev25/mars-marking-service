using System;
using System.Net.Http;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.ResultData;
using mars_marking_svc.Services;
using Xunit;

namespace IntegrationTests.DatabaseUtilityService
{
    public class ResultDataClientTests
    {
        [Fact]
        public async void MarkResultData_UnmarkedResultDataModel_ReturnsDependantResourceModel()
        {
            // Arrange
            var resultDataId = "75d46f68-ea8c-4227-afd8-cbfb15c4da7b";
            var httpService = new HttpService(new HttpClient());
            var resultDataClient = new ResultDataClient(
                httpService
            );

            // Act
            var result = await resultDataClient.MarkResultData(resultDataId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async void UnmarkResultData_MarkedResultDataModel_NoExceptionThrown()
        {
            // Arrange
            var dependantResourceModel = new DependantResourceModel(
                ResourceTypeEnum.ResultData,
                "c36d4764-86fd-4217-b66e-84190e7460d0"
            );
            var httpService = new HttpService(new HttpClient());
            var resultDataClient = new ResultDataClient(
                httpService
            );
            Exception exception = null;

            try
            {
                // Act
                await resultDataClient.UnmarkResultData(dependantResourceModel);
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