using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.Services.Models
{
    public interface IDbService
    {
        Task InsertNewMarkSession(DbMarkSessionModel markSessionModel);

        Task UpdateMarkSession(DbMarkSessionModel markSessionModel);

        Task DeleteMarkSession(DbMarkSessionModel markSessionModel);
    }
}