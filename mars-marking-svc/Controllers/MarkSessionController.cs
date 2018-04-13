using System.Threading.Tasks;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.Utils;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Controllers
{
    [Route("api/[controller]")]
    public class MarkSessionController : Controller
    {
        private readonly IMarkSessionHandler _markSessionHandler;

        public MarkSessionController(
            IMarkSessionHandler markSessionHandler
        )
        {
            _markSessionHandler = markSessionHandler;
        }

        [HttpPost("{resourceType}/{resourceId}")]
        public async Task<IActionResult> CreateMarkSession(
            string resourceType,
            string resourceId,
            [FromQuery(Name = "markSessionType")] string markSessionType,
            [FromQuery(Name = "projectId")] string projectId
        )
        {
            if (string.IsNullOrEmpty(resourceId))
            {
                return BadRequest("resourceId is not specified!");
            }

            if (!ValidateEnumUtil.DoesMarkSessionTypeExist(markSessionType))
            {
                return BadRequest("markSessionType is not specified or the type is invalid!");
            }

            if (string.IsNullOrEmpty(projectId))
            {
                if (resourceType == "project")
                {
                    projectId = resourceId;
                }
                else
                {
                    return BadRequest("projectId is not specified!");
                }
            }

            if (ValidateEnumUtil.DoesResourceTypeExist(resourceType))
            {
                return await _markSessionHandler.CreateMarkSession(
                    resourceType,
                    resourceId,
                    markSessionType,
                    projectId
                );
            }

            return BadRequest("resourceType is not specified or the type is invalid!");
        }

        [HttpGet("{markSessionId}")]
        public async Task<IActionResult> GetMarkSessionById(
            string markSessionId
        )
        {
            if (string.IsNullOrEmpty(markSessionId))
            {
                return BadRequest("markSessionId is not specified!");
            }

            return await _markSessionHandler.GetMarkSessionById(markSessionId);
        }

        [HttpGet]
        public async Task<IActionResult> GetMarkSessionsByMarkSessionType(
            [FromQuery(Name = "markSessionType")] string markSessionType
        )
        {
            if (!ValidateEnumUtil.DoesMarkSessionTypeExist(markSessionType))
            {
                return BadRequest("markSessionType is not specified or the type is invalid!");
            }

            return await _markSessionHandler.GetMarkSessionsByMarkSessionType(markSessionType);
        }

        [HttpPut("{markSessionId}")]
        public async Task<IActionResult> UpdateMarkSessionType(
            string markSessionId,
            [FromQuery(Name = "markSessionType")] string markSessionType
        )
        {
            if (string.IsNullOrEmpty(markSessionId))
            {
                return BadRequest("markSessionId is not specified!");
            }

            if (!ValidateEnumUtil.DoesMarkSessionTypeExist(markSessionType))
            {
                return BadRequest("markSessionType is not specified or the type is invalid!");
            }

            return await _markSessionHandler.UpdateMarkSession(markSessionId, markSessionType);
        }

        [HttpDelete("{markSessionId}")]
        public async Task<IActionResult> DeleteMarkSession(
            string markSessionId
        )
        {
            if (string.IsNullOrEmpty(markSessionId))
            {
                return BadRequest("markSessionId is not specified!");
            }

            return await _markSessionHandler.DeleteMarkSession(markSessionId);
        }
    }
}