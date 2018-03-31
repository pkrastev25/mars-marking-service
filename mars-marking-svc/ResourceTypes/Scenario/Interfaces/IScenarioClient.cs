using System.Collections.Generic;
using System.Threading.Tasks;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes.Scenario.Models;

namespace mars_marking_svc.ResourceTypes.Scenario.Interfaces
{
    public interface IScenarioClient
    {
        Task<ScenarioModel> GetScenario(string scenarioId);

        Task<List<ScenarioModel>> GetScenariosForMetadata(string metadataId);

        Task<List<ScenarioModel>> GetScenariosForProject(string projectId);

        Task<MarkedResourceModel> MarkScenario(string scenarioId);

        Task<MarkedResourceModel> MarkScenario(ScenarioModel scenarioModel);

        Task UnmarkScenario(MarkedResourceModel markedResourceModel);
    }
}