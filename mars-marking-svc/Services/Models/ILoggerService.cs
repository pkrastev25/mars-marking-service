using System;
using mars_marking_svc.MarkedResource.Models;

namespace mars_marking_svc.Services.Models
{
    public interface ILoggerService
    {
        void LodCreateEvent(string message);

        void LogUpdateEvent(string message);

        void LogDeleteEvent(string message);

        void LogMarkEvent(string message);

        void LogUnmarkEvent(string message);

        void LogSkipEvent(string message);

        void LogStopEvent(string message);

        void LogErrorEvent(Exception error);

        void LogWarningEvent(string message);
    }
}