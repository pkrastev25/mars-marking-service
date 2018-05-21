using System;

namespace mars_marking_svc.Constants
{
    public static class Constants
    {
        public const string ProjectSvcUrlKey = "PROJECT_SVC_URL";
        public const string MetadataSvcUrlKey = "METADATA_SVC_URL";
        public const string ScenarioSvcUrlKey = "SCENARIO_SVC_URL";
        public const string ResultConfigSvcUrlKey = "RESULT_CONFIG_SVC_URL";
        public const string SimRunnerSvcUrlKey = "SIM_RUNNER_SVC_URL";
        public const string DatabaseUtilitySvcUrlKey = "DATABASE_UTILITY_SVC_URL";
        public const string MongoDbSvcUrlKey = "MONGO_DB_SVC_URL";

        public static readonly long MarkSessionUpdateReferenceTimeInTicks = TimeSpan.FromMinutes(2).Ticks;
    }
}