using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.MarkSession.Interfaces
{
    public interface IMarkSessionHandler
    {
        Task<MarkSessionModel> CreateMarkSession(
            string resourceId,
            string projectId,
            string resourceType,
            string markSessionType
        );

        Task<MarkSessionModel> GetMarkSessionById(
            string markSessionId
        );

        Task<IEnumerable<MarkSessionModel>> GetMarkSessionsByMarkSessionType(
            string markSessionType
        );

        Task UpdateMarkSessionType(
            string markSessionId,
            string markSessionType
        );

        Task<string> DeleteMarkSession(
            string markSessionId
        );

        Task DeleteEmptyMarkSession(
            string markSessionId
        );
    }
}