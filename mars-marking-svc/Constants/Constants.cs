using System;

namespace mars_marking_svc.Constants
{
    public static class Constants
    {
        public static readonly int MarkSessionExpireIntervalForUpdateStateMs = TimeSpan.FromMinutes(5).Milliseconds;

        public static readonly int MarkSessionExpireIntervalForDoneStateMs = TimeSpan.FromHours(12).Milliseconds;

        public static readonly TimeSpan MarkSessionCronServiceTimeInterval = TimeSpan.FromHours(1);
    }
}