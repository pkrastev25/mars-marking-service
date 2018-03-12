using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.Models;

namespace mars_marking_svc.ResourceTypes.Scenario.Models
{
    public interface IScenarioServiceClient
    {
        Task<ScenarioModel> GetScenario(string scenarioId);

        Task<MarkedResourceModel> MarkScenario(string scenarioId);

        Task<MarkedResourceModel> MarkScenario(ScenarioModel scenarioModel);

        Task<List<ScenarioModel>> GetScenariosForMetadata(string metadataId);

        Task<List<ScenarioModel>> GetScenariosForProject(string projectId);
    }
}