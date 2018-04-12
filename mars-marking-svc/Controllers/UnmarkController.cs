using System.Threading.Tasks;
using mars_marking_svc.MarkSession.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Controllers
{
    [Route("api/[controller]")]
    public class UnmarkController : Controller
    {
        private readonly IMarkSessionHandler _markSessionHandler;

        public UnmarkController(
            IMarkSessionHandler markSessionHandler
        )
        {
            _markSessionHandler = markSessionHandler;
        }

        [HttpDelete("{resourceId}")]
        public async Task<IActionResult> DeleteMarkSession(
            string resourceId
        )
        {
            if (string.IsNullOrEmpty(resourceId))
            {
                return BadRequest("resourceId is not specified!");
            }

            return await _markSessionHandler.UnmarkResourcesForMarkSession(resourceId);
        }
    }
}