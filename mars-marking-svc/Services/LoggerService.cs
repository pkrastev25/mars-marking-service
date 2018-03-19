using System;
using mars_marking_svc.MarkedResource.Models;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.Services
{
    public class LoggerService : ILoggerService
    {
        public void LogMarkedResource(MarkedResourceModel model)
        {
            Console.WriteLine(
                $"[SUCCESS] Marked {model.ResourceType} with id: {model.ResourceId}"
            );
        }

        public void LogUnmarkResource(string resourceType, string resourceId)
        {
            Console.WriteLine(
                $"[SUCCESS] Unmarked {resourceType} with id: {resourceId}"
            );
        }

        public void LogExceptionMessage(string exceptionMessage)
        {
            Console.Error.WriteLine(
                $"[ERROR] {exceptionMessage}"
            );
        }

        public void LogExceptionMessage(Exception error)
        {
            LogExceptionMessage(error.Message);
        }

        public void LogExceptionMessageWithStackTrace(Exception error)
        {
            LogExceptionMessage(error.Message);
            Console.Error.WriteLine(error.StackTrace);
        }
    }
}