using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using MongoDB.Driver;

namespace mars_marking_svc.Services.Models
{
    public interface IMarkSessionRepository
    {
        Task Create(
            MarkSessionModel markSessionModel
        );

        Task<MarkSessionModel> GetForFilter(
            FilterDefinition<MarkSessionModel> filterDefinition
        );

        Task<IEnumerable<MarkSessionModel>> GetAllForFilter(
            FilterDefinition<MarkSessionModel> filterDefinition
        );

        Task<IEnumerable<MarkSessionModel>> GetAll();

        Task Update(
            MarkSessionModel markSessionModel
        );

        Task Delete(
            MarkSessionModel markSessionModel
        );
    }
}