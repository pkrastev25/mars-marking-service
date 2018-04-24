using System;

namespace mars_marking_svc.Constants
{
    public static class Constants
    {
        public static readonly long MarkSessionUpdateReferenceTimeInTicks = TimeSpan.FromMinutes(2).Ticks;
    }
}