using System;
using System.Collections.Generic;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.MarkedResource.Enums;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace UnitTests._DataMocks
{
    public static class MarkSessionModelDataMocks
    {
        public static readonly string MockMarkSessionModelJson = JsonConvert.SerializeObject(MockMarkSessionModel());

        public static readonly string MockMarkSessionModelListJson =
            JsonConvert.SerializeObject(MockMarkSessionModelList());

        public static MarkSessionModel MockMarkSessionModel()
        {
            return new MarkSessionModel(
                "be1cabd5-c121-49a0-9860-824419efb39a",
                "be1cabd5-c121-49a0-9860-824419efb39a",
                ResourceTypeEnum.Project,
                MarkSessionTypeEnum.ToBeDeleted
            )
            {
                Id = new ObjectId("5ae86f68b90b230007d7ea34"),
                State = MarkSessionModel.StateComplete,
                LatestUpdateTimestampInTicks = DateTime.Now.Ticks,
                SourceDependency = null,
                DependantResources = new List<DependantResourceModel>
                {
                    new DependantResourceModel(
                        ResourceTypeEnum.Metadata,
                        "acd8b6d6-5490-4240-9cf3-045b214c7912"
                    )
                    {
                        PreviousState = MetadataModel.FinishedState
                    },
                    new DependantResourceModel(
                        ResourceTypeEnum.Scenario,
                        "3f9c4a5b-c4db-4098-bea9-333cacdc58b1"
                    ),
                    new DependantResourceModel(
                        ResourceTypeEnum.ResultConfig,
                        "3f9c4a5b-c4db-4098-bea9-333cacdc58b1"
                    ),
                    new DependantResourceModel(
                        ResourceTypeEnum.SimPlan,
                        "3f9c4a5b-c4db-4098-bea9-333cacdc58b1"
                    ),
                    new DependantResourceModel(
                        ResourceTypeEnum.SimRun,
                        "3f9c4a5b-c4db-4098-bea9-333cacdc58b1"
                    ),
                    new DependantResourceModel(
                        ResourceTypeEnum.ResultData,
                        "3f9c4a5b-c4db-4098-bea9-333cacdc58b1"
                    )
                }
            };
        }

        public static List<MarkSessionModel> MockMarkSessionModelList()
        {
            return new List<MarkSessionModel>
            {
                MockMarkSessionModel(),
                MockMarkSessionModel(),
                MockMarkSessionModel()
            };
        }
    }
}