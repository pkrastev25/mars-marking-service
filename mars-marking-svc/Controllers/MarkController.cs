using System.Threading.Tasks;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;
using mars_marking_svc.ResourceTypes.Scenario.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Controllers
{
    [Route("api/[controller]")]
    public class MarkController : Controller
    {
        private readonly IMetadataResourceHandler _metadataResourceHandler;
        private readonly IScenarioResourceHandler _scenarioResourceHandler;
        private readonly IResultConfigResourceHandler _resultConfigResourceHandler;

        public MarkController(
            IMetadataResourceHandler metadataResourceHandler,
            IScenarioResourceHandler scenarioResourceHandler,
            IResultConfigResourceHandler resultConfigResourceHandler
        )
        {
            _metadataResourceHandler = metadataResourceHandler;
            _scenarioResourceHandler = scenarioResourceHandler;
            _resultConfigResourceHandler = resultConfigResourceHandler;
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
                    return await _metadataResourceHandler.MarkMetadataDependantResources(resourceId);
                }
                case "scenario":
                {
                    return await _scenarioResourceHandler.MarkScenarioDependantResources(resourceId);
                }
                case "result-config":
                {
                    return await _resultConfigResourceHandler.MarkResultConfigDependantResources(resourceId);
                }
                default:
                {
                    return BadRequest("Specified resource resourceType is unknown!");
                }
            }
        }
    }
}