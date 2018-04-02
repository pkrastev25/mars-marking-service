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
        private readonly IResultConfigClient _resultConfigClient;
        private readonly ISimPlanClient _simPlanClient;
        private readonly ISimRunClient _simRunClient;
        private readonly IResultDataClient _resultDataClient;
        private readonly IDbMarkSessionClient _dbMarkSessionClient;
        private readonly ILoggerService _loggerService;
        private readonly IErrorService _errorService;

        public ResultConfigResourceHandler(
            IResultConfigClient resultConfigClient,
            ISimPlanClient simPlanClient,
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            IDbMarkSessionClient dbMarkSessionClient,
            ILoggerService loggerService,
            IErrorService errorService
        )
        {
            _resultConfigClient = resultConfigClient;
            _simPlanClient = simPlanClient;
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _dbMarkSessionClient = dbMarkSessionClient;
            _loggerService = loggerService;
            _errorService = errorService;
        }

        public async Task<IActionResult> MarkResultConfigDependantResources(string resultConfigId, string projectId)
        {
            var markSessionModel = new DbMarkSessionModel(resultConfigId, projectId, "resultConfig");

            try
            {
                var sourceResultConfig = await _resultConfigClient.GetResultConfig(resultConfigId);
                var sourceDependantResource = new MarkedResourceModel("metadata", sourceResultConfig.ModelId);
                markSessionModel.SourceDependency = sourceDependantResource;
                await _dbMarkSessionClient.Create(markSessionModel);

                var markedSourceResultConfig = await _resultConfigClient.CreateMarkedResultConfig(resultConfigId);
                markSessionModel.DependantResources.Add(markedSourceResultConfig);
                await _dbMarkSessionClient.Update(markSessionModel);

                var simPlansForResultConfig =
                    await _simPlanClient.GetSimPlansForResultConfig(resultConfigId, projectId);
                foreach (var simPlanModel in simPlansForResultConfig)
                {
                    var markedSimPlan = await _simPlanClient.MarkSimPlan(simPlanModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimPlan);
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                var simRunsForSimPlans = new List<SimRunModel>();
                foreach (var simPlanModel in simPlansForResultConfig)
                {
                    simRunsForSimPlans.AddRange(
                        await _simRunClient.GetSimRunsForSimPlan(simPlanModel.Id, projectId)
                    );
                }
                foreach (var simRunModel in simRunsForSimPlans)
                {
                    var markedSimSun = await _simRunClient.StopSimRun(simRunModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimSun);
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                foreach (var simRunModel in simRunsForSimPlans)
                {
                    var markedResultData = await _resultDataClient.CreateMarkedResultData(simRunModel);
                    markSessionModel.DependantResources.Add(markedResultData);
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                markSessionModel.State = DbMarkSessionModel.DoneState;
                await _dbMarkSessionClient.Update(markSessionModel);
                _loggerService.LogUpdateEvent(markSessionModel.ToString());

                return new OkObjectResult(markSessionModel.DependantResources);
            }
            catch (Exception e)
            {
                _errorService.HandleError(e, markSessionModel);

                return _errorService.GetStatusCodeForError(e);
            }
        }
    }
}