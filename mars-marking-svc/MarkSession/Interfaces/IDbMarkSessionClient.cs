using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.Services.Models
{
    public interface IDbMarkSessionClient
    {
        Task Create(DbMarkSessionModel markSessionModel);

        Task Update(DbMarkSessionModel markSessionModel);

        Task Delete(DbMarkSessionModel markSessionModel);
    }
}