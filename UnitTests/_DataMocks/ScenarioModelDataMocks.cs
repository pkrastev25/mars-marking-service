using System.Collections.Generic;
using mars_marking_svc.ResourceTypes.Scenario.Models;
using Newtonsoft.Json;

namespace UnitTests._DataMocks
{
    public static class ScenarioModelDataMocks
    {
        public static readonly string MockMarkedScenarioModelJson =
            JsonConvert.SerializeObject(MockMarkedScenarioModel());

        public static readonly string MockScenarioModelListJson = JsonConvert.SerializeObject(MockScenarioModelList());

        public static ScenarioModel MockMarkedScenarioModel()
        {
            return new ScenarioModel
            {
                ScenarioId = "9973c1fc-59c7-4072-bd06-b4d530684ccc",
                ToBeDeleted = true,
                ReadOnly = true
            };
        }

        public static ScenarioModel MockScenarioModel()
        {
            return new ScenarioModel
            {
                ScenarioId = "9973c1fc-59c7-4072-bd06-b4d530684ccc",
                ToBeDeleted = false,
                ReadOnly = false
            };
        }

        public static List<ScenarioModel> MockScenarioModelList()
        {
            return new List<ScenarioModel>
            {
                MockMarkedScenarioModel(),
                MockMarkedScenarioModel(),
                MockMarkedScenarioModel()
            };
        }
    }
}