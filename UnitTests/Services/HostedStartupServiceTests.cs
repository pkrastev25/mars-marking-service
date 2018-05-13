using System;
using System.Collections.Generic;
using System.Threading;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.Services;
using mars_marking_svc.Services.Models;
using Moq;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.Services
{
    public class HostedStartupServiceTests
    {
        [Fact]
        public async void StartAsync_FalseCancellationToken_NoExceptionThrown()
        {
            // Arrange
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.GetAll())
                .ReturnsAsync(
                    new List<MarkSessionModel>
                    {
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel()
                    }
                );
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            markSessionHandler
                .Setup(m => m.DeleteMarkSession(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<string>());
            var loggerService = new Mock<ILoggerService>();
            var hostedStartupService = new HostedStartupService(
                markSessionRepository.Object,
                markSessionHandler.Object,
                loggerService.Object
            );
            var cancellationToken = new CancellationToken(false);
            Exception exception = null;

            try
            {
                // Act
                await hostedStartupService.StartAsync(cancellationToken);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void StartAsync_TrueCancellationToken_NoExceptionThrown()
        {
            // Arrange
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.GetAll())
                .ReturnsAsync(
                    new List<MarkSessionModel>
                    {
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel()
                    }
                );
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            markSessionHandler
                .Setup(m => m.DeleteMarkSession(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<string>());
            var loggerService = new Mock<ILoggerService>();
            var hostedStartupService = new HostedStartupService(
                markSessionRepository.Object,
                markSessionHandler.Object,
                loggerService.Object
            );
            var cancellationToken = new CancellationToken(true);
            Exception exception = null;

            try
            {
                // Act
                await hostedStartupService.StartAsync(cancellationToken);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void StopAsync_FalseCancellationToken_NoExceptionThrown()
        {
            // Arrange
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.GetAll())
                .ReturnsAsync(
                    new List<MarkSessionModel>
                    {
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel()
                    }
                );
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            markSessionHandler
                .Setup(m => m.DeleteMarkSession(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<string>());
            var loggerService = new Mock<ILoggerService>();
            var hostedStartupService = new HostedStartupService(
                markSessionRepository.Object,
                markSessionHandler.Object,
                loggerService.Object
            );
            var cancellationToken = new CancellationToken(false);
            Exception exception = null;

            try
            {
                // Act
                await hostedStartupService.StopAsync(cancellationToken);
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async void StopAsync_TrueCancellationToken_NoExceptionThrown()
        {
            // Arrange
            var markSessionRepository = new Mock<IMarkSessionRepository>();
            markSessionRepository
                .Setup(m => m.GetAll())
                .ReturnsAsync(
                    new List<MarkSessionModel>
                    {
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel(),
                        MarkSessionModelDataMocks.MockMarkSessionModel()
                    }
                );
            var markSessionHandler = new Mock<IMarkSessionHandler>();
            markSessionHandler
                .Setup(m => m.DeleteMarkSession(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<string>());
            var loggerService = new Mock<ILoggerService>();
            var hostedStartupService = new HostedStartupService(
                markSessionRepository.Object,
                markSessionHandler.Object,
                loggerService.Object
            );
            var cancellationToken = new CancellationToken(true);
            Exception exception = null;

            try
            {
                // Act
                await hostedStartupService.StopAsync(cancellationToken);
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