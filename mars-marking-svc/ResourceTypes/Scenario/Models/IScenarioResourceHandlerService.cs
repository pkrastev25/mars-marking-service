using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace mars_marking_svc.ResourceTypes.Scenario.Models
{
    public interface IScenarioResourceHandlerService
    {
        Task<IActionResult> MarkScenarioDependantResources(string scenarioId);
    }
}