using System;
using mars_marking_svc.Models;

namespace mars_marking_svc.Services.Models
{
    public interface ILoggerService
    {
        void LogMarkedResource(MarkedResourceModel model);

        void LogError(Exception error);
    }
}