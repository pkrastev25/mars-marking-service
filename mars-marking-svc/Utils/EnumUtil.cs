namespace mars_marking_svc.Utils
{
    public class EnumUtil
    {
        public static bool DoesResourceTypeExist(string resourceType)
        {
            switch (resourceType)
            {
                case "project":
                case "metadata":
                case "scenario":
                case "resultConfig":
                case "simPlan":
                case "simRun":
                case "resultData":
                    return true;
                default:
                    return false;
            }
        }

        public static bool DoesMarkSessionTypeExist(string markSessionType)
        {
            switch (markSessionType)
            {
                case "TO_BE_DELETED":
                case "TO_BE_ARCHIVED":
                    return true;
                default:
                    return false;
            }
        }
    }
}