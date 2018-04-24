using System;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.Utils
{
    public static class MarkSessionExtensions
    {
        public static bool IsMarkSessionRecentlyUpdated(
            this MarkSessionModel markSessionModel
        )
        {
            var timePassedInTicks = DateTime.Now.Ticks - markSessionModel.LatestUpdateTimestampInTicks;

            return timePassedInTicks < Constants.Constants.MarkSessionUpdateReferenceTimeInTicks;
        }
    }
}