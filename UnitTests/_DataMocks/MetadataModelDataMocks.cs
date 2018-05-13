using System.Collections.Generic;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using Newtonsoft.Json;

namespace UnitTests._DataMocks
{
    public static class MetadataModelDataMocks
    {
        public static readonly string MockToBeDeletedMetadataModelJson =
            JsonConvert.SerializeObject(MockToBeDeletedMetadataModel());

        public static readonly string MockFinishedMetadataModelJson =
            JsonConvert.SerializeObject(MockFinishedMetadataModel());

        public static MetadataModel MockToBeDeletedMetadataModel()
        {
            return new MetadataModel
            {
                DataId = "acd8b6d6-5490-4240-9cf3-045b214c7912",
                State = MetadataModel.ToBeDeletedState
            };
        }

        public static MetadataModel MockFinishedMetadataModel()
        {
            return new MetadataModel
            {
                DataId = "acd8b6d6-5490-4240-9cf3-045b214c7912",
                State = MetadataModel.FinishedState
            };
        }

        public static List<MetadataModel> MockFinishedMetadataModelList()
        {
            return new List<MetadataModel>
            {
                MockFinishedMetadataModel(),
                MockFinishedMetadataModel(),
                MockFinishedMetadataModel()
            };
        }
    }
}