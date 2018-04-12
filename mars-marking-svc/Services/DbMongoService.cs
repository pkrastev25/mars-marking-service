using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.Services.Models;
using MongoDB.Driver;

namespace mars_marking_svc.Services
{
    public class DbMongoService : IDbMongoService
    {
        private readonly IMongoClient _mongoClient;

        public DbMongoService()
        {
            _mongoClient = new MongoClient("mongodb://mongodb:27017");
        }

        public IMongoCollection<MarkSessionModel> GetMarkSessionCollection()
        {
            return _mongoClient
                .GetDatabase("marked-resources")
                .GetCollection<MarkSessionModel>("mark-session");
        }
    }
}