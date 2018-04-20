using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Controllers
{
    [Route("healthz")]
    public class HealthController : Controller
    {
        public IActionResult HealthCheck()
        {
            return Ok(
                "marking-svc is currently running!"
            );
        }
    }
}