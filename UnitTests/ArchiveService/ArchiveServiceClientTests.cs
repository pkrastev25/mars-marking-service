using System;
using System.Net;
using System.Net.Http;
using mars_marking_svc.ArchiveService;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Services.Models;
using Moq;
using Xunit;

namespace UnitTests.ArchiveService
{
    public class ArchiveServiceClientTests
    {
        [Fact]
        public async void EnsureArchiveRestoreIsNotRunning_NotFoundStatusCode_NoExceptionThrown()
        {
            // Arrnage
            var projectId = "be1cabd5-c121-49a0-9860-824419efb39a";
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var archiveServiceClient = new ArchiveServiceClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await archiveServiceClient.EnsureArchiveRestoreIsNotRunning(projectId);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }
        
        [Fact]
        public async void EnsureArchiveRestoreIsNotRunning_InternalServerErrorStatusCode_ThrowsException()
        {
            // Arrnage
            var projectId = "be1cabd5-c121-49a0-9860-824419efb39a";
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };
            var httpService = new Mock<IHttpService>();
            httpService
                .Setup(m => m.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(httpResponseMessage);
            var archiveServiceClient = new ArchiveServiceClient(httpService.Object);
            Exception exception = null;

            try
            {
                // Act
                await archiveServiceClient.EnsureArchiveRestoreIsNotRunning(projectId);
            }
            catch (FailedToGetResourceException e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
        }
    }
}