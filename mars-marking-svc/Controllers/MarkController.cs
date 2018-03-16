﻿using System.Threading.Tasks;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.Controllers
{
    [Route("api/[controller]")]
    public class MarkController : Controller
    {
        private readonly IMetadataResourceHandler _metadataResourceHandler;
        private readonly IScenarioResourceHandler _scenarioResourceHandler;
        private readonly IResultConfigResourceHandler _resultConfigResourceHandler;
        private readonly ISimPlanResourceHandler _simPlanResourceHandler;
        private readonly ISimRunResourceHandler _simRunResourceHandler;

        public MarkController(
            IMetadataResourceHandler metadataResourceHandler,
            IScenarioResourceHandler scenarioResourceHandler,
            IResultConfigResourceHandler resultConfigResourceHandler,
            ISimPlanResourceHandler simPlanResourceHandler,
            ISimRunResourceHandler simRunResourceHandler
        )
        {
            _metadataResourceHandler = metadataResourceHandler;
            _scenarioResourceHandler = scenarioResourceHandler;
            _resultConfigResourceHandler = resultConfigResourceHandler;
            _simPlanResourceHandler = simPlanResourceHandler;
            _simRunResourceHandler = simRunResourceHandler;
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
                case "projectContents":
                {
                    return BadRequest("To be implemented!");
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
                default:
                {
                    return BadRequest("resourceType is unknown!");
                }
            }
        }
    }
}