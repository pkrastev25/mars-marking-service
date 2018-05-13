using System;
using mars_marking_svc.Services;
using UnitTests._HelperMocks;
using Xunit;

namespace UnitTests.Services
{
    public class LoggerServiceTests
    {
        [Fact]
        public void LodCreateEvent_StringMessage_StringMessageIsWrittenToTheConsole()
        {
            // Arrange
            var messageToWriteToConsole = "Some message to the console!";
            var loggerService = new LoggerService();

            using (var consoleOutputHelperMocks = new ConsoleHelperMocks(Console.Out))
            {
                // Act
                loggerService.LodCreateEvent(messageToWriteToConsole);

                // Asser
                Assert.Contains(messageToWriteToConsole, consoleOutputHelperMocks.GetOuput());
            }
        }

        [Fact]
        public void LogUpdateEvent_StringMessage_StringMessageIsWrittenToTheConsole()
        {
            // Arrange
            var messageToWriteToConsole = "Some message to the console!";
            var loggerService = new LoggerService();

            using (var consoleOutputHelperMocks = new ConsoleHelperMocks(Console.Out))
            {
                // Act
                loggerService.LogUpdateEvent(messageToWriteToConsole);

                // Asser
                Assert.Contains(messageToWriteToConsole, consoleOutputHelperMocks.GetOuput());
            }
        }

        [Fact]
        public void LogDeleteEvent_StringMessage_StringMessageIsWrittenToTheConsole()
        {
            // Arrange
            var messageToWriteToConsole = "Some message to the console!";
            var loggerService = new LoggerService();

            using (var consoleOutputHelperMocks = new ConsoleHelperMocks(Console.Out))
            {
                // Act
                loggerService.LogDeleteEvent(messageToWriteToConsole);

                // Asser
                Assert.Contains(messageToWriteToConsole, consoleOutputHelperMocks.GetOuput());
            }
        }

        [Fact]
        public void LogMarkEvent_StringMessage_StringMessageIsWrittenToTheConsole()
        {
            // Arrange
            var messageToWriteToConsole = "Some message to the console!";
            var loggerService = new LoggerService();

            using (var consoleOutputHelperMocks = new ConsoleHelperMocks(Console.Out))
            {
                // Act
                loggerService.LogMarkEvent(messageToWriteToConsole);

                // Asser
                Assert.Contains(messageToWriteToConsole, consoleOutputHelperMocks.GetOuput());
            }
        }

        [Fact]
        public void LogUnmarkEvent_StringMessage_StringMessageIsWrittenToTheConsole()
        {
            // Arrange
            var messageToWriteToConsole = "Some message to the console!";
            var loggerService = new LoggerService();

            using (var consoleOutputHelperMocks = new ConsoleHelperMocks(Console.Out))
            {
                // Act
                loggerService.LogUnmarkEvent(messageToWriteToConsole);

                // Asser
                Assert.Contains(messageToWriteToConsole, consoleOutputHelperMocks.GetOuput());
            }
        }

        [Fact]
        public void LogSkipEvent_StringMessage_StringMessageIsWrittenToTheConsole()
        {
            // Arrange
            var messageToWriteToConsole = "Some message to the console!";
            var loggerService = new LoggerService();

            using (var consoleOutputHelperMocks = new ConsoleHelperMocks(Console.Out))
            {
                // Act
                loggerService.LogSkipEvent(messageToWriteToConsole);

                // Asser
                Assert.Contains(messageToWriteToConsole, consoleOutputHelperMocks.GetOuput());
            }
        }

        [Fact]
        public void LogErrorEvent_StringMessage_StringMessageIsWrittenToTheConsole()
        {
            // Arrange
            var messageToWriteToConsole = "Some error has occurred!";
            var loggerService = new LoggerService();

            using (var consoleOutputHelperMocks = new ConsoleHelperMocks(Console.Error))
            {
                // Act
                loggerService.LogErrorEvent(new Exception(messageToWriteToConsole));

                // Asser
                Assert.Contains(messageToWriteToConsole, consoleOutputHelperMocks.GetOuput());
            }
        }
    }
}