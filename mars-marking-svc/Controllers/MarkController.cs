using System.Threading.Tasks;
using mars_marking_svc.Clients.Metadata;
using mars_marking_svc.Clients.Scenario;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Controllers
{
    [Route("api/[controller]")]
    public class MarkController : Controller
    {
        private readonly IMetadataServiceClient _metadataServiceClient;
        private readonly IScenarioServiceClient _scenarioServiceClient;

        public MarkController(
            IMetadataServiceClient metadataServiceClient,
            IScenarioServiceClient scenarioServiceClient
        )
        {
            _metadataServiceClient = metadataServiceClient;
            _scenarioServiceClient = scenarioServiceClient;
        }

        [HttpGet("{resourceType}/{resourceId}")]
        public async Task<IActionResult> MarkResources(string resourceType, string resourceId)
        {
            switch (resourceType)
            {
                case "project_contents":
                {
                    //var metadataResult = await _metadataServiceClient.GetMetadataForProject(resourceId);
                    var scenarioResult = await _scenarioServiceClient.GetScenariosForProject(resourceId);

                    return Ok(scenarioResult);
                }
                case "metadata":
                {
                    var result = await _metadataServiceClient.MarkMetadata(resourceId);

                    return Ok(result);
                }
                case "scenario":
                {
                    var result = await _scenarioServiceClient.MarkScenario(resourceId);

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