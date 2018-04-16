using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.ResourceTypes.MarkedResource.Dtos;
using mars_marking_svc.Utils;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Controllers
{
    [Route("api/[controller]")]
    public class MarkSessionController : Controller
    {
        private readonly IMarkSessionHandler _markSessionHandler;
        private readonly IMapper _mapper;

        public MarkSessionController(
            IMarkSessionHandler markSessionHandler,
            IMapper mapper
        )
        {
            _markSessionHandler = markSessionHandler;
            _mapper = mapper;
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

            if (!ValidateEnumUtil.DoesResourceTypeExist(resourceType))
            {
                return BadRequest("resourceType is not specified or the type is invalid!");
            }

            var createdMarkSessionModel = await _markSessionHandler.CreateMarkSession(
                resourceId,
                projectId,
                resourceType,
                markSessionType
            );

            return Ok(
                _mapper.Map<MarkSessionForReturnDto>(createdMarkSessionModel)
            );
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

            var markSessionModel = await _markSessionHandler.GetMarkSessionById(markSessionId);

            return Ok(
                _mapper.Map<MarkSessionForReturnDto>(markSessionModel)
            );
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

            var markSessionModels = await _markSessionHandler.GetMarkSessionsByMarkSessionType(markSessionType);

            return Ok(
                _mapper.Map<List<MarkSessionForReturnDto>>(markSessionModels)
            );
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

            await _markSessionHandler.UpdateMarkSession(markSessionId, markSessionType);

            return Ok();
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

            await _markSessionHandler.DeleteMarkSession(markSessionId);

            return Ok();
        }
    }
}