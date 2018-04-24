using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.SimRun.Models;

namespace mars_marking_svc.ResourceTypes.SimRun.Interfaces
{
    public interface ISimRunClient
    {
        Task<SimRunModel> GetSimRun(
            string simRunId,
            string projectId
        );

        Task<List<SimRunModel>> GetSimRunsForSimPlan(
            string simPlanId,
            string projectId
        );

        Task<List<SimRunModel>> GetSimRunsForProject(
            string projectId
        );

        Task<DependantResourceModel> CreateDependantSimRunResource(
            string simRunId,
            string projectId
        );

        Task<DependantResourceModel> CreateDependantSimRunResource(
            SimRunModel simRunModel,
            string projectId
        );
    }
}