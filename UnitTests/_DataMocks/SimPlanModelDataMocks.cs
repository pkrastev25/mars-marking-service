using System.Collections.Generic;
using mars_marking_svc.ResourceTypes.SimPlan.Models;
using Newtonsoft.Json;

namespace UnitTests._DataMocks
{
    public static class SimPlanModelDataMocks
    {
        public static readonly string MockSimPlanModelListJson = JsonConvert.SerializeObject(MockSimPlanModelList());

        public static readonly string MockMarkedSimPlanModelJson =
            JsonConvert.SerializeObject(MockMarkedSimPlanModelList());

        public static SimPlanModel MockMarkedSimPlanModel()
        {
            return new SimPlanModel
            {
                Id = "5af5b71495c5e500013bc74a",
                Name = "Trial",
                OwnerId = "8d084e7d-3759-4fd1-b211-07899ac05f00",
                GroupId = "ungrouped",
                ProjectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03",
                ScenarioDescriptionId = "9973c1fc-59c7-4072-bd06-b4d530684ccc",
                ResultConfigurationId = "a28008e6-87be-4692-95bf-353f8f7a1121",
                ExecutionConfigurationId = "",
                DockerImageName = "nexus.informatik.haw-hamburg.de/trial-9973c1fc-59c7-4072-bd06-b4d530684ccc:latest",
                DockerImageCreated = true,
                ToBeDeleted = true
            };
        }

        public static SimPlanModel MockSimPlanModel()
        {
            return new SimPlanModel
            {
                Id = "5af5b71495c5e500013bc74a",
                Name = "Trial",
                OwnerId = "8d084e7d-3759-4fd1-b211-07899ac05f00",
                GroupId = "ungrouped",
                ProjectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03",
                ScenarioDescriptionId = "9973c1fc-59c7-4072-bd06-b4d530684ccc",
                ResultConfigurationId = "a28008e6-87be-4692-95bf-353f8f7a1121",
                ExecutionConfigurationId = "",
                DockerImageName = "nexus.informatik.haw-hamburg.de/trial-9973c1fc-59c7-4072-bd06-b4d530684ccc:latest",
                DockerImageCreated = true,
                ToBeDeleted = false
            };
        }

        public static List<SimPlanModel> MockSimPlanModelList()
        {
            return new List<SimPlanModel>
            {
                MockSimPlanModel(),
                MockSimPlanModel(),
                MockSimPlanModel()
            };
        }

        public static List<SimPlanModel> MockMarkedSimPlanModelList()
        {
            return new List<SimPlanModel>
            {
                MockMarkedSimPlanModel(),
                MockMarkedSimPlanModel(),
                MockMarkedSimPlanModel()
            };
        }
    }
}