using System.Collections.Generic;
using mars_marking_svc.ResourceTypes.SimRun.Models;
using Newtonsoft.Json;

namespace UnitTests._DataMocks
{
    public static class SimRunModelDataMocks
    {
        public static readonly string MockSimRunModelListJson = JsonConvert.SerializeObject(MockSimRunModelList());

        private static readonly string MockMarkedSimRunModelListJson =
            JsonConvert.SerializeObject(MockMarkedSimRunModelList());

        public static SimRunModel MockMarkedSimRunModel()
        {
            return new SimRunModel
            {
                Id = "5af5b7ad95c5e500013bc74c",
                OwnerId = "8d084e7d-3759-4fd1-b211-07899ac05f00",
                GroupId = "ungrouped",
                ProjectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03",
                PodName = "",
                SimPlanId = "5af5b71495c5e500013bc74a",
                SimulationId = "60e3182c-792e-421a-af20-73e1cc62313b",
                ConsoleOutput = "",
                Status = SimRunModel.StatusAborted,
                CurrentTick = 0,
                ToBeDeleted = true
            };
        }

        public static SimRunModel MockSimRunModel()
        {
            return new SimRunModel
            {
                Id = "5af5b7ad95c5e500013bc74c",
                OwnerId = "8d084e7d-3759-4fd1-b211-07899ac05f00",
                GroupId = "ungrouped",
                ProjectId = "e580ff4f-a3b3-4252-81c4-ad88a01cac03",
                PodName = "",
                SimPlanId = "5af5b71495c5e500013bc74a",
                SimulationId = "60e3182c-792e-421a-af20-73e1cc62313b",
                ConsoleOutput = "",
                Status = SimRunModel.StatusAborted,
                CurrentTick = 0,
                ToBeDeleted = false
            };
        }

        public static List<SimRunModel> MockMarkedSimRunModelList()
        {
            return new List<SimRunModel>
            {
                MockMarkedSimRunModel(),
                MockMarkedSimRunModel(),
                MockMarkedSimRunModel()
            };
        }

        public static List<SimRunModel> MockSimRunModelList()
        {
            return new List<SimRunModel>
            {
                MockSimRunModel(),
                MockSimRunModel(),
                MockSimRunModel()
            };
        }
    }
}