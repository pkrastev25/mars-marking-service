using System;
using mars_marking_svc.Constants;
using mars_marking_svc.Utils;
using UnitTests._DataMocks;
using Xunit;

namespace UnitTests.Extensions
{
    public class MarkSessionExtensionsTests
    {
        [Fact]
        public void IsMarkSessionRecentlyUpdated_RecentlyUpdatedMarkSession_ReturnsTrue()
        {
            // Arrange
            var markSessionModel = MarkSessionModelDataMocks.MockMarkSessionModel();

            // Act
            var result = markSessionModel.IsMarkSessionRecentlyUpdated();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsMarkSessionRecentlyUpdated_NotRecentlyUpdatedMarkSession_ReturnsFalse()
        {
            // Arrange
            var markSessionModel = MarkSessionModelDataMocks.MockMarkSessionModel();
            markSessionModel.LatestUpdateTimestampInTicks =
                DateTime.Now.Ticks - 2 * Constants.MarkSessionUpdateReferenceTimeInTicks;

            // Act
            var result = markSessionModel.IsMarkSessionRecentlyUpdated();

            // Assert
            Assert.False(result);
        }
    }
}