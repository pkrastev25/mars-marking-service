using System;
using System.Net;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Middlewares;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace UnitTests.Middlewares
{
    public class ErrorHandlerMiddlewareTests
    {
        [Fact]
        public async void Invoke_ResourceAlreadyMarkedException_ReturnsConflictStatusCode()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var loggerService = new Mock<ILoggerService>();
            var errorHandlerMiddleware = new LoggerAndErrorHandlerMiddleware(
                async innerHttpContext => await Task.FromException(new ResourceAlreadyMarkedException("")),
                loggerService.Object
            );

            // Act
            await errorHandlerMiddleware.Invoke(httpContext);

            // Asset
            Assert.Equal((int) HttpStatusCode.Conflict, httpContext.Response.StatusCode);
        }

        [Fact]
        public async void Invoke_GenericException_ReturnsInternalServerErrorStatusCode()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var loggerService = new Mock<ILoggerService>();
            var errorHandlerMiddleware = new LoggerAndErrorHandlerMiddleware(
                async innerHttpContext => await Task.FromException(new Exception("")),
                loggerService.Object
            );

            // Act
            await errorHandlerMiddleware.Invoke(httpContext);

            // Asset
            Assert.Equal((int) HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
        }
    }
}