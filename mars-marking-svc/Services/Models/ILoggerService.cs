using System;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.Services.Models
{
    public interface ILoggerService
    {
        void LogMarkedResource(MarkedResourceModel model);

        void LogUnmarkResource(string resourceType, string id);

        void LogExceptionMessage(string exceptionMessage);

        void LogExceptionMessage(Exception error);

        void LogExceptionMessageWithStackTrace(Exception error);
    }
}