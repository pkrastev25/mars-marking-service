using System;

namespace mars_marking_svc.Constants
{
    public static class Constants
    {
        public static readonly long MarkSessionExpireIntervalForUpdateStateTicks = TimeSpan.FromMinutes(5).Ticks;

        public static readonly long MarkSessionExpireIntervalForDoneStateTicks = TimeSpan.FromHours(1).Ticks;

        public static readonly TimeSpan MarkSessionCronServiceTimeInterval = TimeSpan.FromHours(1);
    }
}