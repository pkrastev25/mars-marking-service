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
            var resultDataId = "4439722e-a6d0-4f7a-9d33-0cc5a2a66da0";
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
                "c9de8a5e-1ab1-431f-a759-f44d7eef4e19"
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