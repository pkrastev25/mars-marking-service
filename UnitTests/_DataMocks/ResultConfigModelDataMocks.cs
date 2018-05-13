using System.Collections.Generic;
using mars_marking_svc.ResourceTypes.ResultConfig.Models;
using Newtonsoft.Json;

namespace UnitTests._DataMocks
{
    public static class ResultConfigModelDataMocks
    {
        public static readonly string MockResultConfigResponseModelJson =
            JsonConvert.SerializeObject(MockResultConfigResponseModel());

        public static readonly string MockResultConfigResponseModelListJson =
            JsonConvert.SerializeObject(MockResultConfigResponseModelList());

        public static ResultConfigResponseModel MockResultConfigResponseModel()
        {
            return new ResultConfigResponseModel
            {
                ResultConfigModel = MockResultConfigModel()
            };
        }

        public static List<ResultConfigResponseModel> MockResultConfigResponseModelList()
        {
            return new List<ResultConfigResponseModel>
            {
                MockResultConfigResponseModel(),
                MockResultConfigResponseModel(),
                MockResultConfigResponseModel()
            };
        }

        public static ResultConfigModel MockResultConfigModel()
        {
            return new ResultConfigModel
            {
                ConfigId = "a28008e6-87be-4692-95bf-353f8f7a1121",
                ModelId = "730cf672-0368-4f9e-a36f-6e4d71308000"
            };
        }
    }
}