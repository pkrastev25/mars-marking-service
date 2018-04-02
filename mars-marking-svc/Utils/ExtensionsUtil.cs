using System;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.Utils
{
    public static class ExtensionsUtil
    {
        public static bool IsMarkSessionExpired(this DbMarkSessionModel dbMarkSessionModel)
        {
            return dbMarkSessionModel.MarkSessionExpireTime < DateTime.Now.Ticks;
        }
    }
}