using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Models;

namespace mars_marking_svc.ResourceTypes.SimPlan.Models
{
    public interface ISimPlanServiceClient
    {
        Task<SimPlanModel> GetSimPlan(string simPlanId, string projectId);

        Task<MarkedResourceModel> MarkSimPlan(string simPlanId, string projectId);

        Task<MarkedResourceModel> MarkSimPlan(SimPlanModel simPlanModel, string projectId);

        Task<List<SimPlanModel>> GetSimPlansForProject(string projectId);

        Task<List<SimPlanModel>> GetSimPlansForScenario(string scenarioId, string projectId);

        Task<List<SimPlanModel>> GetSimPlansForResultConfig(string resultConfigId, string projectId);
    }
}