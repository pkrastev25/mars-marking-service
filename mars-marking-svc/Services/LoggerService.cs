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
                $"[SUCCESS] Marked resource with resource type: {model.resourceType} and id: {model.resourceId}!"
            );
        }

        public void LogError(Exception error)
        {
            Console.Error.WriteLine(
                $"[ERROR] {error.Message}, stack trace:\n {error.StackTrace}!"
            );
        }
    }
}