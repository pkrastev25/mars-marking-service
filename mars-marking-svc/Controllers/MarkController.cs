using System.Threading.Tasks;
using mars_marking_svc.Clients.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Controllers
{
    [Route("api/[controller]")]
    public class MarkController : Controller
    {
        private readonly IMetadataServiceClient _metadataServiceClient;

        public MarkController(IMetadataServiceClient metadataServiceClient)
        {
            _metadataServiceClient = metadataServiceClient;
        }

        [HttpGet("{resourceType}/{resourceId}")]
        public async Task<IActionResult> MarkResources(string resourceType, string resourceId)
        {
            switch (resourceType)
            {
                case "project_contents":
                {
                    var result = await _metadataServiceClient.GetMetadataForProject(resourceId);

                    return result.Count > 0 ? (IActionResult) Ok(result) : BadRequest();
                }
                case "metadata":
                {
                    var result = await _metadataServiceClient.MarkMetadata(resourceId);

                    return Ok(result);
                }
                default:
                {
                    return BadRequest("Specified resource resourceType is unknown!");
                }
            }
        }
    }
}