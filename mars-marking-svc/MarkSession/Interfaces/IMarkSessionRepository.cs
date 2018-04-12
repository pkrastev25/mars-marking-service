using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.Services.Models
{
    public interface IMarkSessionRepository
    {
        Task Create(MarkSessionModel markSessionModel);

        Task<MarkSessionModel> Get(string resourceId);

        Task<IEnumerable<MarkSessionModel>> GetAll();

        Task Update(MarkSessionModel markSessionModel);

        Task Delete(MarkSessionModel markSessionModel);
    }
}