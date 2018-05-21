using System;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.Services.Models;
using MongoDB.Driver;

namespace mars_marking_svc.Services
{
    public class DbMongoService : IDbMongoService
    {
        public static readonly string MongoDbConnection =
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Constants.Constants.MongoDbSvcUrlKey))
                ? "mongodb://mongodb:27017"
                : Environment.GetEnvironmentVariable(Constants.Constants.MongoDbSvcUrlKey);

        public const string MongoDbHangfireName = "hangfire-marking-svc";

        private const string MongoDbMarkingServiceName = "marking-svc";
        private const string MongoDbMarkSessionCollectionName = "mark-sessions";

        private readonly IMongoClient _mongoClient;

        public DbMongoService()
        {
            _mongoClient = new MongoClient(MongoDbConnection);
        }

        public IMongoCollection<MarkSessionModel> GetMarkSessionCollection()
        {
            return _mongoClient
                .GetDatabase(MongoDbMarkingServiceName)
                .GetCollection<MarkSessionModel>(MongoDbMarkSessionCollectionName);
        }
    }
}