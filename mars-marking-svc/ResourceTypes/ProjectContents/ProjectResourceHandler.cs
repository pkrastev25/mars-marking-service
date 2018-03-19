using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.MarkedResource.Interfaces;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.ProjectContents.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.ProjectContents
{
    public class ProjectResourceHandler : IProjectResourceHandler
    {
        private readonly IMetadataServiceClient _metadataServiceClient;
        private readonly IScenarioServiceClient _scenarioServiceClient;
        private readonly IResultConfigServiceClient _resultConfigServiceClient;
        private readonly ISimPlanServiceClient _simPlanServiceClient;
        private readonly ISimRunServiceClient _simRunServiceClient;
        private readonly IResultDataServiceClient _resultDataServiceClient;
        private readonly IMarkedResourceHandler _markedResourceHandler;
        private readonly ILoggerService _loggerService;

        public ProjectResourceHandler(
            IMetadataServiceClient metadataServiceClient,
            IScenarioServiceClient scenarioServiceClient,
            IResultConfigServiceClient resultConfigServiceClient,
            ISimPlanServiceClient simPlanServiceClient,
            ISimRunServiceClient simRunServiceClient,
            IResultDataServiceClient resultDataServiceClient,
            IMarkedResourceHandler markedResourceHandler,
            ILoggerService loggerService
        )
        {
            _metadataServiceClient = metadataServiceClient;
            _scenarioServiceClient = scenarioServiceClient;
            _resultConfigServiceClient = resultConfigServiceClient;
            _simPlanServiceClient = simPlanServiceClient;
            _simRunServiceClient = simRunServiceClient;
            _resultDataServiceClient = resultDataServiceClient;
            _markedResourceHandler = markedResourceHandler;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkProjectDependantResources(string projectId)
        {
            var markedResources = new List<MarkedResourceModel>();

            try
            {
                var metadataForProject = await _metadataServiceClient.GetMetadataForProject(projectId);
                foreach (var metadataModel in metadataForProject)
                {
                    markedResources.Add(
                        await _metadataServiceClient.MarkMetadata(metadataModel)
                    );
                }

                var scenariosForProject = await _scenarioServiceClient.GetScenariosForProject(projectId);
                foreach (var scenarioModel in scenariosForProject)
                {
                    markedResources.Add(
                        await _scenarioServiceClient.MarkScenario(scenarioModel)
                    );
                }

                var resultConfigsForMetadata = new List<ResultConfigModel>();
                foreach (var metadataModel in metadataForProject)
                {
                    resultConfigsForMetadata.AddRange(
                        await _resultConfigServiceClient.GetResultConfigsForMetadata(metadataModel.DataId)
                    );
                }
                // ResultConfigs obey the mark of the metadata!
                markedResources.AddRange(
                    resultConfigsForMetadata.Select(
                        resultConfigModel => new MarkedResourceModel
                        {
                            ResourceType = "resultConfig",
                            ResourceId = resultConfigModel.ConfigId
                        }
                    )
                );

                var simPlansForProject = await _simPlanServiceClient.GetSimPlansForProject(projectId);
                foreach (var simPlanModel in simPlansForProject)
                {
                    markedResources.Add(
                        await _simPlanServiceClient.MarkSimPlan(simPlanModel, projectId)
                    );
                }

                var simRunsForProject = await _simRunServiceClient.GetSimRunsForProject(projectId);
                foreach (var simRunModel in simRunsForProject)
                {
                    markedResources.Add(
                        await _simRunServiceClient.MarkSimRun(simRunModel, projectId)
                    );
                }

                foreach (var simRunModel in simRunsForProject)
                {
                    markedResources.Add(
                        await _resultDataServiceClient.MarkResultData(simRunModel)
                    );
                }

                return new OkObjectResult(markedResources);
            }
            catch (FailedToGetResourceException e)
            {
                _loggerService.LogExceptionMessage(e);
                var unused = _markedResourceHandler.UnmarkMarkedResources(markedResources, projectId);
                
                return new StatusCodeResult(503);
            }
            catch (FailedToUpdateResourceException e)
            {
                _loggerService.LogExceptionMessage(e);
                var unused = _markedResourceHandler.UnmarkMarkedResources(markedResources, projectId);
                
                return new StatusCodeResult(503);
            }
            catch (ResourceAlreadyMarkedException e)
            {
                _loggerService.LogExceptionMessage(e);
                var unused = _markedResourceHandler.UnmarkMarkedResources(markedResources, projectId);
                
                return new StatusCodeResult(503);
            }
            catch (CannotMarkResourceException e)
            {
                _loggerService.LogExceptionMessage(e);
                var unused = _markedResourceHandler.UnmarkMarkedResources(markedResources, projectId);
                
                return new StatusCodeResult(409);
            }
            catch (Exception e)
            {
                _loggerService.LogExceptionMessageWithStackTrace(e);
                var unused = _markedResourceHandler.UnmarkMarkedResources(markedResources, projectId);
                
                return new StatusCodeResult(503);
            }
        }
    }
}