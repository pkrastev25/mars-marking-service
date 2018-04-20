using System;

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

        void LogErrorEvent(Exception error);
    }
}