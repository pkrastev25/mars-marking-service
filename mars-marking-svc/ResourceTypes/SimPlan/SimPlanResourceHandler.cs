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
        private readonly IMarkSessionRepository _markSessionRepository;
        private readonly ILoggerService _loggerService;
        private readonly IErrorService _errorService;

        public SimPlanResourceHandler(
            ISimPlanClient simPlanClient,
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            IMarkSessionRepository markSessionRepository,
            ILoggerService loggerService,
            IErrorService errorService
        )
        {
            _simPlanClient = simPlanClient;
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _markSessionRepository = markSessionRepository;
            _loggerService = loggerService;
            _errorService = errorService;
        }

        public async Task<IActionResult> MarkSimPlanDependantResources(string simPlanId, string projectId)
        {
            var markSessionModel = new MarkSessionModel(simPlanId, projectId, "simPlan");

            try
            {
                await _markSessionRepository.Create(markSessionModel);

                var markedSourceSimPlan = await _simPlanClient.MarkSimPlan(simPlanId, projectId);
                markSessionModel.DependantResources.Add(markedSourceSimPlan);
                await _markSessionRepository.Update(markSessionModel);

                var simRunsForSimPlan = await _simRunClient.GetSimRunsForSimPlan(simPlanId, projectId);
                foreach (var simRunModel in simRunsForSimPlan)
                {
                    var markedSimRun = await _simRunClient.StopSimRun(simRunModel, projectId);
                    markSessionModel.DependantResources.Add(markedSimRun);
                    await _markSessionRepository.Update(markSessionModel);
                }

                foreach (var simRunModel in simRunsForSimPlan)
                {
                    var markedResultData = await _resultDataClient.CreateMarkedResultData(simRunModel);
                    markSessionModel.DependantResources.Add(markedResultData);
                    await _markSessionRepository.Update(markSessionModel);
                }

                markSessionModel.State = MarkSessionModel.DoneState;
                await _markSessionRepository.Update(markSessionModel);
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