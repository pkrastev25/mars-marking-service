using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.ResultConfig
{
    public class ResultConfigResourceHandler : IResultConfigResourceHandler
    {
        private readonly IResultConfigServiceClient _resultConfigServiceClient;
        private readonly ISimPlanServiceClient _simPlanServiceClient;
        private readonly ISimRunServiceClient _simRunServiceClient;
        private readonly IResultDataServiceClient _resultDataServiceClient;
        private readonly IDbService _dbService;
        private readonly ILoggerService _loggerService;
        private readonly IErrorHandlerService _errorHandlerService;

        public ResultConfigResourceHandler(
            IResultConfigServiceClient resultConfigServiceClient,
            ISimPlanServiceClient simPlanServiceClient,
            ISimRunServiceClient simRunServiceClient,
            IResultDataServiceClient resultDataServiceClient,
            IDbService dbService,
            ILoggerService loggerService,
            IErrorHandlerService errorHandlerService
        )
        {
            _resultConfigServiceClient = resultConfigServiceClient;
            _simPlanServiceClient = simPlanServiceClient;
            _simRunServiceClient = simRunServiceClient;
            _resultDataServiceClient = resultDataServiceClient;
            _dbService = dbService;
            _loggerService = loggerService;
            _errorHandlerService = errorHandlerService;
        }

        public async Task<IActionResult> MarkResultConfigDependantResources(string resultConfigId, string projectId)
        {
            var markSessionModel = new DbMarkSessionModel(resultConfigId, projectId, "resultConfig");

            try
            {
                var sourceResultConfig = await _resultConfigServiceClient.GetResultConfig(resultConfigId);
                var sourceDependantResource = new MarkedResourceModel("metadata", sourceResultConfig.ModelId);
                markSessionModel.SourceDependency = sourceDependantResource;
                await _dbService.InsertNewMarkSession(markSessionModel);

                var markedSourceResultConfig = await _resultConfigServiceClient.CreateMarkedResultConfig(resultConfigId);
                markSessionModel.DependantResources.Add(markedSourceResultConfig);
                await _dbService.UpdateMarkSession(markSessionModel);

                var simPlansForResultConfig =
                    await _simPlanServiceClient.GetSimPlansForResultConfig(resultConfigId, projectId);
                foreach (var simPlanModel in simPlansForResultConfig)
                {
                    var markedSimPlan = await _simPlanServiceClient.MarkSimPlan(simPlanModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimPlan);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                var simRunsForSimPlans = new List<SimRunModel>();
                foreach (var simPlanModel in simPlansForResultConfig)
                {
                    simRunsForSimPlans.AddRange(
                        await _simRunServiceClient.GetSimRunsForSimPlan(simPlanModel.Id, projectId)
                    );
                }
                foreach (var simRunModel in simRunsForSimPlans)
                {
                    var markedSimSun = await _simRunServiceClient.StopSimRun(simRunModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimSun);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                foreach (var simRunModel in simRunsForSimPlans)
                {
                    var markedResultData = await _resultDataServiceClient.CreateMarkedResultData(simRunModel);
                    markSessionModel.DependantResources.Add(markedResultData);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                markSessionModel.State = DbMarkSessionModel.DoneState;
                await _dbService.UpdateMarkSession(markSessionModel);
                _loggerService.LogUpdateEvent(markSessionModel.ToString());

                return new OkObjectResult(markSessionModel.DependantResources);
            }
            catch (Exception e)
            {
                var unused = _errorHandlerService.HandleError(e, markSessionModel);

                return _errorHandlerService.GetStatusCodeForError(e);
            }
        }
    }
}