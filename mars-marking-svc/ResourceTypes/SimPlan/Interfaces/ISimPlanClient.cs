using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.SimPlan.Models;

namespace mars_marking_svc.ResourceTypes.SimPlan.Interfaces
{
    public interface ISimPlanClient
    {
        Task<SimPlanModel> GetSimPlan(
            string simPlanId,
            string projectId
        );

        Task<List<SimPlanModel>> GetSimPlansForScenario(
            string scenarioId,
            string projectId
        );

        Task<List<SimPlanModel>> GetSimPlansForResultConfig(
            string resultConfigId,
            string projectId
        );

        Task<List<SimPlanModel>> GetSimPlansForProject(
            string projectId
        );

        Task<DependantResourceModel> MarkSimPlan(
            string simPlanId,
            string projectId
        );

        Task<DependantResourceModel> MarkSimPlan(
            SimPlanModel simPlanModel,
            string projectId
        );

        Task UnmarkSimPlan(
            DependantResourceModel dependantResourceModel,
            string projectId
        );
    }
}