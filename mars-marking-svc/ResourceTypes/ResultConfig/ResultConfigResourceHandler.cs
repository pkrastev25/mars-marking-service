using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.ResourceTypes.ResultConfig
{
    public class ResultConfigResourceHandler : IResultConfigResourceHandler
    {
        private readonly IResultConfigClient _resultConfigClient;
        private readonly ISimPlanClient _simPlanClient;
        private readonly ISimRunClient _simRunClient;
        private readonly IResultDataClient _resultDataClient;
        private readonly IMarkSessionRepository _markSessionRepository;

        public ResultConfigResourceHandler(
            IResultConfigClient resultConfigClient,
            ISimPlanClient simPlanClient,
            ISimRunClient simRunClient,
            IResultDataClient resultDataClient,
            IMarkSessionRepository markSessionRepository
        )
        {
            _resultConfigClient = resultConfigClient;
            _simPlanClient = simPlanClient;
            _simRunClient = simRunClient;
            _resultDataClient = resultDataClient;
            _markSessionRepository = markSessionRepository;
        }

        public async Task<MarkSessionModel> GatherResourcesForMarkSession(MarkSessionModel markSessionModel)
        {
            var resultConfigId = markSessionModel.ResourceId;
            var projectId = markSessionModel.ProjectId;

            var sourceResultConfig = await _resultConfigClient.GetResultConfig(resultConfigId);
            var sourceDependantResource = new DependantResourceModel("metadata", sourceResultConfig.ModelId);
            markSessionModel.SourceDependency = sourceDependantResource;
            await _markSessionRepository.Update(markSessionModel);

            var markedSourceResultConfig = await _resultConfigClient.CreateMarkedResultConfig(resultConfigId);
            markSessionModel.DependantResources.Add(markedSourceResultConfig);
            await _markSessionRepository.Update(markSessionModel);

            var simPlansForResultConfig =
                await _simPlanClient.GetSimPlansForResultConfig(resultConfigId, projectId);
            foreach (var simPlanModel in simPlansForResultConfig)
            {
                var markedSimPlan = await _simPlanClient.MarkSimPlan(simPlanModel, projectId);
                markSessionModel.DependantResources.Add(markedSimPlan);
                await _markSessionRepository.Update(markSessionModel);
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
                await _markSessionRepository.Update(markSessionModel);
            }

            foreach (var simRunModel in simRunsForSimPlans)
            {
                var markedResultData = await _resultDataClient.CreateMarkedResultData(simRunModel);
                markSessionModel.DependantResources.Add(markedResultData);
                await _markSessionRepository.Update(markSessionModel);
            }

            markSessionModel.State = MarkSessionModel.DoneState;
            await _markSessionRepository.Update(markSessionModel);

            return markSessionModel;
        }
    }
}