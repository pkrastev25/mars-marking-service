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
        private readonly ISimPlanServiceClient _simPlanServiceClient;
        private readonly ISimRunServiceClient _simRunServiceClient;
        private readonly IResultDataServiceClient _resultDataServiceClient;
        private readonly IDbService _dbService;
        private readonly ILoggerService _loggerService;
        private readonly IErrorHandlerService _errorHandlerService;

        public SimPlanResourceHandler(
            ISimPlanServiceClient simPlanServiceClient,
            ISimRunServiceClient simRunServiceClient,
            IResultDataServiceClient resultDataServiceClient,
            IDbService dbService,
            ILoggerService loggerService,
            IErrorHandlerService errorHandlerService
        )
        {
            _simPlanServiceClient = simPlanServiceClient;
            _simRunServiceClient = simRunServiceClient;
            _resultDataServiceClient = resultDataServiceClient;
            _dbService = dbService;
            _loggerService = loggerService;
            _errorHandlerService = errorHandlerService;
        }

        public async Task<IActionResult> MarkSimPlanDependantResources(string simPlanId, string projectId)
        {
            var markSessionModel = new DbMarkSessionModel(simPlanId, projectId, "simPlan");

            try
            {
                await _dbService.InsertNewMarkSession(markSessionModel);

                var markedSourceSimPlan = await _simPlanServiceClient.MarkSimPlan(simPlanId, projectId);
                markSessionModel.DependantResources.Add(markedSourceSimPlan);
                await _dbService.UpdateMarkSession(markSessionModel);

                var simRunsForSimPlan = await _simRunServiceClient.GetSimRunsForSimPlan(simPlanId, projectId);
                foreach (var simRunModel in simRunsForSimPlan)
                {
                    var markedSimRun = await _simRunServiceClient.StopSimRun(simRunModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimRun);
                    await _dbService.UpdateMarkSession(markSessionModel);
                }

                foreach (var simRunModel in simRunsForSimPlan)
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