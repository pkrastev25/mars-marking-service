using mars_marking_svc.Services;
using Xunit;

namespace UnitTests.Services
{
    public class DbMongoServiceTests
    {
        [Fact]
        public void GetMarkSessionCollection_ConnectionExists_ReturnsMarkSessionCollection()
        {
            var dbMongoService = new DbMongoService();

            // Act
            var result = dbMongoService.GetMarkSessionCollection();

            // Assert
            Assert.NotNull(result);
        }
    }
}