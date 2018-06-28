using mars_marking_svc.ResourceTypes;
using mars_marking_svc.ResourceTypes.MarkedResource.Enums;

namespace mars_marking_svc.Utils
{
    public static class EnumUtil
    {
        public static bool DoesResourceTypeExist(
            string resourceType
        )
        {
            switch (resourceType)
            {
                case ResourceTypeEnum.Project:
                case ResourceTypeEnum.Metadata:
                case ResourceTypeEnum.Scenario:
                case ResourceTypeEnum.ResultConfig:
                case ResourceTypeEnum.SimPlan:
                case ResourceTypeEnum.SimRun:
                case ResourceTypeEnum.ResultData:
                    return true;
                default:
                    return false;
            }
        }

        public static bool DoesMarkSessionTypeExist(
            string markSessionType
        )
        {
            switch (markSessionType)
            {
                case MarkSessionTypeEnum.ToBeDeleted:
                case MarkSessionTypeEnum.ToBeArchived:
                    return true;
                default:
                    return false;
            }
        }
    }
}