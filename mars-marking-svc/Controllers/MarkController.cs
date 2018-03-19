using System.Threading.Tasks;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.ProjectContents.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Controllers
{
    [Route("api/[controller]")]
    public class MarkController : Controller
    {
        private readonly IProjectResourceHandler _projectResourceHandler;
        private readonly IMetadataResourceHandler _metadataResourceHandler;
        private readonly IScenarioResourceHandler _scenarioResourceHandler;
        private readonly IResultConfigResourceHandler _resultConfigResourceHandler;
        private readonly ISimPlanResourceHandler _simPlanResourceHandler;
        private readonly ISimRunResourceHandler _simRunResourceHandler;
        private readonly IResultDataResourceHandler _resultDataResourceHandler;

        public MarkController(
            IProjectResourceHandler projectResourceHandler,
            IMetadataResourceHandler metadataResourceHandler,
            IScenarioResourceHandler scenarioResourceHandler,
            IResultConfigResourceHandler resultConfigResourceHandler,
            ISimPlanResourceHandler simPlanResourceHandler,
            ISimRunResourceHandler simRunResourceHandler,
            IResultDataResourceHandler resultDataResourceHandler
        )
        {
            _projectResourceHandler = projectResourceHandler;
            _metadataResourceHandler = metadataResourceHandler;
            _scenarioResourceHandler = scenarioResourceHandler;
            _resultConfigResourceHandler = resultConfigResourceHandler;
            _simPlanResourceHandler = simPlanResourceHandler;
            _simRunResourceHandler = simRunResourceHandler;
            _resultDataResourceHandler = resultDataResourceHandler;
        }

        [HttpGet("{resourceType}/{resourceId}")]
        public async Task<IActionResult> MarkResources(
            string resourceType,
            string resourceId,
            [FromQuery(Name = "projectId")] string projectId
        )
        {
            if (string.IsNullOrEmpty(resourceType))
            {
                return BadRequest("resourceType is not specified!");
            }

            if (string.IsNullOrEmpty(resourceId))
            {
                return BadRequest("resourceId is not specified!");
            }

            if (string.IsNullOrEmpty(projectId))
            {
                return BadRequest("projectId is not specified!");
            }

            switch (resourceType)
            {
                case "project":
                {
                    return await _projectResourceHandler.MarkProjectDependantResources(resourceId);
                }
                case "metadata":
                {
                    return await _metadataResourceHandler.MarkMetadataDependantResources(resourceId, projectId);
                }
                case "scenario":
                {
                    return await _scenarioResourceHandler.MarkScenarioDependantResources(resourceId, projectId);
                }
                case "resultConfig":
                {
                    return await _resultConfigResourceHandler.MarkResultConfigDependantResources(resourceId, projectId);
                }
                case "simPlan":
                {
                    return await _simPlanResourceHandler.MarkSimPlanDependantResources(resourceId, projectId);
                }
                case "simRun":
                {
                    return await _simRunResourceHandler.MarkSimRunDependantResources(resourceId, projectId);
                }
                case "resultData":
                {
                    return await _resultDataResourceHandler.MarkResultDataDependantResources(resourceId, projectId);
                }
                default:
                {
                    return BadRequest("resourceType is unknown!");
                }
            }
        }
    }
}