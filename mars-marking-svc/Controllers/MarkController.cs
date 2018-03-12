using System.Threading.Tasks;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using mars_marking_svc.ResourceTypes.Scenario.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Controllers
{
    [Route("api/[controller]")]
    public class MarkController : Controller
    {
        private readonly IMetadataResourceHandlerService _metadataResourceHandlerService;
        private readonly IScenarioResourceHandlerService _scenarioResourceHandlerService;

        public MarkController(
            IMetadataResourceHandlerService metadataResourceHandlerService,
            IScenarioResourceHandlerService scenarioResourceHandlerService
        )
        {
            _metadataResourceHandlerService = metadataResourceHandlerService;
            _scenarioResourceHandlerService = scenarioResourceHandlerService;
        }

        [HttpGet("{resourceType}/{resourceId}")]
        public async Task<IActionResult> MarkResources(string resourceType, string resourceId)
        {
            switch (resourceType)
            {
                case "project_contents":
                {
                    return BadRequest("To be implemented!");
                }
                case "metadata":
                {
                    return await _metadataResourceHandlerService.MarkMetadataDependantResources(resourceId);
                }
                case "scenario":
                {
                    return await _scenarioResourceHandlerService.MarkScenarioDependantResources(resourceId);
                }
                default:
                {
                    return BadRequest("Specified resource resourceType is unknown!");
                }
            }
        }
    }
}