using System;

namespace mars_marking_svc.Constants
{
    public static class Constants
    {
        public const string MetadataSvcUrlKey = "METADATA_SVC_URL";

        public static readonly long MarkSessionUpdateReferenceTimeInTicks = TimeSpan.FromMinutes(2).Ticks;
    }
}