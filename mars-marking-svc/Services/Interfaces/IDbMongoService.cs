using mars_marking_svc.MarkedResource.Models;
using MongoDB.Driver;

namespace mars_marking_svc.Services.Models
{
    public interface IDbMongoService
    {
        IMongoCollection<MarkSessionModel> GetMarkSessionCollection();
    }
}