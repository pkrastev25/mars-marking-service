﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Exceptions;
using mars_marking_svc.Models;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;
using mars_marking_svc.ResourceTypes.Scenario.Models;
using mars_marking_svc.ResourceTypes.SimPlan.Models;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;

namespace mars_marking_svc.ResourceTypes.Metadata
{
    public class MetadataResourceHandler : IMetadataResourceHandler
    {
        private readonly IMetadataServiceClient _metadataServiceClient;
        private readonly IScenarioServiceClient _scenarioServiceClient;
        private readonly IResultConfigServiceClient _resultConfigServiceClient;
        private readonly ISimPlanServiceClient _simPlanServiceClient;
        private readonly ILoggerService _loggerService;

        public MetadataResourceHandler(
            IMetadataServiceClient metadataServiceClient,
            IScenarioServiceClient scenarioServiceClient,
            IResultConfigServiceClient resultConfigServiceClient,
            ISimPlanServiceClient simPlanServiceClient,
            ILoggerService loggerService
        )
        {
            _metadataServiceClient = metadataServiceClient;
            _scenarioServiceClient = scenarioServiceClient;
            _resultConfigServiceClient = resultConfigServiceClient;
            _simPlanServiceClient = simPlanServiceClient;
            _loggerService = loggerService;
        }

        public async Task<IActionResult> MarkMetadataDependantResources(string metadataId, string projectId)
        {
            var markedResources = new ArrayList<MarkedResourceModel>();

            try
            {
                var sourceMetadata = await _metadataServiceClient.MarkMetadata(metadataId);
                markedResources.Add(sourceMetadata);

                var scenariosForMetadata = await _scenarioServiceClient.GetScenariosForMetadata(metadataId);
                foreach (var scenarioModel in scenariosForMetadata)
                {
                    markedResources.Add(
                        await _scenarioServiceClient.MarkScenario(scenarioModel)
                    );
                }

                var resultConfigsForMetadata = await _resultConfigServiceClient.GetResultConfigsForMetadata(metadataId);
                foreach (var resultConfigModel in resultConfigsForMetadata)
                {
                    // The result configs obey the metadata mark!
                    var markedResultConfig = new MarkedResourceModel
                    {
                        resourceType = "result-config",
                        resourceId = resultConfigModel.ConfigId
                    };
                    _loggerService.LogMarkedResource(markedResultConfig);
                    markedResources.Add(markedResultConfig);
                }

                var simPlansForScenarios = new List<SimPlanModel>();
                foreach (var scenarioModel in scenariosForMetadata)
                {
                    simPlansForScenarios.AddRange(
                        await _simPlanServiceClient.GetSimPlansForScenario(scenarioModel.ScenarioId, projectId)
                    );
                }
                foreach (var simPlanModel in simPlansForScenarios)
                {
                    markedResources.Add(
                        await _simPlanServiceClient.MarkSimPlan(simPlanModel, projectId)
                    );
                }

                return new OkObjectResult(markedResources);
            }
            catch (FailedToGetResourceException e)
            {
                // TODO: Remove the marks
                _loggerService.LogExceptionMessage(e);
                return new StatusCodeResult(503);
            }
            catch (FailedToUpdateResourceException e)
            {
                // TODO: Remove the marks
                _loggerService.LogExceptionMessage(e);
                return new StatusCodeResult(503);
            }
            catch (ResourceAlreadyMarkedException e)
            {
                // TODO: Remove the marks
                _loggerService.LogExceptionMessage(e);
                return new StatusCodeResult(503);
            }
            catch (CannotMarkResourceException e)
            {
                // TODO: Remove the marks
                _loggerService.LogExceptionMessage(e);
                return new StatusCodeResult(409);
            }
            catch (Exception e)
            {
                // TODO: Remove the marks
                _loggerService.LogExceptionMessageWithStackTrace(e);
                return new StatusCodeResult(503);
            }
        }
    }
}