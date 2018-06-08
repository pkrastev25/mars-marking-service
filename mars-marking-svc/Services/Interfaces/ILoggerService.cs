using System;

namespace mars_marking_svc.Services.Models
{
    public interface ILoggerService
    {
        void LogInfoEvent(
            string message
        );
        
        void LogInfoWithErrorEvent(
            string message,
            Exception exception
        );

        void LogBackgroundJobInfoEvent(
            string message
        );

        void LogBackgroundJobErrorEvent(
            Exception error
        );

        void LogStartupInfoEvent(
            string message
        );

        void LogStartupErrorEvent(
            Exception exception
        );
    }
}