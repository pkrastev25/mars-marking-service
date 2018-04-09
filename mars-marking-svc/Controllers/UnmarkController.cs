using System.Threading.Tasks;
using mars_marking_svc.MarkSession.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Controllers
{
    [Route("api/[controller]")]
    public class UnmarkController : Controller
    {
        private readonly IDbMarkSessionHandler _dbMarkSessionHandler;

        public UnmarkController(
            IDbMarkSessionHandler dbMarkSessionHandler
        )
        {
            _dbMarkSessionHandler = dbMarkSessionHandler;
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

            return await _dbMarkSessionHandler.UnmarkResourcesForMarkSession(resourceId);
        }
    }
}