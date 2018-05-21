using mars_marking_svc.Services;
using Xunit;

namespace IntegrationTests.MongoDbService
{
    public class DbMongoServiceTestscs
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