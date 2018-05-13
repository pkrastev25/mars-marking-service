using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.Services;
using MongoDB.Driver;
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
            Assert.True(result is IMongoCollection<MarkSessionModel>);
        }
    }
}