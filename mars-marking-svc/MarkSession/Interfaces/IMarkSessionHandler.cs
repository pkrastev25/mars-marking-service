using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.MarkSession.Interfaces
{
    public interface IMarkSessionHandler
    {
        Task<IActionResult> CreateMarkSession(
            string resourceType,
            string resourceId,
            string markSessionType,
            string projectId
        );

        Task<IActionResult> GetMarkSessionById(
            string markSessionId
        );

        Task<IActionResult> GetMarkSessionsByMarkSessionType(
            string markSessionType
        );

        Task<IActionResult> UpdateMarkSession(
            string markSessionId,
            string markSessionType
        );

        Task<IActionResult> DeleteMarkSession(
            string markSessionId
        );

        Task FreeResourcesAndDeleteMarkSession(
            MarkSessionModel markSessionModel
        );
    }
}