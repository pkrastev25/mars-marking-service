using System;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.SimPlan
{
    public class SimPlanResourceHandler : ISimPlanResourceHandler
    {
        private readonly ISimPlanClient _simPlanClient;
        private readonly ISimRunClient _simRunClient;
        private readonly IResultDataClient _resultDataClient;
        private readonly IDbMarkSessionClient _dbMarkSessionClient;
        private readonly ILoggerService _loggerService;
        private readonly IErrorHandlerService _errorHandlerService;

        public SimPlanResourceHandler(
            ISimPlanClient simPlanClient,
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            IDbMarkSessionClient dbMarkSessionClient,
            ILoggerService loggerService,
            IErrorHandlerService errorHandlerService
        )
        {
            _simPlanClient = simPlanClient;
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _dbMarkSessionClient = dbMarkSessionClient;
            _loggerService = loggerService;
            _errorHandlerService = errorHandlerService;
        }

        public async Task<IActionResult> MarkSimPlanDependantResources(string simPlanId, string projectId)
        {
            var markSessionModel = new DbMarkSessionModel(simPlanId, projectId, "simPlan");

            try
            {
                await _dbMarkSessionClient.Create(markSessionModel);

                var markedSourceSimPlan = await _simPlanClient.MarkSimPlan(simPlanId, projectId);
                markSessionModel.DependantResources.Add(markedSourceSimPlan);
                await _dbMarkSessionClient.Update(markSessionModel);

                var simRunsForSimPlan = await _simRunClient.GetSimRunsForSimPlan(simPlanId, projectId);
                foreach (var simRunModel in simRunsForSimPlan)
                {
                    var markedSimRun = await _simRunClient.StopSimRun(simRunModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimRun);
                    await _dbMarkSessionClient.Update(markSessionModel);
                }

                foreach (var simRunModel in simRunsForSimPlan)
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
                var unused = _errorHandlerService.HandleError(e, markSessionModel);

                return _errorHandlerService.GetStatusCodeForError(e);
            }
        }
    }
}