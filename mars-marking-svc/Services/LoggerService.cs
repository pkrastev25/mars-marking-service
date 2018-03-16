using System;
using mars_marking_svc.Models;
using mars_marking_svc.Services.Models;

namespace mars_marking_svc.Services
{
    public class LoggerService : ILoggerService
    {
        public void LogMarkedResource(MarkedResourceModel model)
        {
            Console.WriteLine(
                $"[SUCCESS] Marked {model.resourceType} with id: {model.resourceId}"
            );
        }

        public void LogExceptionMessage(Exception error)
        {
            Console.Error.WriteLine(
                $"[ERROR] {error.Message}"
            );
        }

        public void LogExceptionMessageWithStackTrace(Exception error)
        {
            Console.Error.WriteLine($"[ERROR] {error.Message}");
            Console.Error.WriteLine(error.StackTrace);
        }
    }
}